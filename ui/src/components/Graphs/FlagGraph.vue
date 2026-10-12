<script setup lang="ts">
import { computed } from 'vue';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { LineChart } from 'echarts/charts';
import { GridComponent, TooltipComponent, VisualMapComponent } from 'echarts/components';
import VChart from 'vue-echarts';
import type { TelemetryPackage } from '../../types/telemetry';

use([CanvasRenderer, LineChart, GridComponent, TooltipComponent, VisualMapComponent]);

const props = defineProps<{
  history: TelemetryPackage[];
}>();

// 🟢 Map the exact order of your C# enum to the Y-axis text labels
const FLAG_NAMES = [
  'Chequered',              // 0
  'Green',                  // 1
  'Yellow',                 // 2
  'Red',                    // 3
  'Black_With_Orange_Disc', // 4
];

const chartOption = computed(() => {
  const seriesData = props.history
    .map(point => [
      new Date(point.createdAt).getTime(), 
      point.flag // Maps perfectly to the indices of FLAG_NAMES
    ])
    // .filter(([time]) => !isNaN(time));

  return {
    grid: {
      top: 15,
      bottom: 15,
      left: 15,
      right: 15,
    },
    tooltip: {
      trigger: 'axis',
      backgroundColor: '#1e293b',
      borderColor: '#475569',
      textStyle: { color: '#fff' },
      formatter: (params: any) => {
        const flagIndex = params[0].value[1];
        return `Track Status: <strong>${FLAG_NAMES[flagIndex]}</strong>`;
      }
    },
    visualMap: {
      show: false, // Hidden utility; works behind the scenes
      dimension: 1, // Look at the Y-value (index 1 of the coordinate pair) to determine color
      pieces: [
        { value: 0, color: '#ffffff' }, // Chequered -> White line
        { value: 1, color: '#22c55e' }, // Green -> Green line
        { value: 2, color: '#eab308' }, // Yellow -> Yellow line
        { value: 3, color: '#ef4444' }, // Red -> Red line
        { value: 4, color: '#64748b' }, // White -> Off-white line
      ],
      outOfRange: { color: '#64748b' } // Fallback color for complex multi-color flags
    },
    xAxis: {
      type: 'time',
      boundaryGap: false,
      axisLabel: { show: false},
      axisLine: { lineStyle: { color: '#475569' } },
      splitLine: { show: false }
    },
    yAxis: {
      type: 'category',
      data: FLAG_NAMES,
      axisLabel: {
        color: '#94a3b8',
        fontSize: 10,
        fontFamily: 'monospace'
      },
      axisLine: { lineStyle: { color: '#334155' } },
      splitLine: {
        show: true,
        lineStyle: { color: '#1e293b', type: 'solid' }
      }
    },
    series: [
      {
        name: 'Track Status',
        type: 'line',
        step: 'end',
        data: seriesData,
        showSymbol: false,
        lineStyle: { width: 2 },
      }
    ]
  };
});
</script>

<template>
  <div class="w-full rounded-3xl bg-gray-200 p-4">
    <div class="mb-2 flex items-center justify-between">
            <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none">
                Flag Trace
            </div>
            <span class="text-xs font-medium text-rose-500 leading-none">
                {{ history[history.length - 1]?.flag ?? 'Unknown' }} Flag
            </span>
        </div>

    <div class="h-50 w-full">
      <VChart :option="chartOption" :autoresize="true" />
    </div>
  </div>
</template>