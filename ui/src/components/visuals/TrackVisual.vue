<script setup lang="ts">
import { computed } from 'vue';
import type { Track } from '../types/telemetry';


const props = defineProps<{
    track: Track,
    lapLength: number,
    lapNumber: number,
    distanceTravelled: number,

}>();

const lapProgress = computed(() => {
    const lapDistance = props.distanceTravelled % props.lapLength;
    return (lapDistance / props.lapLength) * 100;
});

const latestSegmentProgress = computed(() => {
    const numberOfSegments = props.track.segments.length;
    let latestSegmentDistance = 0;
    for (let i = 0; i < numberOfSegments; i++) {
        const seg1 = props.track.segments[i];
        if (segmentProgress(seg1.startDistance) < lapProgress.value) { // We've passed the start of this segment
            if (i == numberOfSegments-1) { // Last segment
                latestSegmentDistance = seg1.startDistance;
            } else {
                const seg2 = props.track.segments[i+1 % numberOfSegments];
                if (segmentProgress(seg2.startDistance) > lapProgress.value) { // We're in between these segments
                    latestSegmentDistance = seg1.startDistance;
                    break;
                }
            }
        }
    }
    return (latestSegmentDistance / props.lapLength) * 100
});

const currentSegmentProgress = computed(() => {
    return lapProgress.value - latestSegmentProgress.value;
});

const segmentProgress = (dist: number) => {
    return (dist / props.lapLength) * 100;
}

</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full h-fit p-3 rounded-3xl items-center">
        
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none mb-1 mt-0.5">
            Race Progress
        </div>

        <div class="w-full h-17 bg-gray-400 rounded-2xl flex flex-col p-3 gap-2">
            <div class="relative h-2/3">
                <div class="absolute h-0.5 w-full top-1/2 -translate-y-1/2 bg-gray-500 rounded-full" />
                <div class="absolute h-0.5 top-1/2 -translate-y-1/2 bg-white rounded-l-full"
                :style="{ width: `${lapProgress}%` }" />
                <div class="absolute h-1 top-1/2 -translate-y-1/2 bg-white transition-all"
                :style="{ left: `${latestSegmentProgress}%`, width: `${currentSegmentProgress}%` }" />
                <div v-for="segment in track.segments" :key="segment.startDistance"
                class="absolute h-1/3 w-0.5 rounded-full top-1/2 -translate-y-1/2"
                :style="{
                    left: `${segmentProgress(segment.startDistance)}%`,
                    transform: `translateX(${-segmentProgress(segment.startDistance)}%)`,
                    backgroundColor: segmentProgress(segment.startDistance) > lapProgress ? '#6a7282' : 'white' }" />
                <div class="absolute h-full w-1 rounded-full bg-rose-500 top-1/2 -translate-y-1/2"
                :style="{ left: `${lapProgress}%`, transform: `translateX(${-lapProgress}%)` }" />
            </div>
            <div class="flex w-full h-1/3 justify-between pt-0 gap-0.75">
                <div v-for="lap in new Array(track.laps).fill(null).map((_, i) => i + 1)" :key="lap"
                    class="h-full w-full rounded-full transition-colors" :style="{ backgroundColor: lapNumber == lap ? '#ff2056' : lap < lapNumber ? 'white' : 'black' }" />
            </div>
        </div>
    </div>
</template>