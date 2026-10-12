<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
    value: number
    max: number,
    segments: number
    name: string,
    color: string
}>();

const currentSegments = computed(() => {
    const pct = (props.value / props.max) * props.segments
    return Math.max(0, Math.min(props.segments, Math.round(pct)))
});

</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full h-full p-3 rounded-3xl items-center">
        
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none mb-1 mt-0.5">
            {{ name }}
        </div>

        <div class="relative bg-gray-400 rounded-xl h-full w-full flex flex-col-reverse gap-0.5 p-2">
            <div v-for="seg in new Array(segments).fill(null).map((_, i) => i + 1)" :key="seg"
                class="w-full h-full first:rounded-b-md last:rounded-t-md rounded-xs transition-colors" :style="{ backgroundColor: seg <= currentSegments ? color : 'black' }" />
        </div>
    </div>
</template>