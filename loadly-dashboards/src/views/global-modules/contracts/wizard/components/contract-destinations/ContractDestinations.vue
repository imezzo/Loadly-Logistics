<template>
  <div class="flex flex-col">
    <NavigationTabs
      v-model:active-tab="activeTab"
      :tabs
      class="mb-4"
    />

    <ContractDestinationsForm
      v-show="activeTab === 0"
      ref="stepRef"
      :errors="props.errors"
      @update="onUpdate"
      @next="onNext"
      @prev="onPrev"
      @discard="onDiscard"
    />

    <div
      v-if="isAdminDashboard && activeTab === 1"
      class="flex flex-col"
    >
      <ContractDestinationsSheet />

      <BaseActions
        class="mt-4"
        :prev-button-options="{ hasScrollTop: true }"
        :next-button-options="{ label: saveButtonTitle, hasScrollTop: true }"
        @prev="onPrev"
        @next="onSheetTabNext"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import NavigationTabs from '@/components/navigation/NavigationTabs.vue'
  import ContractDestinationsForm from './ContractDestinationsForm.vue'
  import { useContractWizard } from '../../composables/useContractWizard'
  import { useApp } from '@/composables/useApp'

  const ContractDestinationsSheet = defineAsyncComponent(() => import('./ContractDestinationsSheet.vue'))

  const props = defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    next: []
    prev: []
    discard: []
  }>()

  const { t } = useI18n()
  const { saveButtonTitle } = useContractWizard()
  const { isAdminDashboard } = useApp()
  const activeTab = ref(0)

  const tabs = computed(() => [
    { key: 'add', title: t('content.addNewDestination') },
    ...(isAdminDashboard ? [{ key: 'all', title: t('content.allDestinations') }] : []),
  ])

  const stepRef = useTemplateRef<InstanceType<typeof ContractDestinationsForm>>('stepRef')

  function onSheetTabNext(): void {
    stepRef.value?.submitStep()
  }

  function onUpdate(data: Record<string, unknown>): void {
    emit('update', data)
  }

  function onNext(): void {
    emit('next')
  }

  function onPrev(): void {
    emit('prev')
  }

  function onDiscard(): void {
    emit('discard')
  }

  defineExpose({
    submitStep: () => stepRef.value?.submitStep(),
  })
</script>

<style scoped>

</style>
