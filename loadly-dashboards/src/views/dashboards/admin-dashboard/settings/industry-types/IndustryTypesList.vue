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
        {{ $t('title.industryTypes') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addIndustryType') }}

        <IconPlus />
      </button>
    </div>

    <IndustryTypesTable
      :industry-types="industryTypes"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editIndustryType"
      @delete="deleteIndustryType"
    />

    <EditIndustryTypePopup
      v-model="isEditPopupOpen"
      :industry-type="selectedIndustryType"
      @refresh="fetchIndustryTypes"
    />

    <DeleteIndustryTypePopup
      v-model="isDeletePopupOpen"
      :industry-type-name="industryTypeToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateIndustryTypePopup
      v-model="isCreatePopupOpen"
      @refresh="fetchIndustryTypes"
    />
  </div>
</template>

<script setup lang="ts">
  import type { IndustryType } from '@/types/admin-dashboard/response/industry-type'
  import { IndustryType as IndustryTypePermission, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useIndustryTypeStore } from '@/stores/admin-dashboard/industry-type'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import IndustryTypesTable from './components/IndustryTypesTable.vue'
  import EditIndustryTypePopup from './components/EditIndustryTypePopup.vue'
  import DeleteIndustryTypePopup from './components/DeleteIndustryTypePopup.vue'
  import CreateIndustryTypePopup from './components/CreateIndustryTypePopup.vue'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.IndustryType, IndustryTypePermission.Create)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.industryTypes') },
  ]

  const {
    fetchIndustryTypes,
    industryTypes,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const industryTypeStore = useIndustryTypeStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedIndustryType = ref<IndustryType | null>(null)
  const industryTypeToDelete = ref<IndustryType | null>(null)

  function editIndustryType(industryType: IndustryType) {
    selectedIndustryType.value = industryType
    isEditPopupOpen.value = true
  }

  function deleteIndustryType(industryType: IndustryType) {
    industryTypeToDelete.value = industryType
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!industryTypeToDelete.value) return

    setDeleting(true)
    const { success } = await industryTypeStore.deleteIndustryType(
      industryTypeToDelete.value.id
    )
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      industryTypeToDelete.value = null
      fetchIndustryTypes()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedIndustryType.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      industryTypeToDelete.value = null
    }
  })

  onMounted(() => {
    fetchIndustryTypes()
  })
</script>

<style scoped>

</style>
