<template>
  <BaseForm
    ref="formRef"
    :validation-schema="validationSchema"
    :initial-values="initialValues"
    :groups="groups"
    :errors="errors"
    @submit="$emit('next', $event)"
  >
    <template #submit>
      <BaseActions
        container-class="flex items-center justify-end gap-3 sm:gap-6"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="router.push({ name: 'Drivers' })"
        @next="$emit('next', formRef?.form?.values as Record<string, unknown>)"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import type { FieldsGroup } from '@/types/form'
  import { useLookups } from '@/composables/useLookups'

  const props = defineProps<{
    validationSchema: object
    initialValues: Record<string, unknown>
    groups: Record<string, FieldsGroup>
    errors?: Record<string, string[]>
    isUpdateMode: boolean
  }>()

  defineEmits<{
    next: [values: Record<string, unknown>]
  }>()

  const { t } = useI18n()
  const router = useRouter()
  const { isLoading } = useLoading()
  const { getContractors } = useLookups()

  const formRef = useTemplateRef('formRef')

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'flex h-13.5 w-1/2 items-center justify-center gap-2 rounded-2xl bg-black/15 px-4 py-4.25 font-medium text-content-secondary transition-colors hover:bg-black/20 sm:w-52.5',
    hasScrollTop: true,
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.next'),
    class: 'w-1/2 h-13.5 btn-primary py-3.75 sm:w-75',
    disabled: isLoading.value,
    asSubmit: props.isUpdateMode,
    hasScrollTop: true,
  }))

  defineExpose({
    formRef,
  })

  onMounted(() => getContractors(1))
</script>

<style scoped>

</style>
