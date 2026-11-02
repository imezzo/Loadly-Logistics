import type { IndustryType } from '@/types/admin-dashboard/response/industry-type'
import type { Pagination } from '@/types/response/pagination'
import { useIndustryTypeStore } from '@/stores/admin-dashboard/industry-type'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const industryTypeStore = useIndustryTypeStore()
  const { industryTypesList } = storeToRefs(industryTypeStore)

  const { isLoading, setLoading } = useLoading()

  const typePagination = computed<Pagination | null>(
    () => industryTypesList.value?.pagination || null
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(typePagination)

  async function fetchIndustryTypes() {
    setLoading(true)

    const { success, data } = await industryTypeStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    industryTypesList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchIndustryTypes()
  })

  watch(currentPage, () => {
    fetchIndustryTypes()
  })

  const industryTypes = computed<IndustryType[]>(
    () => industryTypesList.value?.types || []
  )

  return {
    industryTypes,
    fetchIndustryTypes,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}
