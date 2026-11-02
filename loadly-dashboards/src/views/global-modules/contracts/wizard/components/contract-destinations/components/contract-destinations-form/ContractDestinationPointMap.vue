<template>
  <div class="flex flex-col gap-2">
    <a
      v-if="hasValidCoords && googleMapsUrl"
      class="btn-link inline-flex items-center gap-1.5 rounded-lg text-sm font-medium"
      :href="googleMapsUrl"
      target="_blank"
      rel="noopener noreferrer"
    >
      <IconLocation
        filled
        class="size-4 shrink-0"
      />
      {{ $t('button.openInGoogleMaps') }}
    </a>
  </div>
</template>

<script setup lang="ts">
  const props = defineProps<{
    latitude?: string | null
    longitude?: string | null
    mapLabel: string
  }>()

  const latitude = computed(() => Number.parseFloat(String(props.latitude ?? '')))

  const longitude = computed(() => Number.parseFloat(String(props.longitude ?? '')))

  const hasValidCoords = computed(() => {
    return Number.isFinite(latitude.value) && Number.isFinite(longitude.value)
  })

  const googleMapsUrl = computed(() => {
    if (!hasValidCoords.value) return ''

    return `https://www.google.com/maps?q=${latitude.value},${longitude.value}`
  })
</script>

<style scoped>

</style>
