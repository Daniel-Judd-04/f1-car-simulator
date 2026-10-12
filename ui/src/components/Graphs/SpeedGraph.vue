<script setup lang="ts">
import { computed } from 'vue';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { LineChart } from 'echarts/charts';
import { GridComponent, TooltipComponent } from 'echarts/components';
import VChart from 'vue-echarts';
import type { TelemetryPackage, ParametersPackage } from '../../types/telemetry';

use([CanvasRenderer, LineChart, GridComponent, TooltipComponent]);

const props = defineProps<{
  history: TelemetryPackage[];
  parameters: ParametersPackage | null;
}>();

const chartOption = computed(() => {
  const seriesData = props.history
    .map(point => {
      const timestamp = new Date(point.createdAt).getTime();
      return [timestamp, point.speed];
    })
    .filter(([time]) => !isNaN(time));

  return {
    grid: {
      top: 15,
      bottom: 15,
      left: 15,
      right: 15,
    },
    tooltip: {
      trigger: 'axis',
      formatter: 'Speed: {c}km/hr',
      backgroundColor: '#1e293b',
      borderColor: '#475569',
      textStyle: { color: '#fff' }
    },
    xAxis: {
      type: 'time',
      boundaryGap: false,
      axisLabel: { show: false }, // Hides cluttered timestamps along the bottom
      axisLine: { lineStyle: { color: '#475569' } }
    },
    yAxis: {
      type: 'value',
      max: props.parameters?.maxSpeed || null,
      interval: Math.floor((props.parameters?.maxSpeed || 200)/4),
      splitLine: {
        lineStyle: { color: '#334155', type: 'dashed' }
      },
    },
    series: [
      {
        name: 'Speed',
        type: 'line',
        data: seriesData,
        showSymbol: false,
        smooth: true,
        lineStyle: {
          color: '#ff2056',
          width: 2
        },
        areaStyle: {
          color: '#ff205630'
        }
      }
    ]
  };
});
</script>

<template>
    <div class="flex flex-col w-full rounded-3xl bg-gray-200 p-4">
        <div class="mb-2 flex items-center gap-4">
            <div class="text-xs uppercase tracking-wider font-bold text-gray-500 leading-none">
                Speed Trace
            </div>
            <span class="text-xs font-medium text-rose-500 leading-none">
                {{ history[history.length - 1]?.speed.toFixed(0) ?? '0.00' }}km/hr
            </span>
        </div>

        <div class="h-50 w-full">
            <VChart :option="chartOption" :autoresize="true" />
        </div>
    </div>
</template>