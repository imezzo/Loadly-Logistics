<template>
  <button
    type="button"
    class="btn-success-outline"
    @click="showPopup = true"
  >
    {{ $t('button.approveContract') }}
  </button>

  <ActionPopupConfirm
    v-model="showPopup"
    :title="$t('content.approveContractTitle')"
    :subtitle="$t('content.giveApproveComments')"
    :reason-label="$t('content.comments')"
    :discard-button-text="$t('button.discard')"
    :submit-button-text="$t('button.sendToAdmin')"
    :error-message="errorMessage"
    :error-form-errors="errorFormErrors"
    @discard="showPopup = false"
    @submit="onApprove"
  />
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useContractStore } from '@/stores/admin-dashboard/contract'

  const ActionPopupConfirm = defineAsyncComponent(
    () => import('./action-popup/ActionPopupConfirm.vue')
  )

  const emit = defineEmits<{
    success: []
  }>()

  const route = useRoute()
  const { setLoading } = useLoading()
  const contractStore = useContractStore()

  const contractId = computed(() => route.params.id as string)
  const showPopup = ref(false)
  const errorMessage = ref<string | null>(null)
  const errorFormErrors = ref<Record<string, string[]>>({})

  function resetFormErrors(): void {
    errorMessage.value = null
    errorFormErrors.value = {}
  }

  function onFailAction(message?: string, errors?: string[]): void {
    errorMessage.value = message ?? null
    errorFormErrors.value = errors ? { reason: errors } : {}
  }

  async function onApprove(comments: string): Promise<void> {
    if (!contractId.value) return
    resetFormErrors()

    setLoading(true)
    const { success, message, errors } = await contractStore.submitDecision(contractId.value, {
      Action: 1,
      Comments: comments,
    })
    setLoading(false)

    if (!success) {
      onFailAction(message, errors?.Comments)
    } else {
      showPopup.value = false
      emit('success')
    }
  }

  watch(showPopup, (visible) => {
    if (!visible) resetFormErrors()
  })
</script>

<style scoped>

</style>
