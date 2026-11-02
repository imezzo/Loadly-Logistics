<template>
  <div
    class="flex size-8 shrink-0 items-end rounded-lg p-1"
    :class="fileTypeColor"
  >
    <span class="text-xs font-medium text-white">{{ fileExtension }}</span>
  </div>
</template>

<script setup lang="ts">
  import { extractFileName } from '@/utils/file'

  const props = defineProps<{
    file?: File
    fileUrl?: string | null
  }>()

  const { t } = useI18n()

  const fileName = computed(() => {
    return props.file?.name ??
      (props.fileUrl
        ? extractFileName(props.fileUrl)
        : t('content.noFileUploaded'))
  })

  const fileExtension = computed(() => {
    const name = fileName.value
    const ext = name.split('.').pop()?.toUpperCase() || 'PDF'

    return ext
  })

  const fileTypeColor = computed(() => {
    const ext = fileExtension.value.toLowerCase()
    if (ext === 'pdf') return 'bg-red-500'
    if (['jpg', 'jpeg', 'png'].includes(ext)) return 'bg-blue-500'
    if (['xls', 'xlsx'].includes(ext)) return 'bg-success-500'

    return 'bg-gray-500'
  })
</script>

<style scoped>

</style>
