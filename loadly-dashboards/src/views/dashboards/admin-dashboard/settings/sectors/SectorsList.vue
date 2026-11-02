<template>
  <div class="page-content">
    <SectorsHeader @open-wizard="openWizard" />

    <SectorsTable
      @edit="openWizard"
      @delete="openDeletePopup"
    />

    <SectorsWizard
      v-if="isWizardPopupOpen"
      v-model="isWizardPopupOpen"
      :sector="selectedSector"
    />

    <PopupDelete
      v-if="isDeletePopupOpen"
      v-model="isDeletePopupOpen"
      :title="$t('error.deleteSector')"
      :message="$t('error.deleteSectorConfirmation', { name: selectedSector?.name })"
      @confirm="deleteSector"
    />
  </div>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useSectorSettingsStore } from '@/stores/admin-dashboard/sector-settings'
  import type { Sector } from '@/types/admin-dashboard/response/sector/sector'
  import SectorsHeader from './components/SectorsHeader.vue'
  import SectorsTable from './components/SectorsTable.vue'
  import { useList } from './composables/useList'

  const SectorsWizard = defineAsyncComponent(() => import('./components/SectorsWizard.vue'))
  const PopupDelete = defineAsyncComponent(() => import('@/components/popup/PopupDelete.vue'))

  const { fetchSectors } = useList()
  const { setLoading: setDeleting } = useLoading()

  const sectorSettingsStore = useSectorSettingsStore()

  const isWizardPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedSector = ref<Sector | null>(null)

  function resetSector(): void {
    selectedSector.value = null
  }

  function openWizard(sector?: Sector): void {
    selectedSector.value = sector ?? null
    isWizardPopupOpen.value = true
  }

  function openDeletePopup(sector: Sector): void {
    selectedSector.value = sector
    isDeletePopupOpen.value = true
  }

  async function deleteSector(): Promise<void> {
    if (!selectedSector.value) return

    setDeleting(true)
    const { success } = await sectorSettingsStore.deleteSector(selectedSector.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      resetSector()
      fetchSectors()
    }
  }

  watch(isWizardPopupOpen, (newValue) => {
    if (!newValue) resetSector()
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) resetSector()
  })

  onMounted(fetchSectors)
</script>

<style scoped>

</style>
