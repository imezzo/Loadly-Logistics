<template>
  <BasePopup
    v-model="model"
    content-class="max-w-3/4! w-full md:max-w-250!"
  >
    <div class="flex w-full flex-col gap-4 px-6 py-4 pe-12">
      <header class="flex flex-wrap items-baseline gap-2">
        <h2 class="text-xl font-bold text-neutral-800">
          {{ $t('content.contractorsRequests') }}
        </h2>

        <span class="text-sm font-semibold text-primary">
          {{ $t('content.requestsCount', { count: bookingRequests.length }) }}
        </span>
      </header>

      <BaseInputSearch
        v-model="searchKeyword"
        wrapper-class="
          flex h-11 w-full items-center rounded-lg border border-neutral-300 bg-white px-3
        "
        :placeholder="$t('content.searchForContractors')"
      />

      <div
        class="
          grid max-h-96 grid-cols-1 gap-3 overflow-y-auto px-px py-1
          md:grid-cols-2
          xl:grid-cols-3
        "
      >
        <ContractorBookingRequestCard
          v-for="request in filteredRequests"
          :key="request.id"
          :request="request"
          :is-loading="assigningRequestId === request.id"
          @assign="onAssign"
        />
      </div>

      <p
        v-if="!filteredRequests.length"
        class="py-8 text-center text-sm text-content-secondary"
      >
        {{ $t('content.noData') }}
      </p>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { TripBookingRequest } from '@/types/admin-dashboard/response/trip/trip-booking'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import ContractorBookingRequestCard from './ContractorBookingRequestCard.vue'

  const props = defineProps<{
    bookingRequests: TripBookingRequest[]
    tripId: string
  }>()

  const emit = defineEmits<{
    assigned: []
    error: [message: string]
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const { t } = useI18n()
  const tripStore = useTripStore()

  const searchKeyword = ref('')
  const assigningRequestId = ref<number | null>(null)

  const filteredRequests = computed(() => {
    const keyword = searchKeyword.value.trim().toLowerCase()
    if (!keyword) return props.bookingRequests

    return props.bookingRequests.filter((request) => {
      return request.contractorName.toLowerCase().includes(keyword)
        || request.driverPhone?.toLowerCase().includes(keyword)
        || request.driverName?.toLowerCase().includes(keyword)
    })
  })

  async function onAssign(request: TripBookingRequest): Promise<void> {
    if (assigningRequestId.value) return

    assigningRequestId.value = request.id

    const { success, message } = await tripStore.approveBookingRequest(props.tripId, request.id)

    assigningRequestId.value = null

    if (success) {
      model.value = false
      emit('assigned')

      return
    }

    emit('error', message || t('error.somethingWentWrong'))
  }

  watch(model, (isOpen) => {
    if (!isOpen) searchKeyword.value = ''
  })
</script>

<style scoped>

</style>
