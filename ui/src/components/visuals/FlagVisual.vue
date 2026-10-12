<script setup lang="ts">
import type { RaceFlag } from '../types/telemetry';


const props = defineProps<{
    flag: RaceFlag
}>();

const boxBG = (box: number) => {
    switch (props.flag) {
        case 'Chequered':
            return box % 2 == 0 ? '#FFF' : '#000'
        case 'Green':
            return '#0F0'
        case 'Yellow':
            return '#FF0'
        case 'Red':
            return '#F00'
        case 'White':
            return '#FFF'
        case 'Blue':
            return '#00F'
        case 'Black':
        case 'Black_With_Orange_Disc':
        case 'Black_And_White':
            return '#000'
        case 'Yellow_And_Red_Striped':
            return ((box-1) % 5 == 0 || (box-3) % 5 == 0 || box % 5 == 0) ? '#F00' : '#FF0'
        default:
            return box % 2 == 0 ? '#F0F' : '#000'
    }
}

</script>

<template>
    <div class="bg-gray-200 flex flex-col gap-1 h-full aspect-square p-3 rounded-3xl items-center">
        <div class="relative w-full aspect-square rounded-2xl bg-gray-400 overflow-hidden flex flex-wrap">
            <div v-for="box in new Array(25).fill(null).map((_, i) => i + 1)" :key="box"
                class="aspect-square w-1/5" :style="{ backgroundColor: `${boxBG(box)}`}" />
            <div v-if="flag == 'Black_With_Orange_Disc'"
            class="absolute aspect-square w-1/2 rounded-full bg-orange-600 top-1/2 left-1/2 -translate-1/2" />
            <div v-if="flag == 'Black_And_White'"
            class="absolute w-full h-[200%] bg-white left-1/2 -translate-y-1/4 origin-left rotate-45" />
        </div>
    </div>
</template>