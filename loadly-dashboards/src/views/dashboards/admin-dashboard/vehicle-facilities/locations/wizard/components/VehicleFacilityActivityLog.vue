<template>
  <BaseInputSearch
    v-if="visits.length"
    v-model="search"
    :placeholder="$t('form.searchByVehicle')"
    wrapper-class="flex h-11 w-full md:w-1/3 items-center rounded-lg border border-neutral-300 bg-white px-3"
  />

  <section class="mt-4 flex flex-col gap-3">
    <VehicleFacilityActivityLogItem
      v-for="visit in filteredVisits"
      :key="visit.id"
      :visit="visit"
    />

    <p
      v-if="!filteredVisits.length"
      class="py-8 text-center text-sm text-content-secondary"
    >
      {{ $t('content.noData') }}
    </p>
  </section>
</template>

<script setup lang="ts">
  import type { VehicleFacilityVisit } from '@/types/admin-dashboard/response/vehicle-facility'

  const props = defineProps<{
    visits: VehicleFacilityVisit[]
  }>()

  const search = ref('')

  const filteredVisits = computed(() => {
    const query = search.value.trim().toLowerCase().replace(/\s+/g, '')
    if (!query) return props.visits

    return props.visits.filter((visit) => {
      const plate = [visit.vehicle?.plateNo, visit.vehicle?.plateLetters]
        .filter(Boolean)
        .join('')
        .toLowerCase()
        .replace(/\s+/g, '')

      return plate.includes(query)
    })
  })
</script>

<style scoped>

</style>
