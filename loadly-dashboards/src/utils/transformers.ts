export function hasValue(value?: string | number | null): boolean {
  return !!value || value === 0
}

export function hasTruthyValue(value?: string | number | null): string | number | undefined {
  return value ? value : undefined
}

export function getValidProperty<T>(
  key: string,
  value?: T | null,
): Partial<Record<string, T>> {
  return value ? { [key]: value } : {}
}

export function toNumber(value: unknown): number | null {
  if (!value) return null
  const numericValue = Number(value)

  return Number.isFinite(numericValue) ? numericValue : null
}

export function toString(value: unknown): string {
  if (!value) return ''

  return String(value).trim()
}
