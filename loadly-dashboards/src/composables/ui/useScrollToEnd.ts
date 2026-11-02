interface ScrollToEndOptions {
  threshold?: number
}

export function createScrollToEndHandler(
  onReachEnd: () => void,
  options: ScrollToEndOptions = {}
) {
  const { threshold = 0.9 } = options

  return (event: Event): void => {
    const target = event.target as HTMLElement | null
    if (!target) return

    const { scrollTop, scrollHeight, clientHeight } = target
    if (scrollHeight <= 0) return

    const scrollPercentage = (scrollTop + clientHeight) / scrollHeight
    if (scrollPercentage >= threshold) {
      onReachEnd()
    }
  }
}
