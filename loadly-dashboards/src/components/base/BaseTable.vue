<template>
  <div class="overflow-x-auto rounded-lg border border-neutral-500">
    <table class="w-full min-w-max overflow-x-auto">
      <thead :class="headClass">
        <tr>
          <th
            v-for="col in columns"
            :key="col.key"
            class="text-start"
            :class="col.class || 'px-6 py-4 text-sm font-semibold text-neutral-20'"
          >
            <slot
              :name="`header-${col.key}`"
              :column="col"
            >
              {{ col.label }}
            </slot>
          </th>
        </tr>
      </thead>

      <tbody>
        <tr
          v-for="(row, rowIndex) in data"
          :key="rowIndex"
          class="
            border-b border-gray-50 transition-colors
            last:border-b-0
            hover:bg-gray-50/50
          "
        >
          <td
            v-for="col in columns"
            :key="col.key"
            class="px-6 py-4 text-sm text-content-secondary"
            :class="col.cellClass"
          >
            <slot
              :name="`cell-${col.key}`"
              :row="row"
              :col="col"
              :index="rowIndex"
            >
              {{ row[col.key] || '-' }}
            </slot>
          </td>
        </tr>


        <tr v-if="!data.length">
          <td
            class="py-2 text-center text-sm text-content-secondary"
            colspan="100%"
          >
            {{ $t('content.noData') }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts" generic="T extends Record<string, any>">
  import type { TableColumn } from '@/types/ui'

  withDefaults(defineProps<{
    columns: TableColumn[]
    headClass?: string
    data: T[]
  }>(),
    {
      headClass: 'border-b border-neutral-500 bg-neutral-400',
    }
  )
</script>

<style scoped>

</style>
