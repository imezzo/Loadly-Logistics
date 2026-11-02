import type { Pagination } from '@/types/response/pagination'
import { useLoading } from '@/composables/useLoading'

interface PaginatedList<T = unknown> {
  pagination: Pagination
  [key: string]: T[] | Pagination | undefined
}

interface UseLoadMoreOptions<T, K extends string> {
  listRef: Ref<PaginatedList<T> | null>
  itemsKey: K
  fetchPage: (page: number) => Promise<{ success: boolean; data: PaginatedList<T> | null }>
}

export function useLoadMore<T, K extends string = 'items'>(
  options: UseLoadMoreOptions<T, K>
) {
  const { listRef, itemsKey, fetchPage } = options

  const { isLoading, setLoading } = useLoading()

  const hasMore = computed(() => {
    const pagination = listRef.value?.pagination

    return pagination
      ? pagination.page < pagination.pages
      : false
  })

  async function loadMore(): Promise<void> {
    const pagination = listRef.value?.pagination

    if (isLoading.value
      || !pagination
      || pagination.page >= pagination.pages) return

    setLoading(true)

    const { success, data } = await fetchPage(pagination.page + 1)

    if (success && data && listRef.value) {
      const existing = (listRef.value[itemsKey] ?? []) as T[]
      const next = (data[itemsKey] ?? []) as T[]

      listRef.value = {
        ...listRef.value,
        [itemsKey]: [...existing, ...next],
        pagination: data.pagination,
      }
    }

    setLoading(false)
  }

  return { hasMore, loadMore }
}
