<script setup lang="ts">
import { computed } from 'vue';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { BarChart } from 'echarts/charts';
import { GridComponent, TooltipComponent } from 'echarts/components';
import VChart from 'vue-echarts';

use([CanvasRenderer, BarChart, GridComponent, TooltipComponent]);

const props = defineProps<{
  // 🟢 Simple array of durations in milliseconds: [92500, 91200, ...]
  lapTimes: number[];
}>();

/**
 * Utility to format raw milliseconds into a standard racing lap layout (M:SS.CC)
 */
function formatLapTime(ms: number): string {
  if (!ms || isNaN(ms)) return '0:00.00';
  const minutes = Math.floor(ms / 60000);
  const seconds = Math.floor((ms % 60000) / 1000);
  const centiseconds = Math.floor((ms % 1000) / 10);
  
  const paddedSeconds = String(seconds).padStart(2, '0');
  const paddedCenti = String(centiseconds).padStart(2, '0');
  
  return `${minutes}:${paddedSeconds}.${paddedCenti}`;
}

const chartOption = computed(() => {
  // 1. Map array index to clean string labels: ["Lap 1", "Lap 2", ...]
  const xAxisLabels = props.lapTimes.map((_, index) => `Lap ${index + 1}`);

  return {
    grid: {
      top: 15,
      bottom: 15,
      left: 15,
      right: 15,
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' }, // Dark bar highlight behind hovered item
      backgroundColor: '#1e293b',
      borderColor: '#475569',
      textStyle: { color: '#fff' },
      // 🟢 Format the tooltip to display human-readable times
      formatter: (params: any) => {
        const dataIndex = params[0].dataIndex;
        const rawMs = params[0].value;
        return `Lap ${dataIndex + 1}: <strong>${formatLapTime(rawMs)}</strong>`;
      }
    },
    xAxis: {
      type: 'category',
      data: xAxisLabels,
      axisLabel: { color: '#64748b' },
      axisLine: { lineStyle: { color: '#334155' } }
    },
    yAxis: {
      type: 'value',
      // Start axis near minimum lap time to emphasize delta visibility
      scale: true, 
      axisLabel: {
        color: '#64748b',
        // 🟢 Format the Y-axis grid ticks dynamically
        formatter: (value: number) => formatLapTime(value)
      },
      axisLine: { lineStyle: { color: '#334155' } },
      splitLine: {
        lineStyle: { color: '#1e293b', type: 'dashed' }
      }
    },
    series: [
      {
        name: 'Lap Time',
        type: 'bar',
        data: props.lapTimes,
        barWidth: '80%',
        itemStyle: {
          // Elegant gradient from deep violet to emerald/purple highlights
          color: '#818cf8', // Tailwind indigo-400
          borderRadius: [8, 8, 0, 0] // Rounded caps on top of bars
        },
        // Highlight the fastest bar dynamically if desired
        emphasis: {
          itemStyle: { color: '#34d399' } // Emerald accent color on hover
        }
      }
    ]
  };
});
</script>

<template>
  <div class="rounded-xl bg-gray-200 w-full flex-col p-4 backdrop-blur-md">
    <div class="mb-2 flex items-center">
        <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none">
            Lap Times
        </div>
    </div>

    <div class="h-50 w-full">
      <VChart :option="chartOption" :autoresize="true" />
    </div>
  </div>
</template>