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
        {{ $t('title.documents') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addDocument') }}

        <IconPlus />
      </button>
    </div>

    <DocumentsTable
      :documents="documents"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editDocument"
      @delete="deleteDocument"
    />

    <EditDocumentPopup
      v-if="isEditPopupOpen"
      v-model="isEditPopupOpen"
      :document="selectedDocument"
      @refresh="fetchDocuments"
    />

    <DeleteDocumentPopup
      v-if="isDeletePopupOpen"
      v-model="isDeletePopupOpen"
      :document-name="documentToDelete?.label || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateDocumentPopup
      v-if="isCreatePopupOpen"
      v-model="isCreatePopupOpen"
      @refresh="fetchDocuments"
    />
  </div>
</template>

<script setup lang="ts">
  import type { Document } from '@/types/admin-dashboard/response/document'
  import { Document as DocumentPermission, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useDocumentStore } from '@/stores/admin-dashboard/document'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import DocumentsTable from './components/DocumentsTable.vue'

  const EditDocumentPopup = defineAsyncComponent(
    () => import('./components/EditDocumentPopup.vue')
  )
  const DeleteDocumentPopup = defineAsyncComponent(
    () => import('./components/DeleteDocumentPopup.vue')
  )
  const CreateDocumentPopup = defineAsyncComponent(
    () => import('./components/CreateDocumentPopup.vue')
  )

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.Document, DocumentPermission.Create)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.documents') },
  ]

  const {
    fetchDocuments,
    documents,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const documentStore = useDocumentStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedDocument = ref<Document | null>(null)
  const documentToDelete = ref<Document | null>(null)

  function editDocument(document: Document) {
    selectedDocument.value = document
    isEditPopupOpen.value = true
  }

  function deleteDocument(document: Document) {
    documentToDelete.value = document
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!documentToDelete.value) return

    setDeleting(true)
    const { success } = await documentStore.deleteDocument(documentToDelete.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      documentToDelete.value = null
      fetchDocuments()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedDocument.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      documentToDelete.value = null
    }
  })

  onMounted(() => {
    fetchDocuments()
  })
</script>

<style scoped>

</style>
