<template>
  <div class="relative h-80 w-full">
    <Bar
      :data="data"
      :options="options"
    />
  </div>
</template>

<script setup lang="ts">
  import { Bar } from 'vue-chartjs'
  import {
    Chart as ChartJS,
    BarController, BarElement,
    CategoryScale, LinearScale,
    Tooltip,
  } from 'chart.js'
  import type { Chart, TooltipItem, TooltipModel } from 'chart.js'

  ChartJS.register(BarController, BarElement, CategoryScale, LinearScale, Tooltip)

  const { t } = useI18n()

  const props = defineProps<{
    months: string[]
    trips: number[]
    costs: number[]
    tripsDataset?: {
      label?: string
      data?: number[]
      backgroundColor?: string
    }
    costsDataset?: {
      label?: string
      data?: number[]
      backgroundColor?: string
    }
    tooltipLabel?: {
      first?: string
      second?: string
    }
  }>()

  const data = computed(() => ({
    labels: props.months.map(month => month.slice(0, 3)),
    datasets: [
      {
        label: t('title.trips'),
        data: props.trips,
        backgroundColor: '#0F766E',
        borderRadius: 4,
        borderSkipped: false,
        barPercentage: 0.75,
        categoryPercentage: 0.6,
        order: 1,
        ...props.tripsDataset,
      },
      {
        label: t('title.cost'),
        data: props.costs,
        backgroundColor: '#B5D4F4',
        borderRadius: 4,
        borderSkipped: false,
        barPercentage: 0.75,
        categoryPercentage: 0.6,
        order: 2,
        ...props.costsDataset,
      },
    ],
  }))

  const maxY = computed(() => Math.max(...props.trips, ...props.costs) + 10000)

  const options = computed(() => ({
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
            const n = Number(value)

            return n >= 1000
              ? t('content.thousandsSuffix', { value: n / 1000 })
              : String(n)
          },
        },
      },
    },
  }))

  function getExternalTooltip({ chart, tooltip }: {
    chart: Chart<'bar'>
    tooltip: TooltipModel<'bar'>
  }) {
    const el = getTooltip(chart, 'trips-tooltip')

    if (tooltip.opacity === 0) {
      el.style.opacity = tooltip.opacity.toString()

      return
    }

    const tripsPoint = tooltip.dataPoints?.find((d: TooltipItem<'bar'>) => d.datasetIndex === 0)
    const costPoint = tooltip.dataPoints?.find((d: TooltipItem<'bar'>) => d.datasetIndex === 1)

    renderTooltip(el, tooltip, { tripsPoint, costPoint })
  }

  function renderTooltip(
    el: HTMLDivElement,
    tooltip: TooltipModel<'bar'>,
    points: {
      tripsPoint?: TooltipItem<'bar'>
      costPoint?: TooltipItem<'bar'>
    },
  ) {
    const { tripsPoint, costPoint } = points

    el.innerHTML = `
      <div class="bg-primary text-white text-xs font-medium py-1 px-2.5 rounded-md whitespace-nowrap mb-1">
        ${tripsPoint ? `${String(tripsPoint.raw)} ${props.tooltipLabel?.first || t('title.trips')}` : ''}
      </div>
      <div class="bg-white text-primary border border-blue-200 text-xs py-1 px-2.5 rounded-md whitespace-nowrap">
        ${costPoint ? `${Number(costPoint.raw).toLocaleString()} ${props.tooltipLabel?.second || t('title.cost')}` : ''}
      </div>
    `

    el.style.opacity = '1'
    el.style.left = `${tooltip.caretX - el.offsetWidth / 2}px`
    el.style.top = `${tooltip.caretY - el.offsetHeight - 12}px`
  }

  function getTooltip(chart: Chart<'bar'>, id: string): HTMLDivElement {
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
