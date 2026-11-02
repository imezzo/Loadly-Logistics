import type { User } from '@/types/response/user'
import { watchDebounced } from '@vueuse/core'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'
import { useUserStore } from '@/stores/admin-dashboard/user'

export function useUsersList() {
  const userStore = useUserStore()
  const { filters, usersList } = storeToRefs(userStore)

  const { isLoading, setLoading } = useLoading()
  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(computed(() => usersList.value?.pagination || null))

  const users = computed<User[]>(() => usersList.value?.users || [])

  async function fetchUsers() {
    if (isLoading.value) return
    setLoading(true)

    const { success, data } = await userStore.getList({
      search: filters.value.search,
      status: filters.value.status.toString() || undefined,
      roleIds: filters.value.roleIds,
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    usersList.value = success ? data : null
    setLoading(false)
  }

  watchDebounced(
    () => filters.value.search,
    () => {
      currentPage.value = 1
      fetchUsers()
    },
    { debounce: 500 }
  )

  watch(
    [
      () => filters.value,
      itemsPerPage
    ],
    () => {
      currentPage.value = 1
      fetchUsers()
    }
  )

  watch(currentPage, fetchUsers)

  return {
    fetchUsers,
    filters,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    resetFilters: userStore.resetFilters,
    setPage,
    updateFilters: userStore.updateFilters,
    updateItemsPerPage,
    users,
  }
}
