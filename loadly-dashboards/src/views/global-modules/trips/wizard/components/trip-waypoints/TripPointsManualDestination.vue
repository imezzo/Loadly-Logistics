<template>
  <PopupError
    v-model="isUniqueWaypointError"
    :message="$t('error.uniqueWaypoint')"
  />

  <BaseForm
    ref="formRef"
    :initial-values
    :groups
    :validation-schema
    :errors="errors"
    :submit-attrs="{
      class: 'btn-primary w-full',
      disabled: isFormDisabledWithCompletedStatus,
    }"
    @submit="submit"
  >
    <template #submit-text>
      {{ editingIndex !== null ? $t('button.saveChanges') : $t('button.add') }}
    </template>
  </BaseForm>

  <TripPointsList
    v-if="trip.waypoints.length"
    :waypoints="trip.waypoints"
    @edit="editWaypoint"
    @remove="removeWaypoint"
    @reorder="reorderWaypoints"
  />
</template>

<script setup lang="ts">
  import { useTripPointForm } from '../../composables/useTripPointForm'
  import { useMapsStore } from '@/stores/admin-dashboard/maps'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { TripWizardWaypoint } from '@/types/admin-dashboard/request/trip/trip'
  import { useTripWizard } from '../../composables/useTripWizard'
  import { TripType, waypointType } from '@/types/admin-dashboard/response/trip/trip'

  const TripPointsList = defineAsyncComponent(() => import('./TripPointsList.vue'))

  defineProps<{
    errors?: Record<string, string[]>
  }>()

  const { isFormDisabledWithCompletedStatus } = useTripWizard()
  const { groups, initialValues, validationSchema } = useTripPointForm(setPointAddress)

  const mapsStore = useMapsStore()
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const formRef = useTemplateRef<{
    form?: {
      setValues: (v: Record<string, unknown>) => void
      getValues: () => Record<string, unknown>
      resetForm?: () => void
    }
    submitForm?: () => Promise<void>
  }>('formRef')

  const emptyPointValues = {
    stopType: '',
    googleMapLink: '',
    latitude: '',
    longitude: '',
    addressName: '',
    taskDetails: '',
  }

  const editingIndex = ref<number | null>(null)
  const isUniqueWaypointError = ref(false)

  function resetEditor() {
    editingIndex.value = null
    initialValues.value = { ...emptyPointValues }
    formRef.value?.form?.resetForm?.()
  }

  function removeWaypoint(index: number) {
    trip.value.waypoints.splice(index, 1)
  }

  function reorderWaypoints(newOrder: TripWizardWaypoint[]) {
    trip.value.waypoints = newOrder
  }

  async function setPointAddress(url: string) {
    const res = await mapsStore.extractPlace(url)
    if (!res.success || !res.data || !formRef.value?.form) return
    const current = formRef.value.form.getValues() as Record<string, unknown>
    formRef.value.form.setValues({
      ...current,
      latitude: String(res.data.lat),
      longitude: String(res.data.lng),
      addressName: res.data.address,
    })
  }

  function waypointToFormValues(waypoint: TripWizardWaypoint) {
    return {
      stopType: waypoint.stopType ?? '',
      googleMapLink: waypoint.googleMapLink ?? '',
      latitude: waypoint.latitude ?? '',
      longitude: waypoint.longitude ?? '',
      addressName: waypoint.addressName ?? '',
      taskDetails: waypoint.taskDetails ?? '',
    }
  }

  function editWaypoint(index: number): void {
    const waypoint = trip.value.waypoints[index]
    if (!waypoint) return

    editingIndex.value = index
    const values = waypointToFormValues(waypoint)
    initialValues.value = { ...values }
    formRef.value?.form?.setValues(values)
  }

  function isValidWaypoints(values: Record<string, unknown>): boolean {
    if (Number(trip.value.type) === TripType.RoundTrip) return true

    return !trip.value.waypoints
      .some(waypoint => (
        values.stopType !== waypointType.depot
        && waypoint.addressName?.toLowerCase() === String(values.addressName)?.toLowerCase()
      ))
  }

  function submit(values: Record<string, unknown>) {
    if (!isValidWaypoints(values)) {
      isUniqueWaypointError.value = true

      return
    }

    const payload = values as unknown as TripWizardWaypoint
    if (editingIndex.value !== null) {
      const existing = trip.value.waypoints[editingIndex.value]
      if (existing) {
        trip.value.waypoints[editingIndex.value] = { ...existing, ...payload }
      }
      editingIndex.value = null
    }
    else {
      trip.value.waypoints.push(payload)
    }

    initialValues.value = { ...emptyPointValues }
    formRef.value?.form?.resetForm?.()
  }

  defineExpose({ resetEditor })
</script>

<style scoped>

</style>
