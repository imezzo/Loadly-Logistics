<template>
  <nav :aria-label="$t('a11y.breadcrumb')">
    <ul class="flex items-center gap-1">
      <li
        v-for="({ label, to }, index) in items"
        :key="index"
        class="flex items-center gap-2 text-content-secondary"
      >
        <BaseLink
          v-if="to && index < itemsCount"
          :to
          class="text-sm transition-colors"
          :class="{'text-primary': index < itemsCount}"
        >
          {{ label }}
        </BaseLink>

        <span
          v-else
          class="text-sm font-medium"
          v-text="label"
        />

        <span
          v-if="index < itemsCount"
          aria-hidden="true"
          v-text="separator ?? '/'"
        />
      </li>
    </ul>
  </nav>
</template>

<script setup lang="ts">
  import type { BreadcrumbItem } from '@/types/ui'

  const props = defineProps<{
    items: BreadcrumbItem[]
    separator?: string
  }>()

  const itemsCount = computed(() => props.items.length - 1)
</script>

<style scoped>

</style>
