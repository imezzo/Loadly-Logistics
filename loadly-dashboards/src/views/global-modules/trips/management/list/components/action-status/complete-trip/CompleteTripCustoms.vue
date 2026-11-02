<template>
  <PopupError
    v-if="errorMessage"
    :model-value="errorMessage !== null"
    :message="errorMessage"
    @update:model-value="errorMessage = null"
  />

  <button
    v-if="showEnterCustoms"
    type="button"
    class="btn-link flex h-8 items-center gap-1.5 px-3 py-1 text-lg font-bold"
    :disabled="isLoading || disabled"
    @click="enterCustoms"
  >
    <IconExit class="size-5" />
    {{ $t('button.enterCustoms') }}
  </button>

  <button
    v-if="showLeaveCustoms"
    type="button"
    class="btn-link flex h-8 items-center gap-1.5 px-3 py-1 text-lg font-bold"
    :disabled="isLoading || disabled"
    @click="leaveCustoms"
  >
    <IconExit class="size-5 rotate-180" />
    {{ $t('button.leaveCustoms') }}
  </button>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useTripAssistantStore } from '@/stores/admin-dashboard/trip-assistant'
  import { TripCustomsStatus } from '@/types/admin-dashboard/response/trip/trip'

  const props = withDefaults(defineProps<{
    tripId: string | number
    disabled?: boolean
  }>(), {
    disabled: false,
  })

  const PopupError = defineAsyncComponent(() => import('@/components/popup/PopupError.vue'))

  const { t } = useI18n()
  const tripStore = useTripStore()
  const tripAssistantStore = useTripAssistantStore()
  const { trip } = storeToRefs(tripStore)

  const isLoading = ref(false)
  const errorMessage = ref<string | null>(null)

  const showEnterCustoms = computed(() => {
    if (!trip.value?.hasCustoms) return false

    return trip.value.customsStatus !== TripCustomsStatus.CLEARED
      && trip.value.customsStatus !== TripCustomsStatus.UNDER_INSPECTION
  })

  const showLeaveCustoms = computed(() => {
    return !!trip.value?.hasCustoms
      && trip.value.customsStatus === TripCustomsStatus.UNDER_INSPECTION
  })

  async function enterCustoms(): Promise<void> {
    await runCustomsAction(() =>
      tripAssistantStore.enterTripCustoms(+props.tripId, new FormData()),
    )
  }

  async function leaveCustoms(): Promise<void> {
    await runCustomsAction(() =>
      tripAssistantStore.exitTripCustoms(+props.tripId, new FormData()),
    )
  }

  async function runCustomsAction(
    action: () => Promise<{ success: boolean; message?: string }>,
  ): Promise<void> {
    isLoading.value = true
    errorMessage.value = null

    const { success, message } = await action()

    if (!success) {
      errorMessage.value = message || t('error.somethingWentWrong')
      isLoading.value = false

      return
    }

    await tripStore.fetchTrip(+props.tripId)
    isLoading.value = false
  }
</script>

<style scoped>

</style>
