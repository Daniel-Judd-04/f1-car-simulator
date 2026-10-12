<script setup lang="ts" >

const props = defineProps<{
    value: number,
    coolThreshold: number,
    idealThreshold: number,
    name: string
}>();

const getColorClass = () => {
  if (props.value < props.coolThreshold) return 'text-blue-950 bg-blue-500'       // Cold
  if (props.value >= props.coolThreshold && props.value <= props.idealThreshold) return 'text-emerald-950 bg-emerald-500' // Ideal Range
  return 'text-rose-950 bg-rose-500'       // Overheating
}

</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 w-full aspect-square p-3 rounded-3xl items-center">
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none my-0.5">
            {{ name }}
        </div>
        <div :class="`w-full aspect-square flex items-center justify-center rounded-2xl transition-colors ${getColorClass()}`">
            <div class="w-full text-center font-medium text-lg">
                {{ Math.round(value) }}&#xB0;C
            </div>
        </div>
    </div>
    
</template>