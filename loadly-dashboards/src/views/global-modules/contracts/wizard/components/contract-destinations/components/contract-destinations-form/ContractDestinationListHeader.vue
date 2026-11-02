<template>
  <div class="my-4 flex items-center justify-between">
    <h2 class="text-lg font-bold text-neutral-10">
      {{ $t('content.allDestinations') }}
    </h2>

    <div class="flex items-center gap-2">
      <button
        type="button"
        class="btn-primary"
        @click="isImportSheetPopupOpen = true"
      >
        {{ $t('button.import') }}
      </button>

      <button
        v-if="hasPersistedDestinations"
        type="button"
        class="btn-success-outline"
        @click="isBulkUpdateSheetPopupOpen = true"
      >
        {{ $t('button.bulkUpdate') }}
      </button>

      <ExportAction
        v-if="existingDestinations.length"
        :existing-destinations="existingDestinations"
      />
    </div>

    <ImportSheetPopup
      v-if="isImportSheetPopupOpen"
      v-model="isImportSheetPopupOpen"
      :existing-destinations="existingDestinations"
    />

    <BulkUpdateSheetPopup
      v-if="isBulkUpdateSheetPopupOpen"
      v-model="isBulkUpdateSheetPopupOpen"
      :existing-destinations="existingDestinations"
    />
  </div>
</template>

<script setup lang="ts">
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import ExportAction from './contract-destinations-export/ExportAction.vue'

  const props = withDefaults(
    defineProps<{
      existingDestinations?: ContractWizardDestination[]
    }>(),
    { existingDestinations: () => [] },
  )

  const ImportSheetPopup = defineAsyncComponent(
    () => import('./contract-destinations-import/ImportSheetPopup.vue'))
  const BulkUpdateSheetPopup = defineAsyncComponent(
    () => import('./contract-destinations-bulk-update/BulkUpdateSheetPopup.vue'))

  const isImportSheetPopupOpen = ref(false)
  const isBulkUpdateSheetPopupOpen = ref(false)

  const hasPersistedDestinations = computed(() =>
    props.existingDestinations.some(destination => destination.id != null),
  )
</script>

<style scoped>

</style>
