<template>
  <UsersWizardSuccessPopup
    v-if="isSuccess"
    :model-value="isSuccess"
    :is-edit-mode
    :user
    @success="onSuccess"
  />

  <UsersWizardDeactivatePopup
    v-if="isEditMode"
    v-model="isDeactivateConfirmNeeded"
    :user
    @confirm="confirmDeactivation"
  />

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
    :groups="fieldsGroup"
    :errors="formErrors"
    @submit="onSubmit"
  >
    <template #submit="{ meta }">
      <BaseActions
        container-class="flex items-center justify-end gap-3 sm:gap-6"
        :prev-button-options="prevButtonOptions"
        :next-button-options="isViewMode
          ? undefined
          : {
            ...nextButtonOptions,
            disabled: isLoading || (isEditMode && !meta.dirty)
          }"
        @prev="router.push({ name: listRouteName ?? 'UsersList' })"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { UserStatus } from '@/types/user'
  import { useUserWizardForm } from '@/composables/modules/users/useUserWizardForm'

  const UsersWizardSuccessPopup = defineAsyncComponent(() => import('@/components/users/wizard/UsersWizardSuccessPopup.vue'))
  const UsersWizardDeactivatePopup = defineAsyncComponent(() => import('@/components/users/wizard/UsersWizardDeactivatePopup.vue'))

  const props = defineProps<{
    listRouteName?: string
  }>()

  const router = useRouter()
  const {
    errorMessage,
    fieldsGroup,
    formErrors,
    getUser,
    initialValues,
    isDeactivateConfirmNeeded,
    isDeactivateConfirmed,
    isEditMode,
    isViewMode,
    isLoading,
    isSuccess,
    onSubmit,
    user,
    validationSchema,
  } = useUserWizardForm()
  const { t } = useI18n()

  const form = useTemplateRef('form')

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'flex h-13.5 w-1/2 items-center justify-center gap-2 rounded-2xl bg-black/15'
    + 'px-4 py-4.25 font-medium text-content-secondary transition-colors hover:bg-black/20 sm:w-52.5',
  }))

  const isReactivate = computed(() => isEditMode.value && user.value?.status === UserStatus.DISABLED)

  const saveButtonLabel = computed(() => {
    if (isReactivate.value) return t('button.reactivate')

    return isEditMode.value
      ? t('button.saveChanges')
      : t('button.createUser')
  })

  const nextButtonOptions = computed(() => ({
    asSubmit: true,
    class: `w-1/2 py-3.75 btn-primary h-13.5 sm:w-75 ${
      isReactivate.value
        ? 'bg-success-500 hover:bg-success-500/90'
        : 'bg-primary hover:bg-primary/90'
    }`,
    label: isLoading.value && !isEditMode.value
      ? t('form.creating')
      : saveButtonLabel.value,
  }))

  function confirmDeactivation() {
    isDeactivateConfirmNeeded.value = false
    isDeactivateConfirmed.value = true
    onSubmit(form.value?.form?.values as Record<string, unknown>)
  }

  function onSuccess() {
    isSuccess.value = false
    router.push({ name: props.listRouteName ?? 'UsersList' })
  }

  onMounted(() => {
    if (isEditMode.value || isViewMode.value) {
      getUser()
    }
  })
</script>

<style scoped>

</style>
