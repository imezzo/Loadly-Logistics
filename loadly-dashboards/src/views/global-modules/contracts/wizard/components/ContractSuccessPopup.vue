<template>
  <PopupSuccess
    v-model="model"
  >
    <template #message>
      <p class="font-bold">
        {{ message || $t('success.contractCreated') }}
      </p>

      <button
        type="button"
        class="btn-primary mt-4 w-full"
        @click="back"
      >
        {{ $t('button.close') }}
      </button>
    </template>
  </PopupSuccess>
</template>

<script setup lang="ts">
  defineProps<{
    message?: string | null
  }>()

  defineEmits<{
    continue: []
  }>()
  const router = useRouter()
  const model = defineModel<boolean>('modelValue', { required: true })

  function back(): void {
    model.value = false
    router.push({name: 'ContractsList'})
  }
</script>

<style scoped>

</style>
