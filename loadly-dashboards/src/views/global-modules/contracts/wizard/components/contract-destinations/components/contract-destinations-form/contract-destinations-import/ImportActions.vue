<template>
  <div class="flex gap-3">
    <button
      type="button"
      class="btn-primary mb-4 w-1/4"
      @click="fileInputRef?.click()"
    >
      {{ $t('button.import') }}
      <IconPlus />
    </button>

    <input
      ref="fileInputRef"
      type="file"
      accept=".xlsx,.xls"
      class="hidden"
      @change="emit('fileChange', $event)"
    >

    <a
      :href="templateUrl"
      target="_blank"
      download
      class="btn-link mb-4 flex w-1/4 items-center gap-1"
    >
      {{ $t('button.downloadTemplate') }}
      <IconDownload class="size-4" />
    </a>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    templateUrl?: string
  }>()

  const emit = defineEmits<{
    fileChange: [file: Event]
  }>()

  const fileInputRef = useTemplateRef<HTMLInputElement>('fileInputRef')
</script>

<style scoped>

</style>
