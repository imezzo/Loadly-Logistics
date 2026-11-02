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
        {{ $t('title.districts') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addDistrict') }}

        <IconPlus />
      </button>
    </div>

    <DistrictsTable
      :districts="districts"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editDistrict"
      @delete="deleteDistrict"
    />

    <EditDistrictPopup
      v-model="isEditPopupOpen"
      :district="selectedDistrict"
      @refresh="fetchDistricts"
    />

    <DeleteDistrictPopup
      v-model="isDeletePopupOpen"
      :district-name="districtToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateDistrictPopup
      v-model="isCreatePopupOpen"
      @refresh="fetchDistricts"
    />
  </div>
</template>

<script setup lang="ts">
  import type { District } from '@/types/admin-dashboard/response/district'
  import { Address, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useDistrictStore } from '@/stores/admin-dashboard/district'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import DistrictsTable from './components/DistrictsTable.vue'
  import EditDistrictPopup from './components/EditDistrictPopup.vue'
  import DeleteDistrictPopup from './components/DeleteDistrictPopup.vue'
  import CreateDistrictPopup from './components/CreateDistrictPopup.vue'
  import { useLookups } from '@/composables/useLookups'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.Address, Address.CreateDistrict)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.districts') },
  ]

  const {
    fetchDistricts,
    districts,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const { getCities } = useLookups()

  const districtStore = useDistrictStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedDistrict = ref<District | null>(null)
  const districtToDelete = ref<District | null>(null)

  function editDistrict(district: District) {
    selectedDistrict.value = district
    isEditPopupOpen.value = true
  }

  function deleteDistrict(district: District) {
    districtToDelete.value = district
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!districtToDelete.value) return

    setDeleting(true)
    const { success } = await districtStore.deleteDistrict(districtToDelete.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      districtToDelete.value = null
      fetchDistricts()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedDistrict.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      districtToDelete.value = null
    }
  })

  onMounted(() => {
    getCities()
    fetchDistricts()
  })
</script>

<style scoped>

</style>

