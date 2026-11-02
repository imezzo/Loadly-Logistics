<template>
  <CancelTripForm
    v-if="showForm"
    ref="formRef"
    :error-message="cancelError"
    :error-form-errors="cancelFormErrors"
    @discard="onDiscard"
    @submit="onCancelTrip"
  />

  <TripActionStatusCard
    v-else
    :label="$t('button.cancelTrip')"
    class="
      group bg-error-600/80! text-white!
      hover:bg-error-600!
    "
    :disabled="isLoading"
    @select="$emit('select')"
  >
    <template #icon>
      <IconCross
        class="
          size-8 text-white transition-transform duration-300
          group-hover:rotate-180
        "
      />
    </template>
  </TripActionStatusCard>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import CancelTripForm from './CancelTripForm.vue'
  import TripActionStatusCard from '../TripActionStatusCard.vue'

  const props = withDefaults(defineProps<{
    tripId: string | number
    showForm?: boolean
  }>(), {
    showForm: false,
  })

  const emit = defineEmits<{
    select: []
    discard: []
    cancelled: []
  }>()

  const { t } = useI18n()
  const tripStore = useTripStore()

  const formRef = useTemplateRef('formRef')
  const isLoading = ref(false)
  const cancelError = ref<string | null>(null)
  const cancelFormErrors = ref<Record<string, string[]>>({})

  function resetCancelErrors(): void {
    cancelError.value = null
    cancelFormErrors.value = {}
  }

  function onDiscard(): void {
    resetCancelErrors()
    formRef.value?.resetForm()
    emit('discard')
  }

  async function onCancelTrip(cancellationReason: string): Promise<void> {
    if (isLoading.value) return

    isLoading.value = true
    resetCancelErrors()

    const { success, message, errors } = await tripStore.cancelTrip(
      +props.tripId,
      cancellationReason,
    )

    if (success) {
      formRef.value?.resetForm()
      emit('cancelled')
      isLoading.value = false

      return
    }

    cancelError.value = message || t('error.somethingWentWrong')
    const reasonErrors = errors?.cancellationReason ?? errors?.CancellationReason
    cancelFormErrors.value = reasonErrors ? { reason: reasonErrors } : {}
    isLoading.value = false
  }

  watch(() => props.showForm, (visible) => {
    if (!visible) {
      resetCancelErrors()
      formRef.value?.resetForm()
    }
  })
</script>

<style scoped>

</style>
