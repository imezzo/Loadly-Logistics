<template>
  <div class="mt-2">
    <CardPreview :title="$t('content.documents')">
      <div
        v-if="documentRows.length === 0"
        class="
          rounded-[0.625rem] border border-gray-50 bg-neutral-30 px-4 py-6 text-center text-sm
          text-content-secondary
        "
      >
        {{ $t('content.noData') }}
      </div>

      <div
        v-else
        class="divide-y divide-gray-100 rounded-[0.625rem] border border-gray-50 bg-neutral-30"
      >
        <section
          v-for="doc in documentRows"
          :key="doc.rowKey"
          class="p-4"
        >
          <div class="mb-3">
            <p class="text-sm font-semibold text-neutral-800">
              {{ doc.typeName }}
            </p>

            <p class="mt-0.5 text-xs text-content-secondary">
              {{ $t('content.tripDocumentUploaded', { date: formatUploadedDate(doc.createdAt) }) }}
            </p>
          </div>

          <ul class="flex flex-col gap-2">
            <li
              v-for="(file, fileIndex) in doc.files"
              :key="`${doc.rowKey}-${fileIndex}`"
            >
              <div
                class="
                  flex items-center gap-3 rounded-lg border border-gray-100 bg-white px-3 py-2.5
                "
              >
                <div
                  v-if="isImagePath(file.path)"
                  class="
                    size-11 shrink-0 overflow-hidden rounded-md border border-gray-100 bg-neutral-30
                  "
                >
                  <img
                    :src="file.url"
                    :alt="extractFileName(file.path)"
                    class="size-full object-cover"
                    loading="lazy"
                    @error="onThumbError"
                  >
                </div>

                <UploaderFileExtension
                  v-else
                  class="shrink-0"
                  :file-url="file.url"
                />

                <span class="min-w-0 flex-1 truncate text-sm text-neutral-800">
                  {{ extractFileName(file.path) }}
                </span>

                <a
                  :href="file.url"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="
                    btn-primary-outline inline-flex shrink-0 items-center gap-1.5 px-3 py-1.5
                    text-xs
                  "
                >
                  <IconEye
                    visible
                    class="size-3.5"
                  />
                  {{ $t('content.preview') }}
                </a>
              </div>
            </li>
          </ul>
        </section>
      </div>
    </CardPreview>
  </div>
</template>

<script setup lang="ts">
  import CardPreview from '@/components/card/CardPreview.vue'
  import IconEye from '@/components/icon/IconEye.vue'
  import UploaderFileExtension from '@/components/uploader/UploaderFileExtension.vue'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { TripWizardDocument } from '@/types/admin-dashboard/response/trip/trip'
  import { extractFileName, isImagePath } from '@/utils/file'

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const documentRows = computed(() => {
    const list = trip.value.documents ?? []

    return list.map((document: TripWizardDocument, index: number) => ({
      ...document,
      rowKey: `${document.type}-${document.waypointId}-${document.recordingId}-${document.createdAt}-${index}`,
    }))
  })

  function formatUploadedDate(iso: string): string {
    const date = new Date(iso)
    if (Number.isNaN(date.getTime())) return '—'

    return date.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    })
  }

  function onThumbError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none'
  }
</script>

<style scoped>

</style>
