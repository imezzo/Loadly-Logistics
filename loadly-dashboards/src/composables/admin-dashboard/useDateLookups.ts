import type { SelectOption } from '@/types/ui'

export function useDateLookups() {
  const { t } = useI18n()

  const yearLookup = computed<SelectOption[]>(() => {
    const currentYear = new Date().getFullYear()

    return Array.from({ length: 100 }, (_, index) => {
      const year = String(currentYear - index)

      return { value: year, label: year }
    })
  })

  const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']
  const monthLookup = computed<SelectOption[]>(() => [
    { value: '', label: t('content.chooseMonth') },
    ...months.map((month, index) => ({ value: (index + 1).toString(), label: month })),
  ])

  return { yearLookup, monthLookup, months }
}

