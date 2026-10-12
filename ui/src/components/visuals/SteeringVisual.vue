<script setup lang="ts">

defineProps<{
    angle: number
}>();
</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full aspect-square p-3 rounded-3xl items-center">
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none my-0.5">
            Wheel
        </div>
        <div class="w-full aspect-square rounded-2xl bg-gray-400 p-2">
            <div class="relative border-4 rounded-full w-full h-full" :style="{ rotate: `${angle+90}deg`}">
                <div class="absolute top-1/2 -translate-y-1/2 -left-2 h-0.5 w-1/4 bg-rose-500"></div>
                <div class="absolute left-1/2 -translate-x-1/2 h-full w-0.5 bg-black"></div>
                <div class="absolute top-1/2 -translate-y-1/2 left-1/2 h-0.5 w-1/2 bg-black"></div>
            </div>
        </div>
    </div>
</template>