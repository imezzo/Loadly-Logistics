<template>
  <section class="flex w-full min-w-0 flex-col gap-4">
    <div class="min-h-0 flex-1 overflow-x-auto rounded-xl border border-neutral-200 bg-white">
      <table class="w-full min-w-max border-collapse text-start">
        <thead>
          <tr class="border-b border-neutral-500 bg-neutral-400">
            <th
              rowspan="2"
              class="border-e border-neutral-200 p-3 text-xs text-neutral-800"
            >
              {{ $t('table.destinationFromTo') }}
            </th>

            <th
              rowspan="2"
              class="border-e border-neutral-200 p-3 text-xs text-neutral-800"
            >
              {{ $t('table.kilometer') }}
            </th>

            <th
              rowspan="2"
              class="border-e border-neutral-200 p-3 text-xs text-neutral-800"
            >
              {{ $t('table.workingDays') }}
            </th>

            <th
              v-for="vehicleType in vehicleTypes"
              :key="vehicleType.vehicleTypeId"
              colspan="3"
              class="border-e border-neutral-200 p-3 text-xs text-neutral-800"
            >
              {{ getLocalizedValue(vehicleType.vehicleTypeNameEn, vehicleType.vehicleTypeNameAr) }}
            </th>
          </tr>

          <tr class="border-b border-neutral-200 bg-neutral-400">
            <template
              v-for="vehicleType in vehicleTypes"
              :key="`${vehicleType.vehicleTypeId}-sub`"
            >
              <th class="border-e border-neutral-200 p-3 text-xs text-neutral-800">
                {{ $t('table.vehicleRent') }}
              </th>

              <th class="border-e border-neutral-200 p-3 text-xs text-neutral-800">
                {{ $t('table.gas') }}
              </th>

              <th class="border-e border-neutral-200 p-3 text-xs text-neutral-800">
                {{ $t('table.totalTripCost') }}
              </th>
            </template>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="(row, index) in paginatedDestinations"
            :key="index"
            class="
              border-b border-neutral-100 text-sm text-content-secondary transition-colors
              hover:bg-neutral-100
            "
          >
            <td class="max-w-56 p-3 break-all">
              {{ row.name }}
            </td>

            <td class="max-w-56 p-3 break-all">
              {{ row.kms }} {{ $t('table.km') }}
            </td>

            <td class="max-w-56 p-3 break-all">
              {{ row.numberOfDays }}
            </td>

            <template
              v-for="(cell, vi) in row.relatedDestinations"
              :key="`${index}-vt-${vi}`"
            >
              <td class="max-w-56 p-3 break-all">
                {{ cell.dailyValue }}
              </td>

              <td class="max-w-56 p-3 break-all">
                {{ cell.fuelPrice }}
              </td>

              <td class="max-w-56 p-3 break-all">
                {{ cell.totalTripCost }}
              </td>
            </template>
          </tr>
        </tbody>
      </table>
    </div>

    <BasePagination
      :pagination="pagination"
      :per-page-options="[5, 10, 20, 50, 100]"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
    />
  </section>
</template>

<script setup lang="ts">
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import type { ContractDestinationsCalculatedVehicleType } from '@/types/admin-dashboard/contract-destinations-calculate'
  import type { Pagination } from '@/types/response/pagination'
  import { getLocalizedValue } from '@/utils/i18n'
  import { formatValueWithCurrency } from '@/utils/currency.ts'
  import { usePagination } from '@/composables/usePagination.ts'

  interface CalculatedDestinations {
    name: string
    kms?: number
    numberOfDays?: number
    relatedDestinations: {
      dailyValue?: string
      fuelPrice?: string
      totalTripCost?: string
    }[]
  }

  const props = defineProps<{
    vehicleTypes: ContractDestinationsCalculatedVehicleType[]
    destinations: ContractWizardDestination[]
  }>()

  const calculatedDestinations = computed<CalculatedDestinations[]>(() => {
    const list = props.vehicleTypes
    if (!list?.length || !props.destinations.length) return []

    return props.destinations.map((destination, destinationIndex) => {
      const relatedDestinations = Array.from({ length: list.length }, (_, vehicleTypeIndex) => {
        const vehicleTypeDestination = list[vehicleTypeIndex]?.destinations?.[destinationIndex]

        return {
          dailyValue: formatValueWithCurrency(vehicleTypeDestination?.dailyValue),
          fuelPrice: formatValueWithCurrency(vehicleTypeDestination?.fuelPrice),
          totalTripCost: formatValueWithCurrency(vehicleTypeDestination?.totalTripCost),
        }
      })

      return {
        name: getLocalizedValue(destination.nameEn, destination.nameAr),
        kms: destination.expectedKms,
        numberOfDays: destination.numberOfDays,
        relatedDestinations,
      }
    })
  })

  const pagination = computed((): Pagination => {
    const items = calculatedDestinations.value.length
    const pages = Math.max(1, Math.ceil(items / itemsPerPage.value) || 1)

    return {
      page: currentPage.value,
      size: itemsPerPage.value,
      pages,
      items,
    }
  })

  const { setPage, nextPage, prevPage, updateItemsPerPage, currentPage, itemsPerPage } = usePagination(pagination)

  const paginatedDestinations = computed(() => {
    const startedPageIndex = (currentPage.value - 1) * itemsPerPage.value

    return calculatedDestinations.value.slice(startedPageIndex, startedPageIndex + itemsPerPage.value)
  })
</script>

<style scoped>

</style>
