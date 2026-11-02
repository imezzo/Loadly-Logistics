<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <TripForm />
  </div>
</template>

<script setup lang="ts">
  import TripForm from './components/TripForm.vue'
  import { useTripWizard } from './composables/useTripWizard'
  import { useLookups } from '@/composables/useLookups'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useApp } from '@/composables/useApp'
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'
  import { useLoading } from '@/composables/useLoading'

  const { t } = useI18n()
  const route = useRoute()
  const router = useRouter()
  const { setLoading } = useLoading()
  const { pageTitle, isEditMode } = useTripWizard()
  const { isAdminDashboard, isCompanyDashboard } = useApp()
  const {
    getCompanies,
    getContractReferenceNumbers,
    getContractVehicleTypes,
    getContractDestinations,
    contractReferenceNumbers
  } = useLookups()

  const contractStore = useContractStore()
  const tripStore = useTripStore()
  const { trip, selectedDriverVehicle } = storeToRefs(tripStore)

  const breadcrumbItems = computed(() => [
    { label: t('sidebar.tripManagement'), to: '/trips' },
    { label: pageTitle.value },
  ])

  async function initReferenceNumbers(): Promise<void> {
    if (Number(trip.value.status) !== TripStatus.COMPLETED) {
      await getContractReferenceNumbers(trip.value.companyId)

      return
    }

    contractReferenceNumbers.value = {
      items: [
        {
          label: String(trip.value.contract?.referenceNumber),
          value: trip.value.contractId.toString()
        }
      ],
      pagination: {
        page: 1,
        size: 10,
        pages: 1,
        items: 1,
      },
    }
  }

  async function initEditMode(): Promise<void> {
    if (!isEditMode.value) return

    const tripId = Number(route.params.id)
    setLoading(true)
    const { success } = await tripStore.fetchTrip(tripId)

    if (trip.value.companyId) {
      await initReferenceNumbers()
    }

    if (trip.value.contractId) {
      await getContractVehicleTypes(trip.value.contractId)
      await getContractDestinations(trip.value.contractId)
      await contractStore.getContractConsumptionType(trip.value.contractId)
    }
    setLoading(false)

    if (!success || !trip.value?.vehicleId) return

    setSelectedVehicle()
  }

  function setSelectedVehicle(): void {
    const vehicle = trip.value.vehicle
    const tail = vehicle?.tailVehicles?.[0]

    tripStore.selectedDriverVehicle = {
      id: Number(trip.value.vehicleId),
      plateNo: vehicle?.plateNo ?? '',
      plateLetters: vehicle?.plateLetters ?? '',
      vehicleTypeName: trip.value.vehicleType ?? '',
      displayText: [vehicle?.plateNo, vehicle?.plateLetters].filter(Boolean).join(' '),
      driverName: trip.value.driverName ?? vehicle?.driverName ?? null,
      driverPhone: trip.value.driverPhone ?? null,
      driverId: trip.value.driverId ? Number(trip.value.driverId) : null,
      tail: {
        plateLetters: tail?.plateLetters,
        plateNo: tail?.plateNo,
      },
    } as Vehicle
  }

  onMounted(async () => {
    if (isAdminDashboard) await getCompanies()

    if (isCompanyDashboard) await getContractReferenceNumbers()

    await initEditMode()

    if (
      (!trip.value.isEditable || trip.value.isBackfill)
      && isEditMode.value
    ) router.push({ name: 'Unauthorized' })
  })

  onUnmounted(() => {
    tripStore.resetTrip()
    selectedDriverVehicle.value = null
    contractStore.contractConsumptionType = null
  })
</script>

<style scoped>

</style>
