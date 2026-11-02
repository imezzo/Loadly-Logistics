import { api } from '@/utils/api'
import type { ExtractPlaceData } from '@/types/admin-dashboard/response/maps'

export const useMapsStore = defineStore('maps', () => {
  const extractedPlace = ref<ExtractPlaceData | null>(null)

  async function extractPlace(url: string) {
    const response = await api({
      url: 'maps/extract-place',
      method: 'GET',
      params: { url },
    })

    extractedPlace.value = response.data

    return response
  }

  return {
    extractedPlace,
    extractPlace,
  }
})
