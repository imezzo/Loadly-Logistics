<template>
  <div class="mt-4 flex flex-col">
    <ContractDestinationListHeader :existing-destinations="rows" />

    <BaseInput
      v-model="search"
      :placeholder="$t('form.search')"
      wrapper-class="mb-4 sm:max-w-100"
    />

    <BaseTable
      :columns="tableColumns"
      :data="paginatedRows"
    >
      <template #cell-destinationId="{ row }">
        {{ row.clientId }}
      </template>

      <template #cell-destinationName="{ row }">
        {{ getLocalizedValue(row.nameEn, row.nameAr) }}
      </template>

      <template #cell-createdAt="{ row }">
        {{ formatDateLocal(row.createdAt ?? '') }}
      </template>

      <template #cell-updatedAt="{ row }">
        {{ formatDateLocal(row.updatedAt ?? '') }}
      </template>

      <template #cell-action="{ row }">
        <div class="flex items-center gap-2">
          <button
            type="button"
            class="
              flex size-8 items-center justify-center rounded-full border border-neutral-500
              text-neutral-700
              hover:bg-neutral-100
            "
            :title="$t('button.viewDetails')"
            @click="openDetails(row)"
          >
            <IconEye
              visible
              class="size-4"
            />
          </button>

          <template v-if="!isFormDisabled">
            <button
              type="button"
              class="
                flex size-8 items-center justify-center rounded-full text-red-600
                hover:bg-red-50
              "
              :title="$t('button.delete')"
              @click="emit('remove', row)"
            >
              <IconTrash class="size-4" />
            </button>

            <button
              type="button"
              class="
                flex size-8 items-center justify-center rounded-full border border-neutral-500
                text-neutral-700
                hover:bg-neutral-100
              "
              :title="$t('button.edit')"
              @click="emit('edit', row)"
            >
              <IconEdit class="size-4" />
            </button>
          </template>
        </div>
      </template>
    </BaseTable>

    <ContractDestinationDetailsPopup
      v-model="detailsOpen"
      :destination="detailRow"
    />

    <BasePagination
      :pagination="pagination"
      :per-page-options="[5, 10, 20, 50, 100]"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
    />
  </div>
</template>

<script setup lang="ts">
  import { usePagination } from '@/composables/usePagination'
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import type { Pagination } from '@/types/response/pagination'
  import type { TableColumn } from '@/types/ui'
  import { getLocalizedValue } from '@/utils/i18n'
  import { useContractWizard } from '@/views/global-modules/contracts/wizard/composables/useContractWizard'
  import ContractDestinationListHeader from './ContractDestinationListHeader.vue'
  import { formatDateLocal } from '@/utils/date'

  const ContractDestinationDetailsPopup = defineAsyncComponent(
    () => import('./ContractDestinationDetailsPopup.vue'),
  )

  const props = defineProps<{
    rows: ContractWizardDestination[]
  }>()

  const emit = defineEmits<{
    remove: [row: ContractWizardDestination]
    edit: [row: ContractWizardDestination]
  }>()

  const { t } = useI18n()
  const { isFormDisabled } = useContractWizard()

  const detailsOpen = ref(false)
  const detailRow = ref<ContractWizardDestination | null>(null)
  const search = ref('')

  function openDetails(row: ContractWizardDestination): void {
    detailRow.value = row
    detailsOpen.value = true
  }

  watch(detailsOpen, (open) => {
    if (!open) detailRow.value = null
  })

  const filteredRows = computed(() => {
    return props.rows.filter((row) => {
      return row.nameEn.toLowerCase().includes(search.value.toLowerCase())
        || row.nameAr.toLowerCase().includes(search.value.toLowerCase())
    })
  })

  const pagination = computed((): Pagination => {
    const items = props.rows.length
    const pages = Math.max(1, Math.ceil(items / itemsPerPage.value) || 1)

    return {
      page: currentPage.value,
      size: itemsPerPage.value,
      pages,
      items,
    }
  })

  const { currentPage, itemsPerPage, setPage, nextPage, prevPage, updateItemsPerPage } = usePagination(pagination)

  itemsPerPage.value = 5

  const paginatedRows = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage.value

    return filteredRows.value.slice(start, start + itemsPerPage.value)
  })

  const tableColumns = computed((): TableColumn[] => [
    { key: 'destinationId', label: t('table.id') },
    { key: 'destinationName', label: t('table.destinationName') },
    { key: 'createdAt', label: t('table.createdAt') },
    { key: 'updatedAt', label: t('table.updatedAt') },
    { key: 'expectedKms', label: t('table.kilometer') },
    { key: 'returnExpectedKms', label: t('table.returnExpectedKms') },
    { key: 'numberOfDays', label: t('table.numberOfDays') },
    { key: 'pricePerTon', label: t('table.pricePerTon') },
    { key: 'fixedTripPrice', label: t('table.fixedTripPrice') },
    { key: 'contractorPricePerTon', label: t('table.contractorPricePerTon') },
    { key: 'contractorFixedTripPrice', label: t('table.contractorFixedTripPrice') },
    { key: 'action', label: t('table.action') },
  ])

  watch(
    () => pagination.value.pages,
    (pages) => {
      if (currentPage.value > pages) currentPage.value = pages
    },
  )
</script>

<style scoped>

</style>
