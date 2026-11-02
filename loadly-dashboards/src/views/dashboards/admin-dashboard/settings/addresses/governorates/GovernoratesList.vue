<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <div
      class="
        mb-4 flex flex-col gap-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <h1 class="text-xl font-bold text-neutral-800">
        {{ $t('title.governorates') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addGovernorate') }}

        <IconPlus />
      </button>
    </div>

    <GovernoratesTable
      :governorates="governorates"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editGovernorate"
      @delete="deleteGovernorate"
    />

    <EditGovernoratePopup
      v-model="isEditPopupOpen"
      :governorate="selectedGovernorate"
      @refresh="fetchGovernorates"
    />

    <DeleteGovernoratePopup
      v-model="isDeletePopupOpen"
      :governorate-name="governorateToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateGovernoratePopup
      v-model="isCreatePopupOpen"
      @refresh="fetchGovernorates"
    />
  </div>
</template>

<script setup lang="ts">
  import type { Governorate } from '@/types/admin-dashboard/response/governorate'
  import { Address, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useGovernorateStore } from '@/stores/admin-dashboard/governorate'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import GovernoratesTable from './components/GovernoratesTable.vue'
  import EditGovernoratePopup from './components/EditGovernoratePopup.vue'
  import DeleteGovernoratePopup from './components/DeleteGovernoratePopup.vue'
  import CreateGovernoratePopup from './components/CreateGovernoratePopup.vue'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.Address, Address.CreateGovernerate)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.governorates') },
  ]

  const {
    fetchGovernorates,
    governorates,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const governorateStore = useGovernorateStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedGovernorate = ref<Governorate | null>(null)
  const governorateToDelete = ref<Governorate | null>(null)

  function editGovernorate(governorate: Governorate) {
    selectedGovernorate.value = governorate
    isEditPopupOpen.value = true
  }

  function deleteGovernorate(governorate: Governorate) {
    governorateToDelete.value = governorate
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!governorateToDelete.value) return

    setDeleting(true)
    const { success } = await governorateStore.deleteGovernorate(governorateToDelete.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      governorateToDelete.value = null
      fetchGovernorates()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedGovernorate.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      governorateToDelete.value = null
    }
  })

  onMounted(() => {
    fetchGovernorates()
  })
</script>

<style scoped>

</style>

