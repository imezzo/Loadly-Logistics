import { api } from "@/utils/api"
import type { EnumsResponse } from "@/types/response/enum"
import { enumsResponseAdapter } from "@/types/response/enum"

export const useEnumStore = defineStore('enum', () => {
  const enums = ref<EnumsResponse | null>(null)

  async function getEnums() {
    return await api({
      url: import.meta.env.VITE_API_BASE_URL + '/enums',
      method: 'GET',
      responseAdapter: enumsResponseAdapter,
    })
  }

  return {
    enums,
    getEnums,
  }
})

