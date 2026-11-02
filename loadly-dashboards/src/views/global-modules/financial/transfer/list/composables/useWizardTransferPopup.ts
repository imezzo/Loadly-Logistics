import { mixed, number, object, string } from 'yup'
import type { Schema } from '@/types/form'
import type { SelectOption } from '@/types/ui'
import type { BankTransferListItem } from '@/types/response/financial/bank-transfer'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useEnum } from '@/composables/useEnum'
import { useLookups } from '@/composables/useLookups'
import { PayerType } from '@/types/bank-transfer.enums'

interface BaseFormExpose {
  form?: {
    resetForm: () => void
    setFieldValue: (field: string, value: unknown, validate?: boolean) => void
  }
}

const defaultInitialValues = {
  payerType: PayerType.COMPANY,
  payerId: '',
  amount: '',
  paymentMethod: '',
  issueDate: '',
  externalPaymentReferenceNumber: '',
  attachment: undefined,
}

export function useWizardTransferPopup(transfer: Ref<BankTransferListItem | null>) {
  useVeeValidate()
  const { t } = useI18n()
  const { enums } = useEnum()
  const {
    companies,
    getCompanies,
    getLeads,
    hasMoreCompanies,
    hasMoreLeads,
    leads,
    loadMoreCompanies,
    loadMoreLeads,
  } = useLookups()

  const form = useTemplateRef<BaseFormExpose | null>('form')

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()
  const payerType = ref<PayerType>(PayerType.COMPANY)

  const isEditMode = computed(() => transfer.value !== null)

  const paymentMethodOptions = computed<SelectOption[]>(() => enums.value?.PaymentMethod ?? [])

  const payerTypeOptions = computed<SelectOption[]>(() => [
    { value: PayerType.COMPANY, label: t('content.company') },
    { value: PayerType.LEAD, label: t('content.leadCustomer') },
  ])

  const payerOptions = computed<SelectOption[]>(() => {
    return payerType.value === PayerType.COMPANY
      ? companies.value?.items ?? []
      : leads.value?.items ?? []
  })

  const initialValues = computed(() => {
    if (!transfer.value) return defaultInitialValues

    return {
      payerType: transfer.value.payerType,
      payerId: String(transfer.value.payerId),
      amount: transfer.value.amount,
      paymentMethod: String(transfer.value.paymentMethod),
      issueDate: transfer.value.issueDate,
      externalPaymentReferenceNumber: transfer.value.externalPaymentReferenceNumber,
      attachment: undefined,
    }
  })

  function onPayerTypeChange(value: string | number): void {
    payerType.value = value as PayerType
    form.value?.form?.setFieldValue('payerId', '', false)

    if (payerType.value === PayerType.COMPANY) {
      getCompanies(1)

      return
    }

    getLeads(1)
  }

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'payerType',
      as: 'BaseRadioGroup',
      options: payerTypeOptions.value,
      wrapperClass: 'gap-6',
      containerClass: 'w-full mb-2',
      onChange: onPayerTypeChange,
    },
    {
      name: 'payerId',
      as: 'BaseSelect',
      options: payerOptions.value,
      searchable: true,
      label: payerType.value === PayerType.COMPANY ? t('form.chooseCompany') : t('form.chooseLead'),
      labelRequired: true,
      containerClass: 'w-full md:w-[calc(50%-0.4rem)]',
      onScrollToEnd: () => {
        if (payerType.value === PayerType.COMPANY) {
          if (hasMoreCompanies.value) loadMoreCompanies()

          return
        }

        if (hasMoreLeads.value) loadMoreLeads()
      },
      onSearch: (value: string) => {
        if (payerType.value === PayerType.COMPANY) {
          getCompanies(1, value)

          return
        }

        getLeads(1, value)
      },
    },
    {
      name: 'amount',
      as: 'BaseInput',
      type: 'number',
      label: t('form.amount'),
      labelRequired: true,
      containerClass: 'w-full md:w-[calc(50%-0.4rem)]',
    },
    {
      name: 'paymentMethod',
      as: 'BaseSelect',
      options: paymentMethodOptions.value,
      label: t('form.method'),
      labelRequired: true,
      containerClass: 'w-full md:w-[calc(50%-0.4rem)]',
    },
    {
      name: 'issueDate',
      as: 'BaseInput',
      type: 'date',
      label: t('form.issueDate'),
      containerClass: 'w-full md:w-[calc(50%-0.4rem)]',
    },
    {
      name: 'externalPaymentReferenceNumber',
      as: 'BaseInput',
      label: t('form.externalPaymentReferenceNumber'),
      containerClass: 'w-full',
    },
    {
      name: 'attachment',
      as: 'BaseUploader',
      label: t('content.uploadAttachment'),
      labelRequired: !isEditMode.value,
      class: 'w-full',
      containerClass: 'w-full',
      fileUrl: transfer.value?.fileUrl,
      clearFileUrl: () => {
        if (!transfer.value) return

        transfer.value.fileUrl = null
      },
    },
  ])

  const validationSchema = computed(() => object({
    payerType: string().required(),
    payerId: string().required(),
    amount: number().required().moreThan(0).typeError(t('validation.required', { field: t('form.amount') })),
    paymentMethod: string().required(),
    issueDate: string().optional(),
    externalPaymentReferenceNumber: string().optional(),
    attachment: isEditMode.value ? mixed<File>().optional() : mixed<File>().required(),
  }))

  function resetFormState(): void {
    form.value?.form?.resetForm()
    payerType.value = PayerType.COMPANY
    errorMessage.value = null
    formErrors.value = undefined
    if (transfer.value) transfer.value.fileUrl = null
  }

  return {
    errorMessage,
    fieldsSchema,
    form,
    formErrors,
    initialValues,
    isEditMode,
    payerType,
    resetFormState,
    validationSchema,
  }
}
