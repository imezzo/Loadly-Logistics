import { mixed, number, object, string } from 'yup'
import type { Schema } from '@/types/form'
import type { SelectOption } from '@/types/ui'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useInvoiceStore } from '@/stores/financial/invoice'
import { useLoading } from '@/composables/useLoading'
import { useEnum } from '@/composables/useEnum'

interface UseCreatePaymentRequestPopupOptions {
  invoiceId: Ref<number>
  onSuccess: () => void
  onClose: () => void
}

interface BaseFormExpose {
  form?: {
    resetForm: () => void
  }
}

export function useCreatePaymentRequestPopup(options: UseCreatePaymentRequestPopupOptions) {
  useVeeValidate()

  const { t } = useI18n()
  const { enums } = useEnum()
  const invoiceStore = useInvoiceStore()
  const { isLoading, setLoading } = useLoading()
  const form = useTemplateRef<BaseFormExpose | null>('form')

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const paymentMethodOptions = computed<SelectOption[]>(() => enums.value?.PaymentMethod ?? [])

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'flex h-13.5 w-52.5 items-center justify-center rounded-2xl bg-black/15 px-6 font-medium text-content-secondary transition-colors hover:bg-black/20',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.create'),
    class: 'flex h-13.5 w-52.5 items-center justify-center rounded-2xl bg-primary px-6 font-medium text-white transition-colors hover:bg-primary/90',
    disabled: isLoading.value,
    asSubmit: true,
  }))

  const initialValues = {
    amount: '',
    paymentMethod: '',
    requestDate: '',
    attachment: undefined,
    notes: '',
  }

  const fieldsSchema = computed<Schema[]>(() => [
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
      label: t('table.paymentMethod'),
      labelRequired: true,
      containerClass: 'w-full md:w-[calc(50%-0.4rem)]',
    },
    {
      name: 'requestDate',
      as: 'BaseInput',
      type: 'date',
      label: t('table.requestDate'),
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'attachment',
      as: 'BaseUploader',
      label: t('content.uploadAttachment'),
      labelRequired: true,
      class: 'w-full',
      containerClass: 'w-full',
    },
    {
      name: 'notes',
      as: 'BaseTextarea',
      label: t('content.notes'),
      containerClass: 'w-full',
      rows: 3,
    },
  ])

  const validationSchema = computed(() => object({
    amount: number().required().moreThan(0),
    paymentMethod: string().required(),
    requestDate: string().required(),
    attachment: mixed<File>().required(),
    notes: string().optional(),
  }))

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    errorMessage.value = null
    setLoading(true)

    const requestDateValue = String(values.requestDate ?? '')
    const requestDate = requestDateValue ? new Date(requestDateValue).toISOString() : ''

    const { success, message, errors } = await invoiceStore.createPaymentRequest({
      invoiceId: options.invoiceId.value,
      amount: Number(values.amount ?? 0),
      requestDate,
      notes: String(values.notes ?? ''),
      paymentMethod: Number(values.paymentMethod ?? 0),
      attachment: values.attachment as File,
    })

    setLoading(false)

    if (success) {
      form.value?.form?.resetForm()
      options.onClose()
      options.onSuccess()

      return
    }

    errorMessage.value = message || t('error.networkError')
    formErrors.value = errors || {}
  }

  function discard(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    options.onClose()
  }

  return {
    errorMessage,
    fieldsSchema,
    form,
    formErrors,
    initialValues,
    nextButtonOptions,
    onSubmit,
    prevButtonOptions,
    validationSchema,
    discard,
  }
}

