<template>
  <PopupSuccess
    v-model="model"
    :message="message"
  >
    <template #actions>
      <div class="mt-6 flex flex-wrap items-center justify-center gap-3">
        <button
          type="button"
          class="btn-secondary h-13.5"
          @click="close"
        >
          {{ $t('button.stayInContract') }}
        </button>

        <RouterLink
          :to="{ name: 'ContractsList' }"
          class="btn-primary h-13.5"
          @click="close"
        >
          {{ $t('button.backToContractList') }}
        </RouterLink>
      </div>
    </template>
  </PopupSuccess>
</template>

<script setup lang="ts">
  import PopupSuccess from '@/components/popup/PopupSuccess.vue'

  defineProps<{
    message?: string
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  function close(): void {
    model.value = false
  }
</script>

<style scoped>

</style>
