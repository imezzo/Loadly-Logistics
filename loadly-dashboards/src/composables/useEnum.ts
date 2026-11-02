import { useEnumStore } from '@/stores/enum'
import type { EnumItem, EnumsResponse } from '@/types/response/enum'

export function useEnum() {
  const enumStore = useEnumStore()
  const { enums } = storeToRefs(enumStore)

  const userStatus = computed(() => enums.value?.UserStatus ?? [] )

  function getDisplayName(enumType: string, value: number): string {
    const enumItems = enums.value?.[enumType]
    const enumItem = enumItems?.find((item: EnumItem) => item.value === value.toString())

    return enumItem?.label ?? 'N/A'
  }

  async function initEnums(): Promise<void> {
    const { success, data } = await enumStore.getEnums()
    if (success && data) {
      enums.value = data as EnumsResponse | null
    }
  }

  return {
    initEnums,
    enums,
    userStatus,
    getDisplayName,
  }
}

