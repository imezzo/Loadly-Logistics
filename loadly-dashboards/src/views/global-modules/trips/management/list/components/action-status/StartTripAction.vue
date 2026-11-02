<template>
  <PopupError
    v-if="errorMessage"
    :model-value="errorMessage !== null"
    :message="errorMessage"
    @update:model-value="errorMessage = null"
  />

  <span
    v-if="isLoading"
    class="loader"
  />

  <TripActionStatusCard
    :label="$t('button.startTrip')"
    class="
      group bg-success-500! text-white!
      hover:bg-success-500/95!
    "
    :disabled="isLoading"
    @select="startTrip"
  >
    <template #icon>
      <IconTruck
        class="
          size-8 text-white transition-transform duration-300
          group-hover:translate-x-3
        "
      />
    </template>
  </TripActionStatusCard>
</template>

<script setup lang="ts">
  import { useTripAssistantStore } from '@/stores/admin-dashboard/trip-assistant'
  import TripActionStatusCard from './TripActionStatusCard.vue'

  const PopupError = defineAsyncComponent(() => import('@/components/popup/PopupError.vue'))

  const props = defineProps<{
    tripId: string | number
  }>()

  const emit = defineEmits<{
    started: []
  }>()

  const { t } = useI18n()
  const tripAssistantStore = useTripAssistantStore()

  const errorMessage = ref<string | null>(null)
  const isLoading = ref(false)

  async function startTrip(): Promise<void> {
    if (isLoading.value) return

    isLoading.value = true
    errorMessage.value = null

    const { success, message } = await tripAssistantStore.startTripManagement(+props.tripId)

    if (success) {
      emit('started')
      isLoading.value = false

      return
    }

    errorMessage.value = message || t('error.somethingWentWrong')
    isLoading.value = false
  }
</script>

<style scoped>

</style>
