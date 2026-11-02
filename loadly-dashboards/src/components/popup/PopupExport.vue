<template>
  <BasePopup
    v-model="model"
    content-class="max-w-200!"
  >
    <div class="w-full px-6 py-2">
      <h2 class="text-2xl font-bold text-neutral-10">
        {{ title }}
      </h2>

      <p class="mt-1 text-sm text-content-secondary">
        {{ $t('content.chooseExportColumns') }}
      </p>

      <div class="mt-6">
        <div class="border-b border-neutral-500 bg-neutral-400 px-2.75 py-3.5">
          <BaseCheckbox
            :model-value="isAllSelected"
            :indeterminate="isIndeterminate"
            :label="$t('form.allColumns')"
            label-class="font-medium text-neutral-10"
            @update:model-value="toggleAll"
          />
        </div>

        <div
          class="
            mt-4 grid max-h-70 grid-cols-1 gap-5 overflow-x-auto px-2.75
            sm:grid-cols-2
            md:grid-cols-3
          "
        >
          <BaseCheckbox
            v-for="column in columns"
            :key="column.key"
            :model-value="selectedKeys.includes(column.key)"
            :label="column.label"
            label-class="font-medium text-neutral-10"
            @update:model-value="(checked) => toggleKey(column.key, checked)"
          />
        </div>
      </div>

      <BaseActions
        :prev-button-options
        :next-button-options
        class="flex-wrap"
        @prev="model = false"
        @next="emit('export', [...selectedKeys])"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { ExportKey } from '@/types/export-keys.types'

  const props = defineProps<{
    columns: ExportKey[]
    loading?: boolean
    title: string
  }>()

  const emit = defineEmits<{
    export: [keys: string[]]
  }>()

  const { t } = useI18n()

  const model = defineModel<boolean>('modelValue', { required: true })

  const selectedKeys = ref<string[]>([])

  const prevButtonOptions = computed(() => ({
    class: 'btn-secondary px-4 md:w-65',
    label: t('button.cancel'),
  }))

  const nextButtonOptions = computed(() => ({
    class: 'btn-primary px-4 md:w-65',
    disabled: !selectedKeys.value.length || !!props.loading,
    label: t('button.export'),
  }))

  const isAllSelected = computed(() =>
    !!props.columns.length && selectedKeys.value.length === props.columns.length
  )

  const isIndeterminate = computed(() =>
    !!selectedKeys.value.length && selectedKeys.value.length < props.columns.length
  )

  function toggleAll(checked: boolean | undefined): void {
    selectedKeys.value = checked
      ? props.columns.map(column => column.key)
      : []
  }

  function toggleKey(key: string, checked: boolean | undefined): void {
    if (checked) {
      if (!selectedKeys.value.includes(key)) {
        selectedKeys.value = [...selectedKeys.value, key]
      }

      return
    }

    selectedKeys.value = selectedKeys.value.filter(selectedKey => selectedKey !== key)
  }

  function resetSelection(): void {
    selectedKeys.value = props.columns.map(column => column.key)
  }

  watch(model, (isOpen) => {
    if (isOpen) {
      resetSelection()
    }
  })

  watch(() => props.columns, () => {
    if (model.value) {
      resetSelection()
    }
  })
</script>

<style scoped>

</style>
