<template>
  <LazyPopupSuccess
    v-if="isSuccess"
    :model-value="isSuccess"
    @update:model-value="onSuccess"
  >
    <template #message>
      <p class="text-center text-2xl font-bold text-neutral-10">
        {{ $t('success.notificationSent') }}
      </p>
    </template>
  </LazyPopupSuccess>

  <BaseAlert
    v-if="errorMessage"
    :model-value="!!errorMessage"
    :message="errorMessage"
    variant="error"
    class="mb-4 w-full"
  />

  <BaseForm
    ref="formRef"
    :validation-schema
    :initial-values
    :groups="fieldsGroup"
    :errors="formErrors"
    @submit="onSubmit"
  >
    <template #submit>
      <BaseActions
        container-class="flex items-center justify-end gap-3 sm:gap-6"
        :prev-button-options
        :next-button-options="isViewMode ? undefined : nextButtonOptions"
        @prev="router.push({ name: 'NotificationsList' })"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useBroadcastNotificationWizard } from '../composables/useBroadcastNotificationWizard'

  const LazyPopupSuccess = defineAsyncComponent(() => import('@/components/popup/PopupSuccess.vue'))

  const router = useRouter()
  const { t } = useI18n()

  const {
    broadcastNotification,
    errorMessage,
    fieldsGroup,
    formErrors,
    formRef,
    getNotification,
    initLookups,
    initialValues,
    isLoading,
    isSuccess,
    isViewMode,
    onSubmit,
    validationSchema,
  } = useBroadcastNotificationWizard()

  const prevButtonOptions = computed(() => ({
    label: isViewMode.value ? t('button.back') : t('button.discard'),
    class: 'btn-secondary h-13.5 w-1/2 sm:w-52.5',
  }))

  const nextButtonOptions = computed(() => ({
    asSubmit: true,
    class: 'w-1/2 py-3.75 btn-primary h-13.5 sm:w-75',
    label: t('button.sendNotification'),
    disabled: isLoading.value,
  }))

  function onSuccess() {
    isSuccess.value = false
    router.push({ name: 'NotificationsList' })
  }

  onMounted(async () => {
    if (isViewMode.value) {
      await getNotification()

      return
    }

    await initLookups()
  })

  onUnmounted(() => {
    broadcastNotification.value = null
  })
</script>

<style scoped>

</style>
