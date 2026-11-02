<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('title.addNewRole') }}
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
        :validation-schema="validationSchema"
        :initial-values="initialValues"
        :fields-schema="fieldsSchema"
        container-class="grid grid-cols-2 gap-6"
        :errors="formErrors"
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
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useRoleStore } from '@/stores/role'
  import { useLoading } from '@/composables/useLoading'

  useVeeValidate()

  const { t } = useI18n()

  const emit = defineEmits<{
    refresh: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const form = useTemplateRef('form')
  const roleStore = useRoleStore()
  const { isLoading, setLoading } = useLoading()

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const prevButtonOptions = computed(() => ({
    class: ['flex h-13.5 w-52.5 items-center justify-center rounded-2xl',
      'bg-black/15 px-6 font-medium text-content-secondary',
      'transition-colors hover:bg-black/20'].join(' '),
    label: t('button.discard'),
  }))

  const nextButtonOptions = computed(() => ({
    asSubmit: true,
    class: 'h-13.5 w-52.5 btn-primary',
    disabled: isLoading.value,
    label: t('button.addNewRole'),
  }))

  const initialValues = {
    nameEn: '',
    nameAr: '',
    description: '',
  }

  const validationSchema = computed(() => object({
    nameEn: string()
      .required()
      .max(255),
    nameAr: string()
      .required()
      .max(255),
    description: string()
      .required()
      .max(500)
      .min(10),
  }))

  const fieldsSchema = computed(() => [
    {
      name: 'nameEn',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.nameEn'),
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
      name: 'description',
      as: 'BaseTextarea',
      wrapperClass: '[&>.input-container]:min-h-32',
      placeholder: t('form.roleDescription'),
      label: t('form.roleDescription'),
      labelRequired: true,
      maxlength: 500,
      showCounter: true,
      containerClass: 'col-span-full',
    },
  ])

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    errorMessage.value = null
    setLoading(true)

    const { success, message, errors } = await roleStore.create({
      nameEn: values.nameEn as string,
      nameAr: values.nameAr as string,
      guardName: 'admin',
      description: values.description as string,
    })

    if (success) {
      form.value?.form?.resetForm()
      model.value = false
      emit('refresh')
    } else {
      errorMessage.value = message || t('error.networkError')
      formErrors.value = errors || {}
    }

    setLoading(false)
  }

  function discard() {
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

