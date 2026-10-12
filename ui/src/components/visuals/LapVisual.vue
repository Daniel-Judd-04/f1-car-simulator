<script setup lang="ts">

defineProps<{
    lap: number;
    laps: number;
}>();

</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full p-3 rounded-3xl items-center">
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none my-0.5">
            Lap
        </div>
        <div class="w-full rounded-2xl bg-gray-400 p-2 flex justify-center items-center">
            <div class="text-4xl font-light leading-none">
                {{ lap }} of {{ laps }}
            </div>
        </div>
    </div>
</template>