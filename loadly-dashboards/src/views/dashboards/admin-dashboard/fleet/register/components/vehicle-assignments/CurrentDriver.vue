<template>
  <section>
    <h2 class="mb-2 text-sm font-semibold text-neutral-10">
      {{ $t('content.currentDriver') }}
    </h2>

    <div
      v-if="driver"
      class="mb-8 flex items-center gap-4 rounded-lg border border-neutral-300 bg-white p-4"
    >
      <div
        class="
          flex size-12 shrink-0 items-center justify-center overflow-hidden rounded-full
          bg-neutral-200
        "
      >
        <span class="text-lg font-semibold text-neutral-600">
          {{ driverInitial }}
        </span>
      </div>

      <div class="min-w-0 flex-1">
        <p class="text-xs font-medium text-neutral-600">
          {{ $t('content.currentDriver') }}
        </p>
        <p class="font-medium text-neutral-10">
          {{ driver.fullName || driver.displayName }}
        </p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
  export interface CurrentDriverItem {
    id: number
    fullName?: string
    displayName?: string
  }

  const props = defineProps<{
    driver: CurrentDriverItem | null | undefined
  }>()

  const driverInitial = computed(() => {
    const currentDriver = props.driver
    if (!currentDriver?.fullName && !currentDriver?.displayName) return '—'
    const name = (currentDriver.fullName ?? currentDriver.displayName ?? '').trim()
    const parts = name.split(/\s+/).filter(Boolean)
    const first = parts[0]?.[0]
    const second = parts[1]?.[0]
    if (first && second) return (first + second).toUpperCase()

    return name[0]?.toUpperCase() ?? '—'
  })
</script>

<style scoped>

</style>
