<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import type { ParametersPackage, TelemetryPackage } from './types/telemetry'
import TyreFrameVisual from './components/visuals/TyreFrameVisual.vue';
import FuelVisual from './components/visuals/FuelVisual.vue';
import SteeringVisual from './components/visuals/SteeringVisual.vue';
import GearVisual from './components/visuals/GearVisual.vue';
import DialVisual from './components/visuals/DialVisual.vue';
import PedalVisual from './components/visuals/PedalVisual.vue';
import TemperatureVisual from './components/visuals/TemperatureVisual.vue';
import TrackVisual from './components/visuals/TrackVisual.vue';
import LapVisual from './components/visuals/LapVisual.vue';
import DistanceVisual from './components/visuals/DistanceVisual.vue';
import TrackNameVisual from './components/visuals/TrackNameVisual.vue';
import LapTimeVisual from './components/visuals/LapTimeVisual.vue';
import FlagVisual from './components/visuals/FlagVisual.vue';
import SteeringGraph from './components/Graphs/SteeringGraph.vue';
import SpeedGraph from './components/Graphs/SpeedGraph.vue';
import FlagGraph from './components/Graphs/FlagGraph.vue';
import LapGraph from './components/Graphs/LapGraph.vue';

const telemetryHistory = ref<TelemetryPackage[]>([]);
const maxHistory = 200;
const lapTimeHistory = ref<number[]>([]);

const telemetry = ref<TelemetryPackage | null>(null);
const parameters = ref<ParametersPackage | null>(null);
const error = ref<string | null>(null)
let pollingInterval: number | null = null

const bestLap = ref<number>(-1);
const bestLapTimeMs = ref<number>(-1);
const lastFetchLapTimeMs = ref<number>(0);

const fetchTelemetry = async () => {
  try {
    const response = await fetch('http://localhost:5189/api/telemetry')
    if (!response.ok) throw new Error('Engine telemetry stream offline')
    
    telemetry.value = await response.json();

    if (telemetry.value) {
      telemetryHistory.value.push(telemetry.value);
      if (telemetryHistory.value.length > maxHistory) {
        telemetryHistory.value.shift();
      }
      
      if (telemetry.value.flag == "Chequered") {
        if (pollingInterval) clearInterval(pollingInterval)
      }

      if (telemetry.value.isNewLap || telemetry.value.lapTimeMs < lastFetchLapTimeMs.value) {
        const newTime = telemetry.value.isNewLap ? telemetry.value.lapTimeMs : lastFetchLapTimeMs.value;
        lapTimeHistory.value.push(newTime);
        
        if (bestLap.value === -1 || newTime < bestLapTimeMs.value) {
          bestLapTimeMs.value = newTime;
          bestLap.value = telemetry.value.lapNumber-1;
        }
      }

      lastFetchLapTimeMs.value = telemetry.value.lapTimeMs;
    }

    error.value = null
  } catch (err: any) {
    error.value = err.message
  }
}

const fetchParameters = async () => {
  try {
    const response = await fetch('http://localhost:5189/api/parameters')
    if (!response.ok) throw new Error('Engine parameters endpoint offline')
    
    parameters.value = await response.json()
    error.value = null
  } catch (err: any) {
    error.value = err.message
  }
}

onMounted(() => {
  fetchParameters();
  pollingInterval = window.setInterval(fetchTelemetry, parameters.value?.intervalMs ?? 100);
})

onUnmounted(() => {
  if (pollingInterval) clearInterval(pollingInterval)
})

</script>

<template>
  <div v-if="telemetry && parameters" class="flex h-120">
    <div class="flex p-2">
      <TyreFrameVisual :tyres="telemetry.tyres" :angle="telemetry.steeringAngle" />
    </div>
    <div class="w-28 flex flex-col p-2 gap-4">
      <TemperatureVisual :value="telemetry.engineTemperature" :cool-threshold="100" :ideal-threshold="120" name="Engine" />
      <FuelVisual :fuel="telemetry.fuelRemainingLitres" :maxFuel="parameters.initialFuel" />
    </div>
    <div class="w-28 h-3/4 flex flex-col p-2 gap-4 ml-20">
      <DialVisual :value="telemetry.speed" :max="parameters.maxSpeed" name="Speed" />
      <PedalVisual :value="telemetry.brake" :max="1" :segments="10" name="Brake" color="#ff2056" />
    </div>
    <div class="w-28 flex flex-col p-2 gap-4">
      <GearVisual :gear="telemetry.gear" />
      <SteeringVisual :angle="telemetry.steeringAngle" />
    </div>
    <div class="w-28 h-3/4 flex flex-col p-2 gap-4">
      <DialVisual :value="telemetry.engineRevs" :max="11500" name="RPM" />
      <PedalVisual :value="telemetry.throttle" :max="1" :segments="10" name="Throttle" color="#efb100" />
    </div>
    <div class="w-md flex flex-col p-2 gap-4 ml-20">
      <div class="flex gap-4">
        <TrackNameVisual :name="parameters.track.name" />
        <FlagVisual :flag="telemetry.flag" />
      </div>
      <div>
        <TrackVisual :track="parameters.track" :lap-length="parameters.lapLength" :lap-number="telemetry.lapNumber" :distance-travelled="telemetry.distanceTravelled" />
      </div>
      <div class="flex gap-4">
        <LapVisual :lap="telemetry.lapNumber" :laps="parameters.track.laps" />
        <DistanceVisual :dist="telemetry.distanceTravelled" />
      </div>
      <div class="flex gap-4">
        <LapTimeVisual :lap-time="telemetry.lapTimeMs" name="Current Lap" />
        <LapTimeVisual :lap-time="bestLapTimeMs" :name="`Best Lap (${bestLap})`" />
      </div>
    </div>
  </div>
  <div class="p-2 flex gap-4">
    <SteeringGraph :history="telemetryHistory" />
    <SpeedGraph :history="telemetryHistory" :parameters="parameters" />
    <FlagGraph :history="telemetryHistory" />
    <LapGraph :lap-times="lapTimeHistory" />
  </div>
  <div v-if="error">Error: {{ error }}</div>
  <div v-else>Status: OK</div>
  <pre>
    {{ JSON.stringify(telemetry, null, 2) }}
  </pre>
  <br>
  <br>
  <pre>
    {{ JSON.stringify(parameters, null, 2) }}
  </pre>
</template>
