<template>
  <BaseForm
    ref="contractForm"
    :initial-values="initialValues"
    :validation-schema="validationSchema"
    :fields-schema="fieldsSchema"
    :errors="props.errors"
    container-class="flex flex-wrap justify-between gap-6"
    @submit="onSubmit"
  >
    <template
      v-if="hasCompanyPermission && isCreateMode"
      #field-after-companyId
    >
      <div class="-mt-3.5 w-full">
        <RouterLink
          :to="{ name: 'CreateCompany' }"
          class="font-medium text-primary underline"
        >
          {{ $t('button.createNewCompany') }}
        </RouterLink>
      </div>
    </template>

    <template #field-after-startDate>
      <DurationSelector
        v-if="!isDisabled && !!hasStartDate"
        :options="DURATION_OPTIONS"
        :active-months="activeDurationMonths"
        :disabled="!hasStartDate || isDisabled"
        @select="setEndDate"
      />
    </template>

    <template #field-endDate="{ value, formValues: values }">
      <DurationDetector :duration="getDuration(value, values?.startDate?.toString())" />
    </template>

    <template #submit>
      <BaseActions
        :prev-button-options="{ label: $t('button.discard') }"
        :next-button-options="{ label: saveButtonTitle, asSubmit: !isFormDisabled, hasScrollTop: true }"
        @prev="router.push({name: 'ContractsList'})"
        @next="emit('next')"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import DurationDetector from './DurationDetector.vue'
  import DurationSelector from './DurationSelector.vue'
  import { useCompanyInformation } from '../../composables/contract-information/useCompanyInformation'
  import { useContractDuration } from '../../composables/contract-information/useContractDuration'
  import { useContractWizard } from '../../composables/useContractWizard'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { Permission, Company } from '@/types/response/permission-group'
  import { useApp } from '@/composables/useApp'

  const props = defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    next: []
  }>()

  const { isAdminDashboard } = useApp()
  const router = useRouter()
  const contractForm = ref<{
    form?: { values: Record<string, unknown>,
      setFieldValue: (name: string, value: unknown) => void
    }
    submitForm?: () => Promise<void>
  } | null>(null)

  useVeeValidate()
  const { hasPermission: hasCompanyPermission } = usePermission(Permission.Company, Company.Create)
  const {
    fieldsSchema,
    getCompanies,
    initialValues: defaultInitialValues,
    validationSchema,
    isDisabled,
  } = useCompanyInformation()
  const {
    contract,
    formData,
    isCreateMode,
    isFormDisabled,
    isUpdateMode,
    isViewMode,
    saveButtonTitle,
  } = useContractWizard()

  const {
    DURATION_OPTIONS,
    activeDurationMonths,
    getDuration,
    hasStartDate,
    setEndDate,
  } = useContractDuration(contractForm)

  const initialValues = computed(() => {
    const formValues = formData.value
    const base = defaultInitialValues.value

    if (formValues && Object.keys(formValues).length > 0) {
      const merged = { ...base, ...formValues }

      if (merged.salesRepId) {
        merged.salesRepId = String(merged.salesRepId)
      }
      else if ((isViewMode.value || isUpdateMode.value) && contract.value?.salesRepId != null) {
        merged.salesRepId = String(contract.value.salesRepId)
      }

      return merged
    }

    if ((isViewMode.value || isUpdateMode.value) && contract.value?.salesRepId != null) {
      return { ...base, salesRepId: String(contract.value.salesRepId) }
    }

    return base
  })

  function onSubmit(values: Record<string, unknown>): void {
    emit('update', values)
    emit('next')
  }

  function submitStep(): void {
    contractForm.value?.submitForm?.()
  }

  defineExpose({ submitStep })

  onMounted(async () => {
    if (!isAdminDashboard) return
    await getCompanies()
  })
</script>

<style scoped>

</style>
