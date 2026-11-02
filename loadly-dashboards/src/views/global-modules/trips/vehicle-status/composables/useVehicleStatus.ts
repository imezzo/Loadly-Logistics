import { useDashboardStore } from '@/stores/admin-dashboard/dashboard'

export function useVehicleStatus() {
  const { t } = useI18n()
  const dashboardStore = useDashboardStore()
  const { vehicleStatus } = storeToRefs(dashboardStore)

  const totalVehicles = computed(() =>
    vehicleStatus.value.availableVehiclesCount
    + vehicleStatus.value.inRouteVehiclesCount
    + vehicleStatus.value.outOfServiceVehiclesCount
  )

  const statusCards = computed(() => [
    {
      count: vehicleStatus.value.availableVehiclesCount,
      label: t('content.availableVehicles'),
      labelClass: 'text-success-500',
    },
    {
      count: vehicleStatus.value.inRouteVehiclesCount,
      label: t('content.vehicleInRoute'),
      labelClass: 'text-[#9333EA]',
    },
    {
      count: vehicleStatus.value.outOfServiceVehiclesCount,
      label: t('content.outOfServiceVehicles'),
      labelClass: 'text-primary',
    },
    {
      count: vehicleStatus.value.garageVehiclesCount,
      label: t('content.vehicleInGarage'),
      labelClass: 'text-warning-500',
    },
    {
      count: vehicleStatus.value.workshopVehiclesCount,
      label: t('content.vehicleInWorkshop'),
      labelClass: 'text-warning-300',
    },
    {
      count: vehicleStatus.value.agencyVehiclesCount,
      label: t('content.vehicleInAgency'),
      labelClass: 'text-primary',
    },
  ])

  async function fetchVehicleStatus(): Promise<void> {
    const { success, data } = await dashboardStore.fetchVehicleStatus()

    if (!success || !data) return

    vehicleStatus.value = data
  }

  return {
    fetchVehicleStatus,
    statusCards,
    totalVehicles,
  }
}
