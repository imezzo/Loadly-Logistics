<template>
  <BaseForm
    ref="formRef"
    :initial-values="formInitialValues"
    :groups
    :validation-schema
    :errors
    @submit="submit"
  >
    <template #field-before-isHistorical>
      <span class="mb-2 inline-block font-bold">
        {{ $t('form.historical') }}
        <span class="text-error-600">*</span>
      </span>
    </template>

    <template #submit>
      <BaseActions
        container-class="mt-8 flex justify-end gap-3"
        :prev-button-options="{ hasIcon: true, hasScrollTop: true }"
        :next-button-options="{ asSubmit: true, hasIcon: true, hasScrollTop: true }"
        @prev="onBack"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useTripExternalInformation } from '../../composables/useTripExternalInformation'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useLookups } from '@/composables/useLookups'

  defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    next: []
  }>()

  const router = useRouter()
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)
  const { getLeads } = useLookups()

  const {
    formInitialValues,
    groups,
    validationSchema,
  } = useTripExternalInformation()

  function submit(values: Record<string, unknown>) {
    trip.value = {
      ...trip.value,
      leadId: values.leadId?.toString() || null,
      contactPersonName: String(values.contactPersonName ?? ''),
      contactPersonMobile: String(values.contactPersonMobile ?? ''),
      startDate: String(values.startDate),
      endDate: String(values.endDate),
      spotPrice: String(values.spotPrice),
      isExternalTrip: true,
      isHistorical: Boolean(values.isHistorical),
      dispatcherNotes: String(values.dispatcherNotes ?? ''),
    }
    emit('next')
  }

  function onBack() {
    router.push({ name: 'AllTrips' })
  }

  onMounted(() => {
    trip.value.isExternalTrip = true
    getLeads()
  })
</script>

<style scoped>

</style>
