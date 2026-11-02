<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('content.secondaryContacts') }}
      </h2>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-4 w-full"
      />

      <BaseForm
        ref="form"
        :validation-schema
        :initial-values
        :groups="fieldsGroup"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 gap-3"
            :prev-button-options="{ label: t('button.cancel') }"
            :next-button-options
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { object, string } from 'yup'
  import type { FieldsGroup } from '@/types/form'
  import type { SecondaryContact } from '@/types/secondary-contact.types'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useLookups } from '@/composables/useLookups'
  import { createEmailSchema } from '@/utils/validation/email'
  import { createPhoneSchema } from '@/utils/validation/phone'

  const emit = defineEmits<{
    add: [contact: SecondaryContact]
  }>()

  useVeeValidate()

  const { t } = useI18n()
  const { communicationMethods } = useLookups()

  const model = defineModel<boolean>('modelValue', { required: true })

  const form = useTemplateRef('form')
  const errorMessage = ref<string | null>(null)

  const nextButtonOptions = computed(() => ({
    asSubmit: true,
    class: 'flex h-13.5 items-center justify-center rounded-2xl bg-primary px-6 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary/90 md:w-52.5',
    label: t('button.addContact'),
  }))

  const initialValues = {
    fullName: '',
    phone: '',
    email: '',
    position: '',
    communicationMethod: '',
  }

  const validationSchema = computed(() => object({
    fullName: string().required(),
    phone: createPhoneSchema(t, true),
    email: createEmailSchema(t),
    position: string().required(),
    communicationMethod: string().required(),
  }))

  const fieldsGroup = computed<Record<string, FieldsGroup>>(() => ({
    personalInformation: {
      title: t('title.personalInformation'),
      description: t('form.editYourPersonalInformation'),
      class: 'grid grid-cols-1 gap-4 md:grid-cols-2',
      wrapperClass: 'space-y-3',
      fields: [
        {
          name: 'fullName',
          as: 'BaseInput',
          type: 'text',
          placeholder: t('form.fullName'),
          label: t('form.fullName'),
          labelRequired: true,
        },
        {
          name: 'phone',
          as: 'BaseInputPhone',
          type: 'tel',
          placeholder: t('form.phonePlaceholder'),
          label: t('form.phoneNumber'),
          labelRequired: true,
          allowedCountryCode: '+20',
        },
        {
          name: 'email',
          as: 'BaseInput',
          type: 'email',
          placeholder: t('form.email'),
          label: t('form.email'),
          labelRequired: true,
        },
        {
          name: 'position',
          as: 'BaseInput',
          type: 'text',
          placeholder: t('form.position'),
          label: t('form.position'),
          labelRequired: true,
        },
        {
          name: 'communicationMethod',
          as: 'BaseSelect',
          options: communicationMethods.value,
          placeholder: t('form.communicationMethod'),
          label: t('form.communicationMethod'),
          labelRequired: true,
        },
      ],
    },
  }))

  function onSubmit(values: Record<string, unknown>): void {
    errorMessage.value = null

    emit('add', {
      name: values.fullName as string,
      phone: values.phone as string,
      email: values.email as string,
      position: values.position as string,
      communicationMethod: values.communicationMethod as string,
    })

    form.value?.form?.resetForm()
    model.value = false
  }

  function discard(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    model.value = false
  }

  watch(model, (newValue) => {
    if (!newValue && form.value) {
      form.value.form?.resetForm()
      errorMessage.value = null
    }
  })
</script>

<style scoped>

</style>
