<template>
  <div class="page-content">
    <TaxesHeader @open-wizard="openWizard" />

    <TaxesFilters class="mb-4" />

    <TaxesTable
      @edit="openWizard"
      @delete="openDeletePopup"
    />

    <TaxesWizard
      v-if="isWizardPopupOpen"
      v-model="isWizardPopupOpen"
      :tax="selectedTax"
    />

    <PopupDelete
      v-if="isDeletePopupOpen"
      v-model="isDeletePopupOpen"
      :title="$t('error.deleteTax')"
      :message="$t('error.deleteTaxConfirmation', { name: selectedTax?.name })"
      @confirm="deleteTax"
    />
  </div>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useTaxSettingsStore } from '@/stores/admin-dashboard/tax-settings'
  import type { Tax } from '@/types/admin-dashboard/response/tax/tax'
  import TaxesHeader from './components/TaxesHeader.vue'
  import TaxesFilters from './components/TaxesFilters.vue'
  import TaxesTable from './components/TaxesTable.vue'
  import { useList } from './composables/useList'

  const TaxesWizard = defineAsyncComponent(() => import('./components/TaxesWizard.vue'))
  const PopupDelete = defineAsyncComponent(() => import('@/components/popup/PopupDelete.vue'))

  const { fetchTaxes } = useList()
  const { setLoading: setDeleting } = useLoading()

  const taxSettingsStore = useTaxSettingsStore()

  const isWizardPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedTax = ref<Tax | null>(null)

  function resetTax(): void {
    selectedTax.value = null
  }

  function openWizard(tax?: Tax): void {
    selectedTax.value = tax ?? null
    isWizardPopupOpen.value = true
  }

  function openDeletePopup(tax: Tax): void {
    selectedTax.value = tax
    isDeletePopupOpen.value = true
  }

  async function deleteTax(): Promise<void> {
    if (!selectedTax.value) return

    setDeleting(true)
    const { success } = await taxSettingsStore.deleteTax(selectedTax.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      resetTax()
      fetchTaxes()
    }
  }

  watch([isWizardPopupOpen, isDeletePopupOpen], ([wizardOpen, deleteOpen]) => {
    if (!wizardOpen && !deleteOpen) resetTax()
  })

  onMounted(fetchTaxes)
</script>

<style scoped>

</style>
