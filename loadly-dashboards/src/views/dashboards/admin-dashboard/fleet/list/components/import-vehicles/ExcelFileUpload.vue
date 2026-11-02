<template>
  <input
    ref="fileInputRef"
    type="file"
    accept=".xlsx,.xls"
    class="hidden"
    @change="onFileChange"
  >

  <button
    type="button"
    class="
      mb-6 flex w-full flex-col items-center justify-center gap-2 rounded-xl border-2 border-dashed
      border-neutral-300 bg-neutral-100/50 py-10 transition-colors
      hover:border-primary hover:bg-primary/5
    "
    @click="fileInputRef?.click()"
  >
    <span class="text-sm font-medium text-neutral-700">
      {{ model ? model.name : $t('content.clickToUpload') }}
    </span>
    <span class="text-xs text-neutral-10">
      {{ $t('content.excelFilesOnly') }}
    </span>
  </button>
</template>

<script setup lang="ts">
  const model = defineModel<File | null>({ required: true })

  const fileInputRef = useTemplateRef<HTMLInputElement>('fileInputRef')

  function onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement
    const file = input.files?.[0]
    model.value = file ?? null
  }

  function reset(): void {
    model.value = null
    if (fileInputRef.value) fileInputRef.value.value = ''
  }

  defineExpose({ reset })
</script>

<style scoped>

</style>
