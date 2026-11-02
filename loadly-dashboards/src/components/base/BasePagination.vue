<template>
  <div class="mt-6 flex flex-wrap items-center justify-between">
    <div
      class="
        flex w-full items-center gap-2 text-sm text-neutral-800
        lg:w-auto
      "
    >
      <span>{{ $t('table.pagination.show') }}</span>

      <BaseSelect
        :model-value="String(pagination.size)"
        :options="selectOptions"
        class="w-20"
        @update:model-value="$emit('updateItemsPerPage', Number($event))"
      />

      <span>{{ $t('table.pagination.from') }} {{ pagination.items }} {{ $t('table.pagination.data') }}</span>
    </div>

    <div
      v-if="pagination.pages > 1"
      class="pagination"
    >
      <button
        class="pagination-item"
        :class="isPrevDisabled ? 'disabled' : 'enabled'"
        :disabled="isPrevDisabled"
        @click="$emit('setPage', 1)"
      >
        <IconChevronDouble
          class="
            size-4.5
            en:rotate-180
          "
        />
        {{ $t('table.pagination.first') }}
      </button>

      <button
        class="pagination-item"
        :class="isPrevDisabled ? 'disabled' : 'enabled'"
        :disabled="isPrevDisabled"
        @click="$emit('prevPage')"
      >
        <IconChevron
          class="
            size-4.5
            en:-rotate-180
          "
        />
        {{ $t('table.pagination.prev') }}
      </button>

      <template
        v-for="(pageItem, index) in visiblePageItems"
        :key="`page-item-${index}`"
      >
        <button
          v-if="typeof pageItem === 'number'"
          class="pagination-item"
          :class="
            pageItem === pagination.page
              ? 'border-neutral-500 text-neutral-800'
              : `
                border-transparent text-neutral-800
                hover:bg-neutral-400
              `
          "
          @click="$emit('setPage', pageItem)"
        >
          {{ pageItem }}
        </button>

        <span
          v-else
          class="pagination-item border-0"
        >
          ...
        </span>
      </template>

      <button
        class="pagination-item"
        :class=" isNextDisabled ? 'disabled' : 'enabled'"
        :disabled="isNextDisabled"
        @click="$emit('nextPage')"
      >
        {{ $t('table.pagination.next') }}
        <IconChevron
          class="
            size-4.5
            ar:-rotate-180
          "
        />
      </button>

      <button
        class="pagination-item"
        :class="isNextDisabled ? 'disabled' : 'enabled'"
        :disabled="isNextDisabled"
        @click="$emit('setPage', pagination.pages)"
      >
        {{ $t('table.pagination.last') }}
        <IconChevronDouble
          class="
            size-4.5
            ar:rotate-180
          "
        />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import type { Pagination } from '@/types/response/pagination'

  const props = defineProps<{
    pagination: Pagination
    perPageOptions?: number[]
  }>()

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [value: number]
  }>()

  const isPrevDisabled = computed(() => props.pagination.page === 1)
  const isNextDisabled = computed(() => props.pagination.page === props.pagination.pages)

  const selectOptions = computed((): SelectOption[] => {
    const options = props.perPageOptions ?? [10, 20, 50, 100]

    return options.map(value => ({
      label: String(value),
      value: String(value),
    }))
  })

  const visiblePageItems = computed((): (number | 'ellipsis')[] => {
    const totalPages = props.pagination.pages
    const currentPage = props.pagination.page
    const sideWindow = 1
    const maxVisibleWithoutEllipsis = 7

    if (totalPages <= maxVisibleWithoutEllipsis) {
      return Array.from({ length: totalPages }, (_, index) => index + 1)
    }

    const items: (number | 'ellipsis')[] = [1]
    const start = Math.max(2, currentPage - sideWindow)
    const end = Math.min(totalPages - 1, currentPage + sideWindow)

    if (start > 2) {
      items.push('ellipsis')
    }

    for (let page = start; page <= end; page += 1) {
      items.push(page)
    }

    if (end < totalPages - 1) {
      items.push('ellipsis')
    }

    items.push(totalPages)

    return items
  })
</script>

<style scoped>

</style>

