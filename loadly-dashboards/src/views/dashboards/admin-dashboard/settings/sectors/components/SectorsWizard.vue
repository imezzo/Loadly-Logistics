<template>
  <BasePopup v-model="model">
    <div class="w-full p-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ popupTitle }}
      </h2>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-4"
      />

      <BaseForm
        ref="form"
        :validation-schema
        :initial-values
        :fields-schema
        :errors="formErrors"
        container-class="grid grid-cols-1 gap-4"
        @submit="submit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-center gap-3"
            :prev-button-options
            :next-button-options
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useLoading } from '@/composables/useLoading'
  import { useSectorSettingsStore } from '@/stores/admin-dashboard/sector-settings'
  import type { Sector } from '@/types/admin-dashboard/response/sector/sector'
  import { useList } from '../composables/useList'
  import type { ApiResponse } from '@/types/admin-dashboard/response/response'
  import { useSectorsWizard } from '../composables/useSectorsWizard'

  const props = defineProps<{
    sector: Sector | null
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const form = useTemplateRef('form')

  useVeeValidate()
  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const { fetchSectors } = useList()
  const { validationSchema, fieldsSchema, initialValues } = useSectorsWizard(computed(() => props.sector))

  const sectorSettingsStore = useSectorSettingsStore()

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const popupTitle = computed(() => (
    props.sector ? t('title.editSector') : t('title.addSector')
  ))

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'btn-secondary w-52.5 h-13.5',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.saveChanges'),
    class: 'btn-primary w-52.5 h-13.5',
    disabled: isLoading.value,
    asSubmit: true,
  }))

  async function submit(values: Record<string, unknown>): Promise<void> {
    const request = {
      nameAr: String(values.nameAr),
      nameEn: String(values.nameEn),
    }

    setLoading(true)
    const response = props.sector
      ? await sectorSettingsStore.update(props.sector.id, request)
      : await sectorSettingsStore.create(request)
    setLoading(false)

    handleSubmitResponse(response)
  }

  function handleSubmitResponse(response: ApiResponse<Sector | null>): void {
    if (response.success) {
      discard()
      fetchSectors()

      return
    }

    errorMessage.value = response.message || t('error.networkError')
    formErrors.value = response.errors || {}
  }

  function discard(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    model.value = false
  }
</script>

<style scoped>

</style>
