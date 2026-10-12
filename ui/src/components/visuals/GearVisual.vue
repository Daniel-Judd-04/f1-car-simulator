<script setup lang="ts">

defineProps<{
    gear: number
}>();

</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full aspect-square p-3 rounded-3xl items-center">
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none my-0.5">
            Gear
        </div>
        <div class="w-full aspect-square rounded-2xl bg-gray-400 p-2 flex justify-center items-center">
            <div class="text-5xl font-bold leading-none">
                {{ gear == 0 ? 'N' : gear == -1 ? 'R' : gear }}
            </div>
        </div>
    </div>
</template>