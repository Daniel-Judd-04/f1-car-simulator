<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
    fuel: number,
    maxFuel: number
}>();

const fuelPercentage = computed(() => {
    const pct = (props.fuel / props.maxFuel) * 100
    return Math.max(0, Math.min(100, pct))
});

const tabs = 50;
const criticalPercentage = 10;
const largeTickFrequency = 5;
const criticalLimit = tabs - (tabs*(criticalPercentage/100));
</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full h-full p-3 rounded-3xl items-center">
        
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none mb-1 mt-0.5">
            Fuel
        </div>

        <div class="relative bg-gray-400 rounded-b-xl rounded-t h-full w-full overflow-hidden">
            <div class="absolute bottom-0 left-0 w-full flex justify-end bg-yellow-500 transition-all duration-100 ease-out" 
                :style="{ height: `${fuelPercentage}%` }" />
            <div
            class="absolute right-0 flex flex-col items-end p-1 h-fit transition-all"
            :style="{
                transform: `translateY(${fuelPercentage > 50 ? '100%' : '0%'})`,
                bottom: `${fuelPercentage <= criticalPercentage ? criticalPercentage : fuelPercentage}%`}">
                <div class="text-lg font-medium text-gray-800 leading-none">
                    {{ (fuel).toFixed(1) }}L
                </div>
                <div class="text-xs text-gray-800 leading-none">
                    {{ Math.round(fuelPercentage) }}%
                </div>
            </div>
            <div class="absolute top-0 h-full w-full flex flex-col">
                <div
                v-for="tab in new Array(tabs).fill(null).map((_, i) => i + 1)" :key="tab"
                class="relative h-full w-full last:invisible">
                    <div
                    class="absolute bottom-0 h-px translate-y-1/2 rounded-r-full"
                    :style="{ width: tab % largeTickFrequency != 0 ? '0.4em' : '0.75em', backgroundColor: tab >= criticalLimit ? '#ff2056' : '#4b5563' }" />
                    <div v-if="tab == criticalLimit" class="absolute bottom-0 w-full h-0.5 translate-y-1/4 border-b border-dotted border-rose-500" />
                </div>
                <div class="absolute bottom-0 bg-rose-500/25 w-full" :style="{ height: `${criticalPercentage}%` }" />
            </div>
        </div>
    </div>
</template>