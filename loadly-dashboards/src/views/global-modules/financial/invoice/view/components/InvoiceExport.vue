<template>
  <ButtonExport
    :disabled="isLoading"
    :class="{ 'hidden': isLoading }"
    @click="exportPDF"
  />

  <PopupError
    :model-value="!!error"
    :message="error"
  />
</template>

<script setup lang="ts">
  const props = defineProps<{
    invoiceNumber?: string | null
    pdfSection: HTMLElement | null
  }>()

  const emit = defineEmits<({
    isExportInAction: [boolean]
  })>()

  const { t } = useI18n()

  const isLoading = ref(false)
  const error = ref('')

  const exportPDF = async () => {
    if (!props.pdfSection || isLoading.value) return

    emit('isExportInAction', true)
    isLoading.value = true

    try {
      const [{ default: html2canvas }, { jsPDF }] = await Promise.all([
        import('html2canvas'),
        import('jspdf'),
      ])

      const canvas = await html2canvas(props.pdfSection, {
        backgroundColor: '#ffffff',
        scale: 2,
        useCORS: true,
      })

      const convertedImage = canvas.toDataURL('image/png')

      const pdf = new jsPDF({
        format: 'a4',
        orientation: 'landscape',
        unit: 'mm'
      })

      const pdfWidth = pdf.internal.pageSize.getWidth()
      const pdfHeight = (canvas.height * pdfWidth) / canvas.width

      pdf.addImage(convertedImage, 'PNG', 0, 0, pdfWidth, pdfHeight)

      const fileName = `invoice-${props.invoiceNumber ?? ''}-${new Date().toISOString()}.pdf`
      pdf.save(fileName)
    } catch (err) {
      error.value = (err as Error).message || t('error.somethingWentWrong')
    }

    isLoading.value = false
    emit('isExportInAction', false)
  }
</script>

<style scoped>

</style>
