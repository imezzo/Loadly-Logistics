<template>
  <article class="rounded-2xl border border-neutral-500 bg-white p-6">
    <header>
      <h2 class="text-base font-bold text-neutral-10">
        {{ title }}
      </h2>
      <p class="mt-1 text-sm text-content-secondary">
        {{ subtitle }}
      </p>
    </header>

    <div class="mt-2">
      <slot />
    </div>

    <footer
      v-if="legendItems?.length"
      class="mt-2 flex flex-wrap gap-x-6 gap-y-2 border-t border-neutral-200 pt-4"
    >
      <span
        v-for="item in legendItems"
        :key="item.label"
        class="flex items-center gap-2 text-xs text-content-secondary"
      >
        <span
          class="size-2 shrink-0 rounded-full"
          :class="getMetricProgressColorClass(item.progress)"
        />
        {{ item.label }}
      </span>
    </footer>
  </article>
</template>

<script setup lang="ts">
  import {
    getMetricProgressColorClass,
    type DashboardMetricLegendItem,
  } from '../composables/useDashboardOverview'

  defineProps<{
    title: string
    subtitle: string
    legendItems?: DashboardMetricLegendItem[]
  }>()
</script>

<style scoped>

</style>
