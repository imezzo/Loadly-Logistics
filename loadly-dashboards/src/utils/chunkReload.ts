const CHUNK_RELOAD_KEY = 'chunk-reload'

const CHUNK_ERROR_PATTERNS = [
  'Failed to fetch dynamically imported module',
  'error loading dynamically imported module',
  'Importing a module script failed',
  'Unable to preload CSS',
]

let pendingNavigationUrl: string | null = null

export function isChunkLoadError(error: unknown): boolean {
  const message = error instanceof Error ? error.message : String(error)

  return CHUNK_ERROR_PATTERNS.some((pattern) => message.includes(pattern))
}

export function setPendingNavigationUrl(url: string): void {
  pendingNavigationUrl = url
}

export function clearPendingNavigationUrl(): void {
  pendingNavigationUrl = null
}

export function clearChunkReloadFlag(): void {
  sessionStorage.removeItem(CHUNK_RELOAD_KEY)
}

export function reloadAppOnce(url?: string): void {
  if (sessionStorage.getItem(CHUNK_RELOAD_KEY)) return

  sessionStorage.setItem(CHUNK_RELOAD_KEY, '1')

  const destination = url ?? pendingNavigationUrl

  if (destination) {
    window.location.assign(destination)

    return
  }

  window.location.reload()
}
