import { useLoadingStore } from "@/stores/admin-dashboard/loading"

export function useLoading() {
  const loadingStore = useLoadingStore()
  const { isLoading } = storeToRefs(loadingStore)

  function setLoading(value: boolean): void {
    loadingStore.setLoading(value)
  }

  return {
    isLoading,
    setLoading,
  }
}
