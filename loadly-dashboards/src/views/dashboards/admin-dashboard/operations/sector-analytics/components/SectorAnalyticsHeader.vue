<template>
  <header
    class="
      mb-6 flex flex-wrap items-center justify-center gap-4
      4xl:mb-10
    "
  >
    <IconLogo
      with-name
      class="
        me-15 h-14 w-auto
        4xl:h-45 4xl:[&>svg]:size-35
      "
    />

    <div class="text-end">
      <div
        class="
          flex flex-wrap items-center justify-end gap-2
          4xl:gap-4
        "
      >
        <BaseFlipNumber
          :value="totalTrips"
          class="
            text-4xl font-bold text-neutral-10
            4xl:text-5xl
          "
        />

        <span class="text-4xl font-bold text-neutral-10 capitalize">
          {{ $t('content.tripsUnit') }}
        </span>

        <div
          class="
            flex size-8 shrink-0 items-center justify-center rounded-full
            4xl:size-17.5
          "
          :class="isUp ? 'bg-green-100' : 'bg-error-100'"
        >
          <IconArrow
            class="
              size-4
              4xl:size-10
            "
            :class="isUp ? '-rotate-60 fill-green-700!' : 'rotate-120 fill-error-600!'"
          />
        </div>
      </div>

      <p
        class="
          mt-1 text-base font-bold capitalize
          4xl:text-4xl
        "
        :class="isUp ? 'text-success-500' : 'text-error-600'"
      >
        {{ trendPrefix }} {{ $t('content.tripsVsLastDay') }}
      </p>
    </div>
  </header>
</template>

<script setup lang="ts">
  const props = defineProps<{
    delta: number
    direction: string
    totalTrips: number
  }>()

  const isUp = computed(() => props.direction === 'up' || props.delta > 0)

  const trendPrefix = computed(() => {
    const arrow = isUp.value ? '↑' : '↓'
    const delta = props.delta.toLocaleString()
    const signedDelta = props.delta > 0 ? `+${delta}` : delta

    return `${arrow} ${signedDelta}`
  })
</script>

<style scoped>

</style>
