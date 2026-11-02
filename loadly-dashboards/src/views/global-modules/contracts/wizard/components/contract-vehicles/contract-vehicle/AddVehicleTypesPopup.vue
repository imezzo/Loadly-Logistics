<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-220!"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-2 text-2xl font-bold text-neutral-800">
        {{ $t('content.chooseVehicleTypes') }}
      </h2>

      <p class="mb-6 text-sm text-neutral-600">
        {{ $t('content.chooseVehicleTypesDescription') }}
      </p>

      <VehicleTypeMultiSelect
        v-if="!isFormDisabled"
        v-model="multiSelectValues"
        class="mb-6"
        :options="vehicleTypeSelectOptions"
        :placeholder="$t('form.select')"
        @add-types="onAddSelectedTypes"
        @scroll-to-end="fetchVehicleTypes((vehicleTypesList?.pagination?.page ?? 1) + 1)"
      />

      <div class="max-h-75 space-y-4 overflow-y-auto">
        <AddedVehicleTypeCard
          v-for="vehicleTypeId in addedVehicleIds"
          :key="vehicleTypeId"
          :vehicle-type-id="vehicleTypeId"
          :vehicle-type-name="getVehicleTypeName(vehicleTypeId)"
          :config="getVehicleConfig(vehicleTypeId) ?? {}"
          :vehicle-configs="vehicleConfigs"
          :type-id="typeId"
          @remove="removeAddedType(vehicleTypeId)"
          @update-config="({ field, value }) => updateVehicleConfig(vehicleTypeId, field, value)"
        />
      </div>

      <p
        v-if="validationError"
        class="mt-2 text-sm text-error-600"
      >
        {{ validationError }}
      </p>

      <BaseActions
        container-class="mt-6 justify-center gap-3"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="onCancel"
        @next="onSave"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import AddedVehicleTypeCard from './AddedVehicleTypeCard.vue'
  import VehicleTypeMultiSelect from './VehicleTypeMultiSelect.vue'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { useContractWizard } from '../../../composables/useContractWizard'
  import type { VehicleType } from '@/types/admin-dashboard/response/contract/contract-details.ts'

  const props = defineProps<{
    existingVehicleTypes?: VehicleType[]
    typeId?: number
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const emit = defineEmits<{
    add: [vehicles: VehicleType[]]
    remove: [vehicleTypeId: number]
  }>()

  const vehicleStore = useVehicleStore()
  const { vehicleTypesList } = storeToRefs(vehicleStore)
  const { isFormDisabled } = useContractWizard()
  const { t } = useI18n()

  const multiSelectValues = ref<string[]>([])
  const addedVehicleIds = ref<number[]>([])
  const validationError = ref<string | null>(null)
  const vehicleConfigs = ref<Record<number, {
    carCount?: number
    extraTrip?: number
    ExtraWorkingDayFees?: number
    consumptionRate?: number
    tripsPerMonth?: number
    maxKms?: number
    OverWeightPricePerTon?: number
    contractorOverWeightPricePerTon?: number
    monthlyRental?: number
    minBillableWeightTons?: number
    maxPayloadTons?: number
  }>>({})

  const vehicleTypes = computed(() => vehicleTypesList.value?.vehicleTypes ?? [])

  const vehicleTypeSelectOptions = computed<SelectOption[]>(() => {
    const addedSet = new Set(addedVehicleIds.value)

    return vehicleTypes.value
      .filter((vt) => !addedSet.has(vt.id))
      .map((vt) => ({ value: String(vt.id), label: vt.name }))
  })

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
    class: 'flex h-13.5 w-1/2 items-center justify-center gap-2 rounded-2xl bg-black/15 px-4 py-4.25 font-medium text-content-secondary transition-colors hover:bg-black/20 sm:w-52.5',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.saveVehicleTypes'),
    class: 'w-1/2 h-13.5 btn-primary py-3.75 sm:w-75',
    disabled: addedVehicleIds.value.some(
      (id) => vehicleConfigs.value[id]?.carCount && !Number.isInteger(vehicleConfigs.value[id]?.carCount),
    ),
  }))

  function getVehicleTypeName(id: number): string {
    return vehicleTypes.value.find((vt) => vt.id === id)?.name ?? ''
  }

  function initializeVehicleConfig(id: number): void {
    if (!vehicleConfigs.value[id]) {
      vehicleConfigs.value[id] = {
        carCount: undefined,
        extraTrip: undefined,
        ExtraWorkingDayFees: undefined,
        consumptionRate: undefined,
        tripsPerMonth: undefined,
        maxKms: undefined,
        OverWeightPricePerTon: undefined,
        contractorOverWeightPricePerTon: undefined,
        monthlyRental: undefined,
        minBillableWeightTons: undefined,
        maxPayloadTons: undefined,
      }
    }
  }

  function onAddSelectedTypes(): void {
    const idsToAdd = multiSelectValues.value.map(Number).filter((id) => !addedVehicleIds.value.includes(id))
    idsToAdd.forEach((id) => {
      addedVehicleIds.value.push(id)
      initializeVehicleConfig(id)
    })
    multiSelectValues.value = []
  }

  function removeAddedType(id: number): void {
    addedVehicleIds.value = addedVehicleIds.value.filter((vid) => vid !== id)
    vehicleConfigs.value = Object.fromEntries(
      Object.entries(vehicleConfigs.value).filter(([key]) => Number(key) !== id)
    ) as typeof vehicleConfigs.value

    const isExisting = props.existingVehicleTypes?.some((vt) => vt.vehicleTypeId === id) ?? false
    if (isExisting) {
      emit('remove', id)
    }
  }

  function getVehicleConfig(id: number) {
    return vehicleConfigs.value[id]
  }

  function updateVehicleConfig(id: number, field: string, value?: string | number | null): void {
    validationError.value = null
    initializeVehicleConfig(id)
    if (!vehicleConfigs.value[id]) return
    const isEmpty = value === '' || value == null
    const num = Number(value)
    vehicleConfigs.value[id][field as keyof typeof vehicleConfigs.value[number]] = isEmpty || Number.isNaN(num)
      ? undefined
      : num
  }

  const isVehicleLoading = ref(false)
  async function fetchVehicleTypes(page = 1): Promise<void> {
    const { page: currentPage = 1, pages = 2 } = vehicleTypesList.value?.pagination ?? {}
    if (isVehicleLoading.value || currentPage >= pages) return

    isVehicleLoading.value = true
    const { success, data } = await vehicleStore.getVehicleTypes({ page, pageSize: 20 })
    if (success) {
      vehicleTypesList.value = {
        ...vehicleStore.vehicleTypesList,
        vehicleTypes: [...(vehicleStore.vehicleTypesList?.vehicleTypes ?? []), ...data.vehicleTypes],
        pagination: data.pagination,
      }
    }
    isVehicleLoading.value = false
  }

  function onSave(): void {
    validationError.value = null

    const fullList = addedVehicleIds.value.map((id) => {
      const vehicleType = vehicleTypes.value.find((vt) => vt.id === id)
      const config = vehicleConfigs.value[id] ?? {}

      return {
        OverWeightPricePerTon: config.OverWeightPricePerTon,
        contractorOverWeightPricePerTon: config.contractorOverWeightPricePerTon,
        carCount: config.carCount,
        consumptionRate: config.consumptionRate,
        extraTrip: config.extraTrip,
        ExtraWorkingDayFees: config.ExtraWorkingDayFees,
        id,
        maxKms: config.maxKms,
        maxPayloadTons: config.maxPayloadTons,
        minBillableWeightTons: config.minBillableWeightTons,
        monthlyRental: config.monthlyRental,
        name: vehicleType?.name ?? '',
        tripsPerMonth: config.tripsPerMonth,
        trucks: [],
      }
     }) as VehicleType[]

    emit('add', fullList)
    resetForm()
    model.value = false
  }

  function onCancel(): void {
    validationError.value = null
    resetForm()
    model.value = false
  }

  function resetForm(): void {
    multiSelectValues.value = []
    addedVehicleIds.value = []
    vehicleConfigs.value = {}
  }

  function restoreSavedData(): void {
    if (!props.existingVehicleTypes?.length) return

    const ids: number[] = []
    const configs: typeof vehicleConfigs.value = {}

    props.existingVehicleTypes.forEach((existingType) => {
      if (existingType.vehicleTypeId) {
        ids.push(existingType.vehicleTypeId)
        configs[existingType.vehicleTypeId] = {
          carCount: existingType.carCount,
          extraTrip: existingType.extraTrip ?? 0,
          ExtraWorkingDayFees: existingType.ExtraWorkingDayFees ?? 0,
          consumptionRate: existingType.consumptionRate ?? 0,
          tripsPerMonth: existingType.tripsPerMonth ?? 0,
          maxKms: existingType.maxKms ?? 0,
          OverWeightPricePerTon: existingType.OverWeightPricePerTon ?? 0,
          contractorOverWeightPricePerTon: existingType.contractorOverWeightPricePerTon ?? 0,
          monthlyRental: existingType.monthlyRental ?? 0,
          minBillableWeightTons: existingType.minBillableWeightTons ?? 0,
          maxPayloadTons: existingType.maxPayloadTons ?? 0,
        }
      }
    })

    addedVehicleIds.value = ids
    vehicleConfigs.value = configs
  }

  watch(
    () => model.value,
    (isOpen) => {
      if (isOpen) {
        fetchVehicleTypes()
        restoreSavedData()

        return
      }

      resetForm()
    }
  )
</script>

<style scoped>

</style>
