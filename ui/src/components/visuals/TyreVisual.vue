<script setup lang="ts" >
import { computed } from 'vue';
import type { TyreMetric } from '../../types/telemetry';

const props = defineProps<{
    isFront: boolean,
    isLeft: boolean,
    angle: number,
    tyre: TyreMetric
}>();

const getTyreColorClass = (temp: number) => {
  if (temp < 60) return 'text-blue-950 bg-blue-500'       // Cold
  if (temp >= 60 && temp <= 100) return 'text-emerald-950 bg-emerald-500' // Ideal Range
  return 'text-rose-950 bg-rose-500'       // Overheating
}

const appliedAngle = computed(() => {
    return props.isFront ? Math.min(props.angle/4, 15) : 0;
});

</script>

<template>
    <div class="flex relative"
    :style="{
        flexFlow: `${isLeft ? 'row-reverse' : 'row'}`,
        transformOrigin: `${isLeft ? '90%' : '10%'} 50%`,
        rotate: `${appliedAngle}deg`
        }">
        <div v-if="isFront" class="-z-1 absolute w-16 h-full"
        :style="{
            left: `${isLeft ? '50%' : '-50%'}`,
            transform: `translate(${isLeft ? '-10%' : '10%'})`
        }">
            <div class="relative w-full h-full" :style="{ transform: `scale(${isLeft ? '-1' : '1'},1)`}">
                <div class="absolute h-16 aspect-square rounded-full top-1/2 -translate-y-1/2 border-2 border-dotted" />
                <div class="absolute h-0.5 w-3 top-1/2 -translate-y-1/2 -left-2.5 bg-black rounded-full" />
            </div>
        </div>
        <div v-if="tyre"
        :class="`flex items-center justify-center w-20 h-30 rounded-2xl transition-colors duration-500 ${getTyreColorClass(tyre.temperature)}`"
        :style="{ width: `${isFront ? '4rem' : '5rem'}` }">
            <div
            class="w-full flex flex-col items-center"
            :style="{
                rotate: `${-appliedAngle}deg`,
                transformOrigin: `${isLeft ? '90%' : '10%'} 50%`}">
                <div class="text-center font-medium text-lg">
                    {{ Math.round(tyre.temperature) }}&#xB0;C
                </div>
                <div v-if="isFront" class="text-xs">
                    {{ appliedAngle.toFixed(1) }}&#xB0;
                </div>
            </div>
        </div>
        <div v-else class="flex items-center justify-center w-20 h-30 rounded-2xl bg-gray-950 text-gray-300 animate-pulse">
            <div class="w-full text-center font-thin text-lg">
                Error
            </div>
        </div>
    </div>
</template>