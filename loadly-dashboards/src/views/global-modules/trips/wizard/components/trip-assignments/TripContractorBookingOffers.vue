<template>
  <div class="rounded-lg border border-neutral-200 p-4">
    <header class="mb-3 flex flex-wrap items-baseline gap-2">
      <h3 class="text-sm font-medium text-neutral-10">
        {{ $t('content.contractorsOffers') }}
      </h3>

      <span class="text-sm font-semibold text-primary">
        {{ $t('content.requestsCount', { count: bookingRequests.length }) }}
      </span>
    </header>

    <BaseInputSearch
      v-model="searchKeyword"
      wrapper-class="
        mb-4 flex h-11 w-full items-center rounded-lg border border-neutral-300 bg-white px-3
      "
      :placeholder="$t('content.searchForContractors')"
      :disabled="disabled"
    />

    <div
      class="
        grid grid-cols-1 gap-3
        md:grid-cols-2
      "
    >
      <div
        v-for="request in filteredRequests"
        :key="request.id"
        class="
          flex items-center justify-between gap-3 rounded-xl border border-neutral-200 bg-white p-4
        "
      >
        <div class="min-w-0">
          <p class="truncate text-sm font-bold text-neutral-800">
            {{ request.contractorName }}
          </p>

          <p
            v-if="request.totalTrips != null"
            class="mt-0.5 text-xs font-medium tracking-wide text-content-secondary uppercase"
          >
            {{ $t('content.tripsWithUs', { count: request.totalTrips }) }}
          </p>

          <p
            v-if="phoneOf(request)"
            class="mt-1 text-sm text-content-secondary"
          >
            {{ phoneOf(request) }}
          </p>
        </div>

        <button
          type="button"
          class="
            shrink-0 rounded-xl border border-neutral-300 bg-white px-4 py-2 text-sm font-semibold
            text-neutral-800 transition-colors
            hover:bg-neutral-500
            disabled:cursor-not-allowed disabled:opacity-60
          "
          :disabled="disabled || assigningRequestId === request.id"
          @click="emit('assign', request)"
        >
          {{ $t('button.assign') }}
        </button>
      </div>
    </div>

    <p
      v-if="!filteredRequests.length"
      class="py-6 text-center text-sm text-content-secondary"
    >
      {{ $t('content.noData') }}
    </p>
  </div>
</template>

<script setup lang="ts">
  import type { TripBookingRequest } from '@/types/admin-dashboard/response/trip/trip-booking'

  const props = defineProps<{
    assigningRequestId?: number | null
    bookingRequests: TripBookingRequest[]
    disabled?: boolean
  }>()

  const emit = defineEmits<{
    assign: [request: TripBookingRequest]
  }>()

  const searchKeyword = ref('')

  const filteredRequests = computed(() => {
    const keyword = searchKeyword.value.trim().toLowerCase()
    if (!keyword) return props.bookingRequests

    return props.bookingRequests.filter((request) => {
      return request.contractorName.toLowerCase().includes(keyword)
        || request.contractorPhone?.toLowerCase().includes(keyword)
        || request.driverPhone?.toLowerCase().includes(keyword)
        || request.driverName?.toLowerCase().includes(keyword)
    })
  })

  function phoneOf(request: TripBookingRequest): string {
    return request.contractorPhone?.trim() || request.driverPhone?.trim() || ''
  }
</script>

<style scoped>

</style>
