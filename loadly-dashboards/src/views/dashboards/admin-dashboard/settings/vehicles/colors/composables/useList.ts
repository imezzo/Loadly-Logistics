import type { Color } from '@/types/admin-dashboard/response/color'
import type { Pagination } from '@/types/response/pagination'
import { useColorStore } from '@/stores/admin-dashboard/color'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const colorStore = useColorStore()
  const { colorsList } = storeToRefs(colorStore)

  const { isLoading, setLoading } = useLoading()

  const colorPagination = computed<Pagination | null>(
    () => colorsList.value?.pagination || null
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(colorPagination)

  async function fetchColors() {
    setLoading(true)

    const { success, data } = await colorStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    colorsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchColors()
  })

  watch(currentPage, () => {
    fetchColors()
  })

  const colors = computed<Color[]>(() => colorsList.value?.colors || [])

  return {
    colors,
    fetchColors,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}
