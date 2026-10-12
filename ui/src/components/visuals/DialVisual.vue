<script setup lang="ts">
import { computed } from 'vue';


const props = defineProps<{
    value: number,
    max: number,
    name: string
}>();

const angle = computed(() => {
    const pct = (props.value / props.max) * 270
    return Math.max(0, Math.min(270, pct)) + 45;
})
</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full aspect-square p-3 rounded-3xl items-center">
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none my-0.5">
            {{ name }}
        </div>
        <div class="relative w-full aspect-square rounded-2xl bg-gray-400 p-2">
            <div class="relative w-full h-full -rotate-45 rounded-full overflow-hidden">
                <div class="relative border-4 rounded-full w-full h-full" :style="{ rotate: `${angle-45}deg`}">
                    <div class="absolute left-0.5 top-1/2 -translate-y-1/2 h-0.5 w-[calc(50%-0.25em)] bg-rose-500" />
                    <div class="absolute h-1.5 w-1.5 top-1/2 -translate-1/2 left-1/2 bg-black rounded-full"/>
                </div>
                <div class="absolute w-full aspect-square top-1/2 right-1/2 border-2 bg-gray-400" />
                
            </div>
            <div class="absolute bottom-0.5 leading-none font-light left-1/2 -translate-x-1/2 text-xs">
                {{ value.toFixed(0) }}
            </div>
        </div>
    </div>
</template>