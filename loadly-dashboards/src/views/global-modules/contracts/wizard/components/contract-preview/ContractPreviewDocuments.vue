<template>
  <CardPreview :title="$t('content.documentation')">
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

        <div
          class="
            ms-5.5 flex w-1/2 flex-col gap-1 rounded-lg border border-gray-200 p-3
            sm:flex-row sm:items-center sm:gap-3
          "
        >
          <div class="flex min-w-0 items-center gap-3">
            <UploaderFileExtension
              :file="doc.file"
              :file-url="doc.fileUrl"
            />

            <div class="min-w-0">
              <p class="truncate text-sm font-medium text-content-secondary">
                {{ getDocumentFileName(doc) }}
              </p>
              <p
                v-if="doc.expirationDate"
                class="text-xs text-neutral-600"
              >
                {{ $t('form.expiryDate') }}: {{ formatExpiryDate(doc.expirationDate) }}
              </p>
              <p
                v-if="doc.status != null && String(doc.status).trim() !== ''"
                class="text-xs text-neutral-600"
              >
                {{ $t('form.statusField') }}: {{ getDocumentStatusLabel(doc.status) }}
              </p>
            </div>
          </div>

          <BaseLink
            v-if="doc.fileUrl || doc.file"
            :to="doc.fileUrl || '#'"
            target="_blank"
            rel="noopener noreferrer"
            class="
              ms-auto flex items-center gap-2 rounded-full bg-primary/30 px-4 py-2 text-sm
              font-medium text-primary transition-colors
              hover:bg-primary/40
            "
            @click="doc.file && previewFile(doc.file)"
          >
            <IconEye
              visible
              class="[&>path]:stroke-primary"
            />
            {{ $t('content.preview') }}
          </BaseLink>
        </div>
      </div>

      <p
        v-if="!documentList.length"
        class="text-sm text-gray-500"
      >
        {{ $t('content.noFileUploaded') }}
      </p>
    </div>
  </CardPreview>
</template>

<script setup lang="ts">
  import { useContractDocuments } from '../../composables/useContractDocuments'
  import { useContractWizard } from '../../composables/useContractWizard'
  import { useEnum } from '@/composables/useEnum'
  import { extractFileName } from '@/utils/file'

  const { formData } = useContractWizard()
  const { t } = useI18n()
  const { getDisplayName } = useEnum()

  const { documents, fetchDocuments, restoreFormValues } = useContractDocuments()

  interface DocumentItem {
    label: string
    file?: File
    fileUrl?: string | null
    status?: string
    id?: number
    expirationDate?: string
  }

  const documentList = computed<DocumentItem[]>(() => {
    const data = formData.value || {}

    return documents.value.map((doc, index) => {
      const fileKey = `document_${index}` as keyof typeof data
      const file = data[fileKey] as File | undefined

      return {
        label: doc.label,
        file: doc.file || file,
        fileUrl: doc.fileUrl || null,
        status: doc.status || 'Pending',
        id: doc.id,
        expirationDate: doc.expirationDate,
      }
    })
  })

  function getDocumentStatusLabel(status: string | undefined): string {
    if (status == null || String(status).trim() === '') return t('content.status.pending')
    const num = Number(status)

    return Number.isNaN(num) ? status : getDisplayName('DocumentVerificationStatus', num)
  }

  function formatExpiryDate(value: string): string {
    if (!value) return value
    const date = new Date(value)

    return Number.isNaN(date.getTime()) ? value : date.toLocaleDateString()
  }

  function getDocumentFileName(doc: DocumentItem): string {
    return doc.file?.name ??
      (doc.fileUrl
        ? extractFileName(doc.fileUrl)
        : t('content.noFileUploaded'))
  }

  function previewFile(file: File): void {
    const url = URL.createObjectURL(file)
    window.open(url, '_blank')
  }

  onMounted(async () => {
    await fetchDocuments()
    if (formData.value?._documents && Array.isArray(formData.value._documents)) {
      restoreFormValues(formData.value)
    }
  })

  watch(
    () => formData.value?._documents,
    (docs) => {
      if (docs && Array.isArray(docs) && documents.value.length) {
        restoreFormValues(formData.value)
      }
    },
    { deep: true }
  )
</script>

<style scoped>

</style>

