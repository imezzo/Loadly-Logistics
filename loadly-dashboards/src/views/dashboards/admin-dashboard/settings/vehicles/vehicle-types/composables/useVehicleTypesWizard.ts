import { array, object, string } from 'yup'
import type { VehicleType } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
import type { SelectOption } from '@/types/ui'
import { useEnum } from '@/composables/useEnum'
import { useVehicleTagSettingsStore } from '@/stores/admin-dashboard/vehicle-tag-settings'
import type { VehicleTag } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'

export function useVehicleTypesWizard(vehicleType: ComputedRef<VehicleType | null>) {
  const { t } = useI18n()
  const { enums } = useEnum()
  const vehicleTagSettingsStore = useVehicleTagSettingsStore()
  const tagOptions = ref<SelectOption[]>([])

  onMounted(async () => {
    const { success, data } = await vehicleTagSettingsStore.getList({
      pagination: {
        page: 1,
        size: 100,
      },
    })

    if (!success || !data) return

    tagOptions.value = data.vehicleTags.map((tag: VehicleTag) => ({
      label: tag.name,
      value: String(tag.id),
      content: () => tagOptionContent(tag),
    }))
  })

  function tagOptionContent(tag: VehicleTag): string {
    const badgeClass = tag.isPricing
      ? 'rounded-full bg-amber-100 px-2 py-0.5 text-xs font-medium text-amber-600'
      : ''
    const badgeLabel = tag.isPricing ? t('table.pricing') : ''

    return `
      <span class="flex w-full items-center justify-between gap-3">
        <span>${tag.name}</span>
        <span class="${badgeClass}">${badgeLabel}</span>
      </span>`
  }

  const vehicleTypeCategoryOptions = computed(() => enums.value?.VehicleTypeCategory ?? [])
  const vehicleUnitTypeOptions = computed(() => enums.value?.VehicleUnitType ?? [])
  const statusOptions = computed(() => enums.value?.AddressStatus ?? [])

  const initialValues = computed(() => ({
    nameEn: vehicleType.value?.nameEn || '',
    nameAr: vehicleType.value?.nameAr || '',
    category: vehicleType.value?.category?.toString() || '',
    vehicleUnitType: vehicleType.value?.vehicleUnitType?.toString() || '',
    status: vehicleType.value?.status?.toString() || '',
    tagIds: vehicleType.value?.tags?.map((tag) => String(tag.id)) ?? [],
  }))

  const validationSchema = computed(() => object({
    nameEn: string().required().max(255),
    nameAr: string().required().max(255),
    category: string().required(),
    vehicleUnitType: string().required(),
    status: string().required(),
    tagIds: array().of(string()),
  }))

  const fieldsSchema = computed(() => [
    {
      name: 'nameEn',
      as: 'BaseInput',
      type: 'text',
      label: t('form.nameEn'),
      labelRequired: true,
    },
    {
      name: 'nameAr',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.nameAr'),
      label: t('form.nameAr'),
      labelRequired: true,
    },
    {
      name: 'category',
      as: 'BaseSelect',
      options: vehicleTypeCategoryOptions.value,
      label: t('form.chooseVehicleCategory'),
      searchable: true,
      labelRequired: true,
    },
    {
      name: 'vehicleUnitType',
      as: 'BaseSelect',
      options: vehicleUnitTypeOptions.value,
      searchable: true,
      label: t('form.vehicleUnitTypes'),
      labelRequired: true,
    },
    {
      name: 'status',
      as: 'BaseSelect',
      options: statusOptions.value,
      searchable: true,
      label: t('table.status'),
      labelRequired: true,
    },
    {
      name: 'tagIds',
      as: 'BaseMultiSelect',
      options: tagOptions.value,
      searchable: true,
      label: t('form.tags'),
    },
  ])

  return {
    fieldsSchema,
    initialValues,
    validationSchema,
  }
}
