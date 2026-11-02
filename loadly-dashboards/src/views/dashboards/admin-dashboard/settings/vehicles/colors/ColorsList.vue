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
        {{ $t('title.colors') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addColor') }}

        <IconPlus />
      </button>
    </div>

    <ColorsTable
      :colors="colors"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editColor"
      @delete="deleteColor"
    />

    <EditColorPopup
      v-model="isEditPopupOpen"
      :color="selectedColor"
      @refresh="fetchColors"
    />

    <DeleteColorPopup
      v-model="isDeletePopupOpen"
      :color-name="colorToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateColorPopup
      v-model="isCreatePopupOpen"
      @refresh="fetchColors"
    />
  </div>
</template>

<script setup lang="ts">
  import type { Color } from '@/types/admin-dashboard/response/color'
  import { Color as ColorPermission, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useColorStore } from '@/stores/admin-dashboard/color'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import ColorsTable from './components/ColorsTable.vue'
  import EditColorPopup from './components/EditColorPopup.vue'
  import DeleteColorPopup from './components/DeleteColorPopup.vue'
  import CreateColorPopup from './components/CreateColorPopup.vue'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.Color, ColorPermission.Create)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.colors') },
  ]

  const {
    fetchColors,
    colors,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const colorStore = useColorStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedColor = ref<Color | null>(null)
  const colorToDelete = ref<Color | null>(null)

  function editColor(color: Color) {
    selectedColor.value = color
    isEditPopupOpen.value = true
  }

  function deleteColor(color: Color) {
    colorToDelete.value = color
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!colorToDelete.value) return

    setDeleting(true)
    const { success } = await colorStore.deleteColor(colorToDelete.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      colorToDelete.value = null
      fetchColors()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedColor.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      colorToDelete.value = null
    }
  })

  onMounted(() => {
    fetchColors()
  })
</script>

<style scoped>

</style>
