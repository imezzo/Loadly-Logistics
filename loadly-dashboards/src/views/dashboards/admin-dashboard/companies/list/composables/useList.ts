import type { Company } from '@/types/admin-dashboard/response/company'
import { useCompanyStore } from '@/stores/admin-dashboard/company'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const companyStore = useCompanyStore()
  const { companiesList } = storeToRefs(companyStore)

  const { isLoading, setLoading } = useLoading()

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => companiesList.value?.pagination || null))

  async function fetchCompanies() {
    setLoading(true)

    const { success, data } = await companyStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    companiesList.value = success ? data : null
    setLoading(false)
  }

  watch(
    [itemsPerPage],
    () => {
      currentPage.value = 1
      fetchCompanies()
    },
  )

  watch(currentPage, () => {
    fetchCompanies()
  })

  const companies = computed<Company[]>(() => companiesList.value?.companies || [])

  return {
    companies,
    fetchCompanies,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}

