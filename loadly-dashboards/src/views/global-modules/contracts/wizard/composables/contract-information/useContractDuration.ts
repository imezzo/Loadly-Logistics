import type { Ref } from 'vue'

const DURATION_OPTIONS = [1, 3, 6, 9, 12] as const

type ContractFormRef = Ref<{
  form?: {
    values: Record<string, unknown>
    setFieldValue: (name: string, value: unknown) => void
  }
} | null>

function getMonthsBetween(startStr: string, endStr: string): number | null {
  const start = new Date(startStr)
  const end = new Date(endStr)
  if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) return null
  const months = (end.getFullYear() - start.getFullYear()) * 12
    + (end.getMonth() - start.getMonth())

  return months >= 0 ? months : null
}

function addMonthsToDate(dateStr: string, months: number): string {
  const date = new Date(dateStr)
  if (Number.isNaN(date.getTime())) return ''
  date.setMonth(date.getMonth() + months)

  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
}

export function useContractDuration(contractFormRef: ContractFormRef) {
  const selectedDurationMonths = ref<number | null>(null)

  const hasStartDate = computed(() => {
    const values = contractFormRef.value?.form?.values as Record<string, unknown> | undefined

    return (values?.startDate as string) ?? ''
  })

  const startDateFromForm = computed(
    () => (contractFormRef.value?.form?.values as Record<string, unknown> | undefined)?.startDate as string | undefined
  )

  const endDateFromForm = computed(
    () => (contractFormRef.value?.form?.values as Record<string, unknown> | undefined)?.endDate as string | undefined
  )

  const activeDurationMonths = computed(() => {
    if (selectedDurationMonths.value !== null) return selectedDurationMonths.value
    const start = startDateFromForm.value
    const end = endDateFromForm.value
    if (!start || !end) return null
    const months = getMonthsBetween(start, end)

    return months !== null && DURATION_OPTIONS.includes(months as (typeof DURATION_OPTIONS)[number])
      ? months
      : null
  })

  function setEndDate(months: number): void {
    const start = hasStartDate.value
    if (!start) return
    selectedDurationMonths.value = months
    const endStr = addMonthsToDate(start, months)
    if (endStr) contractFormRef.value?.form?.setFieldValue('endDate', endStr)
  }

  watch(startDateFromForm, (newStart) => {
    if (selectedDurationMonths.value === null || !newStart) return
    const endStr = addMonthsToDate(newStart, selectedDurationMonths.value)
    if (endStr) contractFormRef.value?.form?.setFieldValue('endDate', endStr)
  })

  watch(endDateFromForm, (newEnd) => {
    if (selectedDurationMonths.value === null) return
    const start = startDateFromForm.value
    if (!start || !newEnd) return
    const months = getMonthsBetween(start, newEnd)
    if (months !== selectedDurationMonths.value) {
      selectedDurationMonths.value = null
    }
  })

  watch(
    [startDateFromForm, endDateFromForm],
    ([start, end]) => {
      if (selectedDurationMonths.value !== null) return
      if (!start || !end) return
      const months = getMonthsBetween(start as string, end as string)
      if (months !== null && DURATION_OPTIONS.includes(months as (typeof DURATION_OPTIONS)[number])) {
        selectedDurationMonths.value = months
      }
    },
    { immediate: true }
  )

  function getDuration(endDate?: string, startDate?: string): string {
    if (!endDate || !startDate) return ''

    const start = new Date(startDate)
    const end = new Date(endDate)

    if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) return ''
    if (end <= start) return ''

    const diffTime = Math.abs(end.getTime() - start.getTime())
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
    const years = Math.floor(diffDays / 365)
    const months = Math.floor((diffDays % 365) / 30)
    const days = diffDays % 30

    const parts: string[] = []

    if (years > 0) {
      parts.push(`${years} ${years === 1 ? 'year' : 'years'}`)
    }

    if (months > 0) {
      parts.push(`${months} ${months === 1 ? 'month' : 'months'}`)
    }

    if (days > 0 && years === 0) {
      parts.push(`${days} ${days === 1 ? 'day' : 'days'}`)
    }

    return parts.length > 0 ? parts.join(', ') : ''
  }

  return {
    DURATION_OPTIONS,
    activeDurationMonths,
    getDuration,
    hasStartDate,
    setEndDate,
  }
}
