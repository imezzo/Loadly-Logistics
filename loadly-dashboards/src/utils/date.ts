const monthNames = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
]

function getPaddedNumber(n: number): string {
  return n.toString().padStart(2, '0')
}

function toLocalDateString(date: Date): string {
  return `${date.getFullYear()}-${getPaddedNumber(date.getMonth() + 1)}-${getPaddedNumber(date.getDate())}`
}

function toIsoDateString(date: Date): string {
  return date.toISOString().split('T')[0] || ''
}

export function formatDate(dateString: string): string {
  const date = new Date(dateString)
  const day = getPaddedNumber(date.getDate())
  const month = monthNames[date.getMonth()]
  const year = date.getFullYear()

  return `${day} ${month} ${year}`
}

export function formatDateToISO(dateString: string, hasTime = false): string {
  const dateObj = new Date(dateString)
  if (!hasTime) {
    dateObj.setHours(0, 0, 0, 0)
  }

  return dateObj.toISOString()
}

export function getMinDate(options?: { daysFromToday?: number; baseDate?: Date }): string {
  const { daysFromToday = 1, baseDate = new Date() } = options || {}
  const date = new Date(baseDate)
  date.setDate(date.getDate() + daysFromToday)
  date.setHours(0, 0, 0, 0)

  return toIsoDateString(date)
}

export function getYesterdayDate(): string {
  const date = new Date()
  date.setDate(date.getDate() - 1)

  return toLocalDateString(date)
}

export function isBeforeToday(value: string | Date): boolean {
  const date = value instanceof Date ? value : new Date(value)
  if (Number.isNaN(date.getTime())) return false

  const today = new Date()
  today.setHours(0, 0, 0, 0)

  return date < today
}

export function extractISODate(isoDateString?: string): string | undefined {
  if (!isoDateString) return undefined

  return isoDateString.split('T')[0]
}

export function formatDateLocal(dateString: string): string {
  const date = new Date(dateString)
  if (Number.isNaN(date.getTime())) return ''

  return toLocalDateString(date)
}

export function toDateTimeLocal(isoDateString?: string): string {
  if (!isoDateString) return ''

  const date = new Date(isoDateString)
  if (Number.isNaN(date.getTime())) return ''

  return `${toLocalDateString(date)}T${getPaddedNumber(date.getHours())}:${getPaddedNumber(date.getMinutes())}`
}

export function shiftDateTimeLocalByDays(dateString: string, days: number): string {
  if (!dateString || !Number.isFinite(days)) return ''

  const date = new Date(dateString)
  if (Number.isNaN(date.getTime())) return ''

  date.setDate(date.getDate() + days)

  return `${toLocalDateString(date)}T${getPaddedNumber(date.getHours())}:${getPaddedNumber(date.getMinutes())}`
}

export function getIsoDateTime(
  date: string | null,
  time: [number, number, number, number]
): string {
  const adaptedDate = new Date(date ?? '')

  if (!Number.isNaN(adaptedDate.getTime())) {
    adaptedDate.setHours(...time)
  }

  return adaptedDate.toISOString()
}

export function isoToDate(isoDateString: string): string {
  return toIsoDateString(new Date(isoDateString))
}
