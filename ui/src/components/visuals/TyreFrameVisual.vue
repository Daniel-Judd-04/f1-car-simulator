<script setup lang="ts" >
import type { TyreMetric } from '../types/telemetry';
import TyreVisual from './TyreVisual.vue';

const props = defineProps<{
    tyres: TyreMetric[],
    angle: number,
}>();

</script>

<template>
    <div class="bg-gray-200 flex flex-col items-center gap-2 w-fit p-3 pt-2 rounded-3xl">
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none my-1">
            Tyres
        </div>
        <div v-if="tyres && tyres.length == 4" class="relative h-full flex flex-col justify-between">
            <div v-for="axle in [0, 1]" class="flex justify-center items-center">
                <TyreVisual :tyre="tyres[2*axle]" :angle="angle" :is-front="axle == 0" :is-left="(2*axle) % 2 == 0"/>
                <div class="w-20 h-1 bg-gray-400" />
                <TyreVisual :tyre="tyres[(2*axle)+1]" :angle="angle" :is-front="axle == 0" :is-left="((2*axle)+1) % 2 == 0"/>
            </div>
            <div class="absolute top-15 left-1/2 -translate-x-1/2 w-1 h-[calc(100%-7.5em)] bg-gray-400" />
        </div>
        <div v-else class="w-50 h-full flex items-center text-center justify-center font-bold text-lg text-red-500/75">
            <div>No Tyre Data!</div>
        </div>
    </div>
</template>