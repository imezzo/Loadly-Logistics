<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
  >
    <div class="w-full p-4">
      <h2 class="mb-6 text-xl font-bold text-neutral-800">
        {{ $t('title.addNewReview') }}
      </h2>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-4 w-full"
      />

      <BaseForm
        ref="reviewForm"
        :validation-schema
        :initial-values
        :fields-schema
        :errors="formErrors"
        container-class="grid grid-cols-1 gap-x-6 gap-y-4 md:grid-cols-2"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-center gap-3"
            :prev-button-options="prevButtonOptions"
            :next-button-options="nextButtonOptions"
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { object, string } from 'yup'
  import type { Schema } from '@/types/form'
  import type { MonthlyContract } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useLookups } from '@/composables/useLookups'
  import { useLoading } from '@/composables/useLoading'
  import { useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
  import { formatDateLocal } from '@/utils/date'

  interface ReviewForm {
    form: {
      resetForm: () => void
      setFieldValue: (field: string, value: unknown) => void
      values: Record<string, unknown>
    }
  }

  const emit = defineEmits<{
    refresh: []
  }>()

  const props = defineProps<{
    review?: MonthlyContract | null
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const isEditMode = computed(() => !!props.review)

  useVeeValidate()
  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const {
    companies,
    contractReferenceNumbers,
    getCompanies,
    getContractReferenceNumbers,
    hasMoreCompanies,
    hasMoreContractReferenceNumbers,
    loadMoreCompanies,
    loadMoreContractReferenceNumbers,
  } = useLookups()

  const monthlyContractStore = useMonthlyContractStore()

  const reviewFormRef = useTemplateRef<ReviewForm>('reviewForm')

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const formValues = computed(() => reviewFormRef.value?.form?.values)
  const companyId = computed(() => String(formValues.value?.companyId ?? ''))
  const periodStart = computed(() => String(formValues.value?.periodStart ?? ''))

  const companyOptions = computed(() => {
    if (isEditMode.value && props.review) {
      return [{ value: String(props.review.companyId), label: props.review.companyName }]
    }

    return companies.value?.items ?? []
  })

  const contractOptions = computed(() => {
    if (isEditMode.value && props.review) {
      return [{ value: String(props.review.contractId), label: props.review.contractName }]
    }

    return contractReferenceNumbers.value?.items ?? []
  })

  const initialValues = computed(() => ({
    companyId: props.review?.companyId ?? '',
    contractId: props.review ? String(props.review.contractId) : '',
    name: props.review?.name ?? '',
    periodEnd: formatDateLocal(props.review?.periodEnd ?? ''),
    periodStart: formatDateLocal(props.review?.periodStart ?? ''),
  }))

  const validationSchema = computed(() => object({
    companyId: isEditMode.value
      ? string().notRequired()
      : string().required(t('validation.required', { field: t('content.company') })),
    contractId: isEditMode.value
      ? string().notRequired()
      : string().required(t('validation.required', { field: t('content.contract') })),
    name: string(),
    periodEnd: string()
      .required(t('validation.required', { field: t('table.dateTo') }))
      .test('period-order', function validatePeriodOrder(value) {
        const start = this.parent.periodStart as string | undefined
        if (!value || !start || value >= start) return true

        return this.createError({ message: t('validation.minDate', { min: start }) })
      }),
    periodStart: string().required(t('validation.required', { field: t('table.dateFrom') })),
  }))

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'companyId',
      as: 'BaseSelect',
      options: companyOptions.value,
      searchable: true,
      disabled: isEditMode.value,
      label: t('content.company'),
      labelRequired: !isEditMode.value,
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      onScrollToEnd: hasMoreCompanies.value && !isEditMode.value
        ? loadMoreCompanies
        : undefined,
      onChange: (value: unknown) => {
        if (isEditMode.value || companyId.value == undefined) return

        setFieldValue('contractId', '')

        if (value) {
          getContractReferenceNumbers(String(value), 1)
        }
      },
    },
    {
      name: 'contractId',
      as: 'BaseSelect',
      options: contractOptions.value,
      searchable: true,
      disabled: isEditMode.value || !companyId.value,
      label: t('content.contract'),
      labelRequired: !isEditMode.value,
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      onScrollToEnd: hasMoreContractReferenceNumbers.value && !isEditMode.value
        ? loadMoreContractReferenceNumbers
        : undefined,
    },
    {
      name: 'periodStart',
      as: 'BaseInput',
      type: 'date',
      disabled: isEditMode.value,
      label: t('table.dateFrom'),
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      placeholder: t('form.datePlaceholder'),
    },
    {
      name: 'periodEnd',
      as: 'BaseInput',
      type: 'date',
      disabled: isEditMode.value,
      label: t('table.dateTo'),
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      placeholder: t('form.datePlaceholder'),
      min: periodStart.value || undefined,
    },
    {
      name: 'name',
      as: 'BaseInput',
      type: 'text',
      disabled: isEditMode.value,
      label: t('form.name'),
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      containerClass: 'w-full md:col-span-2',
    },
  ])

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'btn-secondary h-11 min-w-40',
  }))

  const nextButtonOptions = computed(() => isEditMode.value
    ? undefined
    : {
      label: t('button.addReview'),
      class: 'btn-primary h-11 min-w-40',
      disabled: isLoading.value,
      asSubmit: true,
    })

  function setFieldValue(field: string, value: unknown): void {
    reviewFormRef.value?.form?.setFieldValue(field, value)
  }

  function resetForm(): void {
    reviewFormRef.value?.form?.resetForm()
    errorMessage.value = null
    formErrors.value = undefined
  }

  function discard(): void {
    resetForm()
    model.value = false
  }

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    errorMessage.value = null
    formErrors.value = undefined
    setLoading(true)

    const payload = {
      contractSlotId: null,
      name: String(values.name ?? '').trim(),
      periodEnd: String(values.periodEnd ?? ''),
      periodStart: String(values.periodStart ?? ''),
    }
    const contractId = isEditMode.value
      ? String(props.review?.contractId ?? '')
      : String(values.contractId ?? '')
    const { success, message, errors } = isEditMode.value && props.review
      ? await monthlyContractStore.update(contractId, props.review.snapshotId, payload)
      : await monthlyContractStore.create(contractId, payload)

    setLoading(false)

    if (success) {
      resetForm()
      model.value = false
      emit('refresh')

      return
    }

    errorMessage.value = message || t('error.networkError')
    formErrors.value = errors || {}
  }

  watch(model, async (isOpen) => {
    if (!isOpen) {
      resetForm()

      return
    }

    if (!companies.value?.items?.length && !isEditMode.value) {
      await getCompanies(1)
    }
  })
</script>

<style scoped>

</style>
