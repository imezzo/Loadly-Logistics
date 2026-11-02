<template>
  <div class="page-content">
    <LeadsHeader @open-wizard="openWizard" />

    <LeadsTable
      @edit="openWizard"
      @delete="openDeletePopup"
    />

    <LeadsWizard
      v-if="isWizardPopupOpen"
      v-model="isWizardPopupOpen"
      :lead="selectedLead"
    />

    <PopupDelete
      v-if="isDeletePopupOpen"
      v-model="isDeletePopupOpen"
      :title="$t('error.deleteLead')"
      :message="$t('error.deleteLeadConfirmation', { name: selectedLead?.fullName })"
      @confirm="deleteLead"
    />
  </div>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useLeadSettingsStore } from '@/stores/admin-dashboard/lead-settings'
  import type { Lead } from '@/types/admin-dashboard/response/lead/lead'
  import LeadsHeader from './components/LeadsHeader.vue'
  import LeadsTable from './components/LeadsTable.vue'
  import { useList } from './composables/useList'

  const LeadsWizard = defineAsyncComponent(() => import('./components/LeadsWizard.vue'))
  const PopupDelete = defineAsyncComponent(() => import('@/components/popup/PopupDelete.vue'))

  const { fetchLeads } = useList()
  const { setLoading: setDeleting } = useLoading()

  const leadSettingsStore = useLeadSettingsStore()

  const isWizardPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedLead = ref<Lead | null>(null)

  function resetLead(): void {
    selectedLead.value = null
  }

  function openWizard(lead?: Lead): void {
    selectedLead.value = lead ?? null
    isWizardPopupOpen.value = true
  }

  function openDeletePopup(lead: Lead): void {
    selectedLead.value = lead
    isDeletePopupOpen.value = true
  }

  async function deleteLead(): Promise<void> {
    if (!selectedLead.value) return

    setDeleting(true)
    const { success } = await leadSettingsStore.deleteLead(selectedLead.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      resetLead()
      fetchLeads()
    }
  }

  watch([isDeletePopupOpen, isWizardPopupOpen], ([deletePopupOpen, wizardPopupOpen]) => {
    if (!deletePopupOpen && !wizardPopupOpen) resetLead()
  })

  onMounted(fetchLeads)
</script>

<style scoped>

</style>
