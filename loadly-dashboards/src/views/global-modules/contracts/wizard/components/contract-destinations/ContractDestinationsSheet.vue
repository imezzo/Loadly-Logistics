<template>
  <div class="flex w-full min-w-0 flex-col gap-4">
    <DestinationsCalculationErrors
      v-if="fetchError"
      :errors="fetchError"
    />

    <DestinationsCalculationTable
      v-else-if="vehicleTypesResult?.length && destinations.length"
      :vehicle-types="vehicleTypesResult"
      :destinations="destinations"
    />

    <p
      v-else
      class="text-sm text-content-secondary"
    >
      {{ $t('content.noData') }}
    </p>
  </div>
</template>

<script setup lang="ts">
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import type {
    ContractDestinationsCalculateRequest,
    ContractDestinationsCalculatedVehicleType,
  } from '@/types/admin-dashboard/contract-destinations-calculate'
  import { useContractWizard } from '../../composables/useContractWizard'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { ContractType, type VehicleType } from '@/types/admin-dashboard/response/contract/contract-details'
  import { useLoading } from '@/composables/useLoading'
  import { hasValue } from '@/utils/transformers'

  const DestinationsCalculationErrors = defineAsyncComponent(
    () => import('./components/destination-calculation/DestinationsCalculationErrors.vue'),
  )
  const DestinationsCalculationTable = defineAsyncComponent(
    () => import('./components/destination-calculation/DestinationsCalculationTable.vue'),
  )

  const { t } = useI18n()
  const { formData } = useContractWizard()
  const contractStore = useContractStore()
  const { setLoading } = useLoading()

  const fetchError = ref<Record<string, string> | null>(null)
  const vehicleTypesResult = ref<ContractDestinationsCalculatedVehicleType[] | null>(null)

  const contractType = computed(() => {
    const type = Number(formData.value?.contractType)
    if (!hasValue(type)) return null

    return Number.isFinite(type) ? type : null
  })

  const monthlyWorkingDays = computed(() => {
    const days = Number(formData.value?.monthlyWorkingDays)
    if (!hasValue(days)) return null

    return Number.isFinite(days) ? days : null
  })

  const destinations = computed((): ContractWizardDestination[] => {
    return Array.isArray(formData.value?.destinations)
      ? formData.value.destinations
      : []
  })

  function getDestinationRequest(): ContractDestinationsCalculateRequest | null {
    const reqContractType = Number(contractType.value)
    const contractDestinations = destinations.value.map((
      { expectedKms: kms, numberOfDays, nameAr: name_ar, nameEn: name_en }
    ) => ({
      kms: Number(kms),
      numberOfDays: Number(numberOfDays),
      name_ar,
      name_en,
    }))

    const vehicle_types = (formData.value?.vehicleTypes as VehicleType[])
      .filter(vehicleType => vehicleType.vehicleTypeId)
      .map(vehicleType => ({
        vehicleTypeId: Number(vehicleType.vehicleTypeId),
        consumptionRate: Number(vehicleType.consumptionRate),
        monthlyRental: Number(vehicleType.monthlyRental),
        tripsPerMonth: reqContractType === ContractType.Included
          ? null
          : Number(vehicleType.tripsPerMonth),
      }))

    return {
      contractType: reqContractType,
      monthly_working_days: Number(monthlyWorkingDays.value),
      destinations: contractDestinations,
      vehicle_types,
    }
  }

  function setCalculationErrors(errors: unknown, message: string): void {
    const validationErrors: Record<string, string> = {}

    if (errors && typeof errors === 'object') {
      Object.entries(errors).forEach(([key, value]) => {
        validationErrors[key] = value[0] ?? ''
      })
    }

    fetchError.value = validationErrors || { message: message || t('content.destinationCostSheetError') }
  }

  async function fetchCalculation(): Promise<void> {
    const destinationRequestParams = getDestinationRequest()
    if (!destinationRequestParams) return

    setLoading(true)
    fetchError.value = null

    const { data, errors, message, success } = await contractStore.calculateDestinations(destinationRequestParams)

    setLoading(false)

    if (!success) {
      vehicleTypesResult.value = null
      setCalculationErrors(errors, message)

      return
    }

    vehicleTypesResult.value = data?.vehicleTypes ?? null
    if (!vehicleTypesResult.value) setCalculationErrors(null, message)
  }

  onMounted(fetchCalculation)
</script>

<style scoped>

</style>
