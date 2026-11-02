<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ $t('content.tripsRequests') }}
    </h1>

    <TripRequestsFilters @search="applyFilters" />

    <div
      class="
        grid grid-cols-1 gap-4
        xl:grid-cols-2
      "
    >
      <TripRequestCard
        v-for="trip in trips"
        :key="trip.id"
        :trip="trip"
        :is-new="isNewTrip(trip.id)"
        @push-to-contractors="openPushToContractorsPopup(trip)"
      />

      <p
        v-if="!trips.length"
        class="col-span-full py-12 text-center text-sm text-content-secondary"
      >
        {{ $t('content.noData') }}
      </p>
    </div>

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

  <PushToContractorsPopup
    v-if="isPushPopupOpen"
    v-model="isPushPopupOpen"
    :trip="selectedTrip"
    @success="onPushSuccess"
    @error="onPushError"
  />

  <PopupSuccess
    v-if="isPushSuccess"
    :model-value="isPushSuccess"
    @update:model-value="dismissPushSuccess"
  >
    <template #message>
      {{ $t('success.tripPushedToContractors') }}
    </template>
  </PopupSuccess>

  <PopupError
    v-if="pushError"
    :model-value="pushError !== null"
    :message="pushError"
    @update:model-value="pushError = null"
  />
</template>

<script setup lang="ts">
  import TripRequestsFilters from './components/TripRequestsFilters.vue'
  import { useTripRequestsList } from './composables/useTripRequestsList'

  const TripRequestCard = defineAsyncComponent(
    () => import('./components/trip-request-card/TripRequestCard.vue'),
  )
  const PushToContractorsPopup = defineAsyncComponent(
    () => import('./components/trip-request-card/push-to-contractor-popup/PushToContractorsPopup.vue'),
  )

  const { t } = useI18n()

  const {
    applyFilters,
    dismissPushSuccess,
    fetchTripRequests,
    isNewTrip,
    isPushPopupOpen,
    isPushSuccess,
    nextPage,
    onPushError,
    onPushSuccess,
    openPushToContractorsPopup,
    pagination,
    prevPage,
    pushError,
    selectedTrip,
    setPage,
    trips,
    updateItemsPerPage,
  } = useTripRequestsList()

  const breadcrumbItems = computed(() => [
    { label: t('sidebar.tripManagement'), to: '/trips' },
    { label: t('content.tripsRequests') },
  ])

  onMounted(fetchTripRequests)
</script>

<style scoped>

</style>
