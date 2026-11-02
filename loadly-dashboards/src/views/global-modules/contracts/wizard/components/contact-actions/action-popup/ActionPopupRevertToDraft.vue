<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-2xl"
  >
    <div class="flex w-full flex-col px-3.25 py-2 text-center">
      <p class="text-2xl font-bold text-neutral-10">
        {{ $t('content.saveAsDraftTitle') }}
      </p>

      <p class="mt-4 text-content-secondary">
        {{ $t('content.pullBackToDraftConfirmation') }}
      </p>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-4 w-full"
      />

      <BaseActions
        container-class="mt-6 flex flex-wrap items-center justify-center gap-3"
        :prev-button-options="{ label: $t('button.cancel') }"
        :next-button-options="nextButtonOptions"
        @prev="model = false"
        @next="onConfirm"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import BasePopup from '@/components/base/BasePopup.vue'
  import { useLoading } from '@/composables/useLoading'
  import { useContractStore } from '@/stores/admin-dashboard/contract'

  const props = defineProps<{
    contractId: string
  }>()

  const { t } = useI18n()
  const model = defineModel<boolean>('modelValue', { required: true })
  const emit = defineEmits<{ success: [] }>()
  const { setLoading } = useLoading()
  const contractStore = useContractStore()

  const errorMessage = ref<string | null>(null)

  async function onConfirm(): Promise<void> {
    if (!props.contractId) return

    errorMessage.value = null
    setLoading(true)
    const { success, message } = await contractStore.saveAsDraft(props.contractId)
    setLoading(false)

    if (!success) {
      errorMessage.value = message ?? t('error.somethingWentWrong')

      return
    }

    emit('success')
    model.value = false
  }

  watch(model, (isOpen) => {
    if (!isOpen) {
      errorMessage.value = null
    }
  })

  const nextButtonOptions = computed(() => ({
    label: t('button.yesSave'),
    class: 'flex h-13.5 items-center justify-center gap-2 rounded-2xl bg-success-500 px-6 py-3 text-sm font-semibold text-white transition-colors hover:bg-success-500/90 px-30',
  }))
</script>

<style scoped>

</style>
