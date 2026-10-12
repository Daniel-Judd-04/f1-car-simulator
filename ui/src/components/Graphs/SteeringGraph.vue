<script setup lang="ts">
import { computed } from 'vue';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { LineChart } from 'echarts/charts';
import { GridComponent, TooltipComponent } from 'echarts/components';
import VChart from 'vue-echarts';
import type { TelemetryPackage } from '../../types/telemetry';

use([CanvasRenderer, LineChart, GridComponent, TooltipComponent]);

const props = defineProps<{
  history: TelemetryPackage[];
}>();

const chartOption = computed(() => {
  const seriesData = props.history
    .map(point => {
      const timestamp = new Date(point.createdAt).getTime();
      return [timestamp, point.steeringAngle];
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
      formatter: 'Steering: {c}',
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
      min: -80,
      max: 80,
      interval: 40,
      splitLine: {
        lineStyle: { color: '#334155', type: 'dashed' }
      },
      axisLine: {
        onZero: true,
        lineStyle: { color: '#64748b' }
      }
    },
    series: [
      {
        name: 'Steering Angle',
        type: 'line',
        data: seriesData,
        showSymbol: false,
        smooth: true,      // Makes the steering trace look organic and fluid
        lineStyle: {
          color: '#ff2056', // Tailwind sky-400 (Clean racing telemetry blue)
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
                Steering Trace
            </div>
            <span class="text-xs font-medium text-rose-500 leading-none">
                {{ history[history.length - 1]?.steeringAngle.toFixed() ?? '0.00' }}&#xB0;
            </span>
        </div>

        <div class="flex">
            <div class="h-50 w-full">
                <VChart :option="chartOption" :autoresize="true" />
            </div>
            <div class="flex flex-col justify-between py-2 text-xs text-gray-500">
                <span>Right</span>
                <span>Center</span>
                <span>Left</span>
            </div>
        </div>
    </div>
</template>