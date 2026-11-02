<template>
  <button
    type="button"
    class="btn-success"
    @click="showPopup = true"
  >
    {{ $t('button.reactivate') }}
  </button>

  <ActionPopupReactivate
    v-model="showPopup"
    :contract-id="contractId"
    @discard="showPopup = false"
    @success="emit('success')"
  />
</template>

<script setup lang="ts">
  const route = useRoute()

  const emit = defineEmits<{ success: [] }>()

  const ActionPopupReactivate = defineAsyncComponent(
    () => import('./action-popup/ActionPopupReactivate.vue')
  )

  const contractId = computed(() => route.params.id as string)
  const showPopup = ref(false)
</script>

<style scoped>

</style>
