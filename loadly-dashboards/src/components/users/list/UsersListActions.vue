<template>
  <section class="mb-4 flex items-center">
    <p class="font-medium text-neutral-10">
      {{ $t('content.youSelected', { count: selectedIds.length }) }}
    </p>

    <BaseActions
      container-class="ms-8 gap-2"
      :next-button-options
      :secondary-button-options
      @next="activate"
      @secondary="deactivate"
    />
  </section>

  <LazyPopupSuccess
    v-if="isActivationSuccess"
    :model-value="isActivationSuccess"
    @update:model-value="reset"
  >
    <template #message>
      {{ activationSuccessMessage }}
    </template>
  </LazyPopupSuccess>

  <LazyPopupError
    v-if="activationError"
    :model-value="!!activationError"
    :error-message="activationError"
    @update:model-value="reset"
  />
</template>

<script setup lang="ts">
  import { useUserStore } from '@/stores/admin-dashboard/user'
  import { useLoading } from '@/composables/useLoading'

  const LazyPopupSuccess = defineAsyncComponent(() => import('@/components/popup/PopupSuccess.vue'))
  const LazyPopupError = defineAsyncComponent(() => import('@/components/popup/PopupError.vue'))

  const props = defineProps<{
    selectedIds: string[]
  }>()

  const emit = defineEmits<{
    reset: []
    fetchUsers: []
  }>()

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()

  const userStore = useUserStore()

  const activationError = ref<string | null>(null)
  const activationType = ref<'activate' | 'deactivate' | null>(null)
  const isActivationSuccess = ref<boolean>(false)

  const activationSuccessMessage = computed(() => {
    return activationType.value === 'activate'
      ? t('success.usersSuccessfullyActivated', { count: props.selectedIds.length })
      : t('success.usersSuccessfullyDeactivated', { count: props.selectedIds.length })
  })
  const rawSelectedIds = computed(() => toRaw(props.selectedIds))

  const nextButtonOptions = computed(() => ({
    label: t('button.activateSelected'),
    class: 'btn-primary',
    disabled: isLoading.value,
  }))

  const secondaryButtonOptions = computed(() => ({
    label: t('button.deactivateSelected'),
    class: 'btn-error-outline',
  }))

  async function onStatusChange(success: boolean, message: string): Promise<void> {
    isActivationSuccess.value = success
    setLoading(false)

    if (success) {
      emit('fetchUsers')

      return
    }
    activationError.value = message
  }

  async function activate(): Promise<void> {
    setLoading(true)
    activationType.value = 'activate'
    const { success, message } = await userStore.activate(rawSelectedIds.value)
    onStatusChange(success, message)
  }

  async function deactivate(): Promise<void> {
    setLoading(true)
    activationType.value = 'deactivate'
    const { success, message } = await userStore.deactivate(rawSelectedIds.value)
    onStatusChange(success, message)
  }

  function reset(): void {
    activationType.value = null
    activationError.value = null
    isActivationSuccess.value = false
    emit('reset')
  }
</script>

<style scoped>

</style>
