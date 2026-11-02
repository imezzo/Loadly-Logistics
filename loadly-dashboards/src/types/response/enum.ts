export interface EnumItem {
  value: string
  label: string
}

export type EnumsResponse = Record<string, EnumItem[]>

export interface ApiEnumItem {
  name: string
  value: number
  displayName: string
}

export function enumItemAdapter(item: ApiEnumItem): EnumItem {
  return {
    value: item.value.toString(),
    label: item.displayName,
  }
}

export function enumsResponseAdapter(response: Record<string, ApiEnumItem[]>): EnumsResponse {
  const enums: EnumsResponse = {}

  for (const [key, items] of Object.entries(response)) {
    enums[key] = items.map(item => enumItemAdapter(item))
  }

  return enums
}
