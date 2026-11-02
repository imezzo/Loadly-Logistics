<template>
  <CardPreview :title="$t('content.documents')">
    <div class="space-y-3 rounded-[0.625rem] border border-gray-50 bg-neutral-30 p-3">
      <div
        v-for="(doc, index) in documentList"
        :key="index"
        class="flex items-center justify-between"
      >
        <CardPreviewLabel
          :label="doc.label"
          content=""
        />

        <div class="ms-5.5 flex w-1/3 items-center gap-3 rounded-lg border border-gray-200 p-3">
          <UploaderFileExtension
            :file="doc.file"
            :file-url="doc.fileUrl"
          />

          <p class="truncate text-sm font-medium text-content-secondary">
            {{ getDocumentFileName(doc) }}
          </p>

          <div class="flex items-center gap-2">
            <span
              v-if="doc.status"
              class="rounded-full px-3 py-1 text-xs font-medium"
              :class="getStatusClass(doc.status)"
            >
              {{ getStatusTranslation(doc.status) }}
            </span>

            <BaseLink
              v-if="doc.fileUrl"
              :to="doc.fileUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="
                text-primary
                hover:text-primary/80
              "
            >
              {{ $t('content.preview') }}
            </BaseLink>
          </div>
        </div>
      </div>

      <p
        v-if="!documentList.length"
        class="text-sm text-gray-500"
      >
        {{ $t('content.noDocuments') }}
      </p>
    </div>
  </CardPreview>
</template>

<script setup lang="ts">
  import { extractFileName } from '@/utils/file'

  interface DocumentItem {
    label: string
    file?: File
    fileUrl?: string | null
    status?: string
    id?: number
    expirationDate?: string
  }

  defineProps<{
    documentList: DocumentItem[]
  }>()

  const { t } = useI18n()

  function getDocumentFileName(doc: DocumentItem): string {
    return doc.file?.name ??
      (doc.fileUrl
        ? extractFileName(doc.fileUrl)
        : t('content.noFileUploaded'))
  }

  function getStatusClass(status: string): string {
    const statusLower = status.toLowerCase()
    if (statusLower.includes('verified') || status === '1') {
      return 'bg-green-100 text-green-800'
    }
    if (statusLower.includes('rejected') || status === '2') {
      return 'bg-red-100 text-red-800'
    }

    return 'bg-orange-100 text-orange-800'
  }

  function getStatusTranslation(status: string): string {
    const statusLower = status.toLowerCase()
    if (statusLower.includes('verified') || status === '1') {
      return t('content.status.verified')
    }
    if (statusLower.includes('rejected') || status === '2') {
      return t('content.status.rejected')
    }

    return t('content.status.pending')
  }
</script>

<style scoped>

</style>
