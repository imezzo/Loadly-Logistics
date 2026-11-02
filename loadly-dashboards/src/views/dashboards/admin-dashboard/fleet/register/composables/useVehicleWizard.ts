export function useVehicleWizard() {
  const route = useRoute()

  const isViewMode = computed(() => route.name === 'ViewVehicle')
  const isUpdateMode = computed(() => route.name === 'UpdateVehicle')
  const isCreateMode = computed(() => route.name === 'RegisterVehicle')
  const vehicleId = computed(() => {
    const id = route.params.id

    return !id || Array.isArray(id)
      ? undefined
      : String(id)
  })

  const isFormDisabled = computed(() => {
    return isViewMode.value
  })

  return {
    isViewMode,
    isUpdateMode,
    isCreateMode,
    vehicleId,
    isFormDisabled,
  }
}
