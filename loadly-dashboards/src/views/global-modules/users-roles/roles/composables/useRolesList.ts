import type { Role } from '@/types/response/role'
import { useRoleStore } from '@/stores/role'
import { useLoading } from '@/composables/useLoading'
import { useLoadMore } from '@/composables/useLoadMore'
import { usePagination } from '@/composables/usePagination'
import { watchDebounced } from '@vueuse/core'

export function useRolesList() {
  const roleStore = useRoleStore()
  const { rolesList, search } = storeToRefs(roleStore)

  const { isLoading, setLoading } = useLoading()

  const {
    currentPage,
    itemsPerPage,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => rolesList.value?.pagination || null))

  const {
    hasMore: hasMoreRoles,
    loadMore: loadMoreRoles,
  } = useLoadMore<Role, 'roles'>({
    listRef: rolesList,
    itemsKey: 'roles',
    fetchPage: (page) =>
      roleStore.getList({
        page,
        pageSize: itemsPerPage.value,
        search: search.value,
      }),
  })

  async function fetchRoles() {
    setLoading(true)

    const { success, data } = await roleStore.getList({
      page: currentPage.value,
      pageSize: itemsPerPage.value,
      search: search.value,
    })

    rolesList.value = success ? data : null
    setLoading(false)
  }

  watchDebounced(
    search,
    () => {
      currentPage.value = 1
      fetchRoles()
    },
    { debounce: 500 }
  )

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchRoles()
  })

  watch(currentPage, () => {
    fetchRoles()
  })

  const roles = computed<Role[]>(() => rolesList.value?.roles || [])

  const pagination = computed(() => {
    const api = rolesList.value?.pagination

    return {
      page: currentPage.value,
      size: itemsPerPage.value,
      pages: api?.pages || 0,
      items: api?.items || 0,
    }
  })

  return {
    fetchRoles,
    hasMoreRoles,
    isLoading,
    loadMoreRoles,
    nextPage,
    pagination,
    prevPage,
    roles,
    search,
    setPage,
    updateItemsPerPage,
  }
}

