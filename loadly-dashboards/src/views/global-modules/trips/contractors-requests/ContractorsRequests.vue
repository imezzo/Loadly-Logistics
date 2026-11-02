<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ $t('content.contractorsRequests') }}
    </h1>

    <ContractorsRequestsFilters @search="applyFilters" />

    <BaseTable
      :columns="tableColumns"
      :data="trips"
    >
      <template #cell-referenceNumber="{ row }">
        #{{ row.referenceNumber ?? row.id }}
      </template>

      <template #cell-from="{ row }">
        {{ getTripPickupAddress(row) || '-' }}
      </template>

      <template #cell-to="{ row }">
        {{ getTripDropOffAddress(row) || '-' }}
      </template>

      <template #cell-bookingRequests="{ row }">
        <button
          v-if="row.bookingRequests?.length && !row.contractorId"
          type="button"
          class="btn-primary rounded-xl px-4 py-2 text-sm font-semibold"
          @click="openAssignPopup(row)"
        >
          ({{ row.bookingRequests?.length }}) {{ $t('button.viewAndAssign') }}
        </button>

        <span
          v-else-if="row.contractorId"
          class="flex items-center gap-0.5 text-sm font-medium text-warning-500"
        >
          <IconUser class="size-4" />
          {{ row.contractorName }}
        </span>

        <span
          v-else
          class="text-sm text-content-secondary"
        >
          {{ $t('content.noBookingRequests') }}
        </span>
      </template>

      <template #cell-action="{ row }">
        <RouterLink
          :to="{ name: 'EditTrip', params: { id: row.id } }"
          class="btn-action-success"
          :aria-label="$t('a11y.edit')"
        >
          <IconEdit class="size-4" />
        </RouterLink>
      </template>
    </BaseTable>

    <BasePagination
      v-if="trips.length"
      :pagination="pagination"
      class="mt-6"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
    />
  </div>

  <ContractorsRequestsPopup
    v-if="isAssignPopupOpen && selectedTrip"
    v-model="isAssignPopupOpen"
    :trip-id="selectedTrip.id"
    :booking-requests="selectedTrip.bookingRequests ?? []"
    @assigned="onAssigned"
    @error="assignError = $event"
  />

  <PopupSuccess
    v-if="isAssignSuccess"
    :model-value="isAssignSuccess"
    @update:model-value="dismissAssignSuccess"
  >
    <template #message>
      {{ $t('success.bookingRequestApproved') }}
    </template>
  </PopupSuccess>

  <PopupError
    v-if="assignError"
    :model-value="assignError !== null"
    :message="assignError"
    @update:model-value="assignError = null"
  />
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
  import { getTripDropOffAddress, getTripPickupAddress } from '@/utils/trip'
  import ContractorsRequestsFilters from './components/ContractorsRequestsFilters.vue'
  import ContractorsRequestsPopup from './components/ContractorsRequestsPopup.vue'
  import { useContractorsRequestsList } from './composables/useContractorsRequestsList'

  const { t } = useI18n()

  const {
    applyFilters,
    fetchContractorsRequests,
    nextPage,
    pagination,
    prevPage,
    setPage,
    trips,
    updateItemsPerPage,
  } = useContractorsRequestsList()

  const isAssignPopupOpen = ref(false)
  const selectedTrip = ref<Trip | null>(null)
  const isAssignSuccess = ref(false)
  const assignError = ref<string | null>(null)

  const breadcrumbItems = computed(() => [
    { label: t('sidebar.tripManagement'), to: '/trips' },
    { label: t('content.contractorsRequests') },
  ])

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'referenceNumber', label: t('content.tripId') },
    { key: 'startDate', label: t('table.startEndDate') },
    { key: 'companyName', label: t('table.companyName') },
    { key: 'from', label: t('content.from') },
    { key: 'to', label: t('content.to') },
    { key: 'bookingRequests', label: t('table.contractors') },
    { key: 'action', label: t('table.action') },
  ])

  function openAssignPopup(trip: Trip): void {
    selectedTrip.value = trip
    isAssignPopupOpen.value = true
  }

  function dismissAssignSuccess(open: boolean): void {
    if (!open) isAssignSuccess.value = false
  }

  async function onAssigned(): Promise<void> {
    isAssignSuccess.value = true
    await fetchContractorsRequests()
  }

  let intervalId: ReturnType<typeof setInterval>

  onMounted(() => {
    fetchContractorsRequests()
    intervalId = setInterval(() => fetchContractorsRequests(false), 60_000)
  })

  onUnmounted(() => {
    clearInterval(intervalId)
  })
</script>

<style scoped>

</style>
