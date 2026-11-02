const IMAGE_EXTENSION_PATTERN = /\.(png|jpe?g|gif|webp|bmp|svg)$/i

export function extractFileName(path: string): string {
  const parts = path.split('/')

  return parts[parts.length - 1] || path
}

export function isImagePath(path: string): boolean {
  return IMAGE_EXTENSION_PATTERN.test(extractFileName(path))
}
