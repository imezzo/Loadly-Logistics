export function formatTimer(secondsTimer: number): string {
  const minutes = Math.floor(secondsTimer / 60)
  const seconds = secondsTimer % 60
  const formattedMinutes = minutes.toString().padStart(2, '0')
  const formattedSeconds = seconds.toString().padStart(2, '0')

  return `${formattedMinutes}:${formattedSeconds}`
}

export function timerUnites(secondsTimer: number): { minutes: string, seconds: string } {
  const minutes = Math.floor(secondsTimer / 60)
  const seconds = secondsTimer % 60
  const formattedMinutes = minutes.toString().padStart(2, '0')
  const formattedSeconds = seconds.toString().padStart(2, '0')

  return { minutes: formattedMinutes, seconds: formattedSeconds }
}

/** Break ms into calendar-ish units (30-day months). */
export function getDurationParts(ms: number): {
  months: number
  days: number
  hours: number
  minutes: number
} {
  if (!Number.isFinite(ms) || ms <= 0) {
    return { months: 0, days: 0, hours: 0, minutes: 0 }
  }

  const totalMinutes = Math.floor(ms / 60_000)
  const minutesPerDay = 24 * 60
  const minutesPerMonth = 30 * minutesPerDay

  const months = Math.floor(totalMinutes / minutesPerMonth)
  const days = Math.floor((totalMinutes % minutesPerMonth) / minutesPerDay)
  const hours = Math.floor((totalMinutes % minutesPerDay) / 60)
  const minutes = totalMinutes % 60

  return { months, days, hours, minutes }
}

export function formatTime(date: Date, locale: string, options?: Intl.DateTimeFormatOptions): string {
  return date.toLocaleTimeString(locale, {
    hour: 'numeric',
    minute: '2-digit',
    hour12: true,
    ...options,
  })
}

export function formatDate(date: Date, locale: string, options?: Intl.DateTimeFormatOptions): string {
  return date.toLocaleDateString(locale, {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    ...options,
  })
}
