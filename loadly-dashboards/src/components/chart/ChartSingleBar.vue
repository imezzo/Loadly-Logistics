<template>
  <div class="relative h-80 w-full">
    <Bar
      :data="chartData"
      :options="chartOptions"
    />
  </div>
</template>

<script setup lang="ts">
  import { Bar } from 'vue-chartjs'
  import {
    BarController,
    BarElement,
    CategoryScale,
    Chart as ChartJS,
    LinearScale,
    Tooltip,
  } from 'chart.js'
  import type { Chart, TooltipModel } from 'chart.js'

  ChartJS.register(BarController, BarElement, CategoryScale, LinearScale, Tooltip)

  const { t } = useI18n()

  const props = defineProps<{
    labels: string[]
    values: number[]
    tooltipSuffix?: string
  }>()

  const chartData = computed(() => ({
    labels: props.labels.map((label) => label.slice(0, 3)),
    datasets: [
      {
        data: props.values,
        backgroundColor: '#167CF2',
        borderRadius: 4,
        borderSkipped: false,
        barPercentage: 0.75,
        categoryPercentage: 0.6,
      },
    ],
  }))

  const maxY = computed(() => {
    const peak = Math.max(...props.values, 0)

    return peak > 0 ? peak * 1.2 : 100
  })

  const chartOptions = computed(() => ({
    responsive: true,
    maintainAspectRatio: false,
    interaction: { mode: 'index' as const, intersect: false },
    plugins: {
      legend: { display: false },
      tooltip: {
        enabled: false,
        external: getExternalTooltip,
      },
    },
    scales: {
      x: {
        grid: { display: false },
        border: { display: false },
      },
      y: {
        border: { display: false, dash: [4, 4] },
        grid: { color: '#e5e7eb', lineWidth: 0.8 },
        max: maxY.value,
        ticks: {
          callback: (value: number | string) => {
            const amount = Number(value)

            return amount >= 1000
              ? t('content.thousandsSuffix', { value: amount / 1000 })
              : String(amount)
          },
        },
      },
    },
  }))

  function getExternalTooltip({
    chart,
    tooltip,
  }: {
    chart: Chart<'bar'>
    tooltip: TooltipModel<'bar'>
  }): void {
    const el = getTooltipElement(chart)

    if (tooltip.opacity === 0) {
      el.style.opacity = tooltip.opacity.toString()

      return
    }

    const point = tooltip.dataPoints?.[0]
    const suffix = props.tooltipSuffix ?? t('form.egp')

    el.innerHTML = `
      <div class="rounded-md bg-primary px-2.5 py-1 text-xs font-medium whitespace-nowrap text-white">
        ${point ? `${Number(point.raw).toLocaleString()} ${suffix}` : ''}
      </div>
    `

    el.style.opacity = '1'
    el.style.left = `${tooltip.caretX - el.offsetWidth / 2}px`
    el.style.top = `${tooltip.caretY - el.offsetHeight - 12}px`
  }

  function getTooltipElement(chart: Chart<'bar'>): HTMLDivElement {
    const id = 'single-bar-tooltip'
    let el = document.getElementById(id) as HTMLDivElement | null

    if (el) return el

    el = document.createElement('div')
    el.id = id
    el.style.cssText = 'position:absolute;pointer-events:none;transition:opacity .15s;'
    ;(chart.canvas.parentNode as HTMLElement).style.position = 'relative'
    ;(chart.canvas.parentNode as HTMLElement).appendChild(el)

    return el
  }
</script>

<style scoped>

</style>
