<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
    lapTime: number;
    name: string;
}>();

const currentLapTime = computed(() => {

    if (isNaN(props.lapTime) || props.lapTime <= 0) {
        return "0:00.00";
    }

    const minutes = Math.floor(props.lapTime / 60000);
    const seconds = Math.floor((props.lapTime % 60000) / 1000);
    
    const centiseconds = Math.floor((props.lapTime % 1000) / 10);

    const formattedSeconds = String(seconds).padStart(2, '0');
    const formattedCentiseconds = String(centiseconds).padStart(2, '0');

    return `${minutes}:${formattedSeconds}.${formattedCentiseconds}`;
});

</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full p-3 rounded-3xl items-center">
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none my-0.5">
            {{ name }}
        </div>
        <div class="w-full rounded-2xl bg-gray-400 p-2 flex justify-center items-center">
            <div class="text-4xl font-thin leading-none">
                {{ currentLapTime }}
            </div>
        </div>
    </div>
</template>