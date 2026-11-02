<template>
  <BasePopup v-model="model">
    <div class="w-full p-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ popupTitle }}
      </h2>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-4"
      />

      <BaseForm
        ref="form"
        :validation-schema
        :initial-values
        :fields-schema
        :errors="formErrors"
        container-class="grid grid-cols-2 gap-4"
        @submit="submit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-center gap-3"
            :prev-button-options
            :next-button-options
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useLoading } from '@/composables/useLoading'
  import { useDiscountSettingsStore } from '@/stores/admin-dashboard/discount-settings'
  import type { Discount } from '@/types/admin-dashboard/response/discount/discount'
  import type { DiscountWizardRequest } from '@/types/admin-dashboard/request/discount-settings'
  import type { ApiResponse } from '@/types/admin-dashboard/response/response'
  import { useList } from '../composables/useList'
  import { useDiscountsWizard } from '../composables/useDiscountsWizard'

  const props = defineProps<{
    discount: Discount | null
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const form = useTemplateRef('form')

  useVeeValidate()
  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const { fetchDiscounts } = useList()
  const { validationSchema, fieldsSchema, initialValues } = useDiscountsWizard(
    computed(() => props.discount)
  )

  const discountSettingsStore = useDiscountSettingsStore()

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const popupTitle = computed(() => (
    props.discount ? t('title.editDiscount') : t('title.addDiscount')
  ))

  const prevButtonOptions = computed(() => ({
    class: 'btn-secondary w-52.5 h-13.5',
    label: t('button.discard'),
  }))

  const nextButtonOptions = computed(() => ({
    asSubmit: true,
    class: 'btn-primary w-52.5 h-13.5',
    disabled: isLoading.value,
    label: t('button.saveChanges'),
  }))

  function buildDiscountRequest(values: Record<string, unknown>): DiscountWizardRequest {
    return {
      nameAr: String(values.nameAr),
      nameEn: String(values.nameEn),
      type: Number(values.type),
      value: Number(values.value),
      status: Number(values.status),
    }
  }

  async function submit(values: Record<string, unknown>): Promise<void> {
    const request = buildDiscountRequest(values)

    setLoading(true)
    const response = props.discount
      ? await discountSettingsStore.update(props.discount.id, request)
      : await discountSettingsStore.create(request)
    setLoading(false)

    handleSubmitResponse(response)
  }

  function handleSubmitResponse(response: ApiResponse<Discount | null>): void {
    if (response.success) {
      discard()
      fetchDiscounts()

      return
    }

    errorMessage.value = response.message || t('error.networkError')
    formErrors.value = response.errors || {}
  }

  function discard(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    model.value = false
  }
</script>

<style scoped>

</style>
