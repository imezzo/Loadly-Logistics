<template>
  <div class="page-content">
    <DiscountsHeader @open-wizard="openWizard" />

    <DiscountsFilters class="mb-4" />

    <DiscountsTable
      @edit="openWizard"
      @delete="openDeletePopup"
    />

    <DiscountsWizard
      v-if="isWizardPopupOpen"
      v-model="isWizardPopupOpen"
      :discount="selectedDiscount"
    />

    <PopupDelete
      v-if="isDeletePopupOpen"
      v-model="isDeletePopupOpen"
      :title="$t('error.deleteDiscount')"
      :message="$t('error.deleteDiscountConfirmation', { name: selectedDiscount?.name })"
      @confirm="deleteDiscount"
    />
  </div>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useDiscountSettingsStore } from '@/stores/admin-dashboard/discount-settings'
  import type { Discount } from '@/types/admin-dashboard/response/discount/discount'
  import DiscountsHeader from './components/DiscountsHeader.vue'
  import DiscountsFilters from './components/DiscountsFilters.vue'
  import DiscountsTable from './components/DiscountsTable.vue'
  import { useList } from './composables/useList'

  const DiscountsWizard = defineAsyncComponent(() => import('./components/DiscountsWizard.vue'))
  const PopupDelete = defineAsyncComponent(() => import('@/components/popup/PopupDelete.vue'))

  const { fetchDiscounts } = useList()
  const { setLoading: setDeleting } = useLoading()

  const discountSettingsStore = useDiscountSettingsStore()

  const isWizardPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedDiscount = ref<Discount | null>(null)

  function resetDiscount(): void {
    selectedDiscount.value = null
  }

  function openWizard(discount?: Discount): void {
    selectedDiscount.value = discount ?? null
    isWizardPopupOpen.value = true
  }

  function openDeletePopup(discount: Discount): void {
    selectedDiscount.value = discount
    isDeletePopupOpen.value = true
  }

  async function deleteDiscount(): Promise<void> {
    if (!selectedDiscount.value) return

    setDeleting(true)
    const { success } = await discountSettingsStore.deleteDiscount(selectedDiscount.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      resetDiscount()
      fetchDiscounts()
    }
  }

  watch([isWizardPopupOpen, isDeletePopupOpen], ([wizardOpen, deleteOpen]) => {
    if (!wizardOpen && !deleteOpen) resetDiscount()
  })

  onMounted(fetchDiscounts)
</script>

<style scoped>

</style>
