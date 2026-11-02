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
              {{ getDisplayName('DocumentVerificationStatus', Number(doc.status)) }}
            </span>

            <BaseLink
              v-if="doc.fileUrl || doc.file"
              :to="doc.fileUrl || '#'"
              target="_blank"
              rel="noopener noreferrer"
              class="
                flex items-center gap-2 rounded-full bg-primary/50 px-4 py-2 text-sm font-medium
                text-primary transition-colors
                hover:bg-primary/40
              "
              @click="(e: MouseEvent) => { if (doc.file && !doc.fileUrl) { e.preventDefault(); previewFile(doc.file!) } }"
            >
              <IconEye
                visible
                class="[&>path]:stroke-primary"
              />
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
  import { useEnum } from '@/composables/useEnum'
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
  const { getDisplayName } = useEnum()

  function getDocumentFileName(doc: DocumentItem): string {
    return doc.file?.name ??
      (doc.fileUrl
        ? extractFileName(doc.fileUrl)
        : t('content.noFileUploaded'))
  }

  function getStatusClass(status: string): string {
    if (!['1', '2'].includes(status)) return 'bg-orange-100 text-orange-800'

    return status === '1'
      ? 'bg-green-100 text-green-800'
      : 'bg-red-100 text-red-800'
  }

  function previewFile(file: File): void {
    const url = URL.createObjectURL(file)
    window.open(url, '_blank')
  }
</script>

<style scoped>

</style>

