<template>
  <svg
    viewBox="0 0 100 32"
    class="h-12 w-full"
    preserveAspectRatio="none"
    aria-hidden="true"
  >
    <path
      :d="path"
      fill="none"
      :stroke="strokeColor"
      stroke-width="2"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>
</template>

<script setup lang="ts">
  const props = defineProps<{
    points: number[]
    variant: 'primary' | 'success' | 'warning'
  }>()

  const strokeColor = computed(() => {
    switch (props.variant) {
      case 'success':
        return '#22c55e'
      case 'warning':
        return '#fbbf24'
      case 'primary':
      default:
        return '#0F766E'
    }
  })

  const path = computed(() => buildSparklinePath(props.points))

  function buildSparklinePath(
    points: number[],
    width = 100,
    height = 32,
    padding = 4,
  ): string {
    if (points.length < 2) return ''

    const min = Math.min(...points)
    const max = Math.max(...points)
    const range = max - min || 1
    const stepX = width / (points.length - 1)

    return points
      .map((point, index) => {
        const x = index * stepX
        const y = height - padding - ((point - min) / range) * (height - padding * 2)

        return `${index === 0 ? 'M' : 'L'} ${x} ${y}`
      })
      .join(' ')
  }
</script>

<style scoped>

</style>
