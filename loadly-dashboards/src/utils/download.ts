interface TriggerDownloadOptions {
  prefix?: string
  fileName?: string
  extension?: string
}

export function triggerDownload(blob: Blob, options: TriggerDownloadOptions): void {
  const url = URL.createObjectURL(blob)
  const link = document.createElement("a")

  link.href = url
  link.download = options.fileName || options.prefix + new Date().toISOString() + options.extension
  link.style.display = "none"

  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  URL.revokeObjectURL(url)
}
