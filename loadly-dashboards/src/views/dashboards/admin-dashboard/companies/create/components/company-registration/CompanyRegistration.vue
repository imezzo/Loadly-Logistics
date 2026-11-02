<template>
  <BaseForm
    ref="companyForm"
    :initial-values="initialValues"
    :validation-schema="validationSchema"
    :fields-schema="fieldsSchema"
    :errors="props.errors"
    container-class="flex flex-wrap justify-between"
    @submit="onSubmit"
  >
    <template #field-taxId>
      <hr class="mt-6 mb-4 border-t border-neutral-200">
      <h4 class="mb-4 font-bold text-neutral-10">
        {{ t('content.mainContactPerson') }}
      </h4>
    </template>

    <template #field-phone>
      <div class="mt-6 w-full">
        <SecondaryContactsHeader @add-contact="isAddContactPopupOpen = true" />

        <SecondaryContactsList
          :contacts="secondaryContacts"
          @remove="removeSecondaryContact"
        />

        <hr class="mb-4 border-t border-neutral-200">
        <h4 class="mb-4 font-bold text-neutral-10">
          {{ t('content.mainBranchCompany') }}
        </h4>
      </div>
    </template>

    <AddSecondaryContactPopup
      v-model="isAddContactPopupOpen"
      @add="addSecondaryContact"
    />

    <template #submit>
      <BaseActions
        :next-button-options="{ asSubmit: true, hasScrollTop: true }"
        @prev="emit('prev')"
        @next="emit('next')"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import type { SecondaryContact } from '@/types/secondary-contact.types'
  import { useCompanyRegistration } from '../../composables/useCompanyRegistration'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useLookups } from '@/composables/useLookups'

  const AddSecondaryContactPopup = defineAsyncComponent(() => import('@/components/secondary-contact/AddSecondaryContactPopup.vue'))

  interface CompanyForm {
    form: {
      setFieldValue: (field: string, value: unknown) => void
      values: Record<string, unknown>
    }
  }

  const props = defineProps<{
    formValues?: Record<string, unknown>
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
    next: []
  }>()

  useVeeValidate()
  const { t } = useI18n()
  const {
    getCities,
  } = useLookups()
  const {
    initialValues,
    validationSchema,
    fieldsSchema,
    fetchDistricts,
  } = useCompanyRegistration(computed(() => props.formValues))

  onMounted(() => {
    fetchDistricts()
  })

  const companyFormRef = useTemplateRef<CompanyForm>('companyForm')
  const companyFormValues = computed<Record<string, unknown> | undefined>(
    () => companyFormRef.value?.form?.values)

  watch(() => companyFormValues.value?.governorate as string | undefined, (newValue, oldValue) => {
    if (newValue
      && newValue !== oldValue
      && oldValue !== undefined) {
      onGovernorateChange(newValue)
    }
  })

  watch(() => companyFormValues.value?.city as string | undefined, async (newValue, oldValue) => {
    if (newValue !== oldValue && oldValue !== undefined) {
      setFieldValue('district', '')
    }
    if (newValue) {
      await fetchDistricts(Number(newValue))
    }
  }, { immediate: true })

  const isAddContactPopupOpen = ref(false)

  const secondaryContacts = ref<SecondaryContact[]>(props.formValues?.secondaryContacts as SecondaryContact[] || [])

  function addSecondaryContact(contact: SecondaryContact): void {
    secondaryContacts.value.push(contact)
  }

  function removeSecondaryContact(index: number): void {
    secondaryContacts.value.splice(index, 1)
  }

  function onSubmit(values: Record<string, unknown>): void {
    emit('update', {
      ...values,
      secondaryContacts: secondaryContacts.value,
    })
    emit('next')
  }

  function setFieldValue(field: string, value: unknown): void {
    companyFormRef.value?.form?.setFieldValue(field, value)
  }

  async function onGovernorateChange(value: string) {
    if (!value) return

    const id = parseInt(value, 10)
    await getCities(id)

    setFieldValue('city', '')
    setFieldValue('district', '')
  }
</script>

<style scoped>

</style>
