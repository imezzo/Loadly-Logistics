export function useUploadSimulator(
  uploadProgress: Ref<number | undefined>,
  uploadState?: Ref<'idle' | 'uploading' | 'success' | 'error' | undefined>,
  onComplete?: () => void
) {
  let uploadInterval: ReturnType<typeof setInterval> | null = null

  function startSimulation(): void {
    if (uploadInterval) {
      clearInterval(uploadInterval)
    }

    uploadProgress.value = 0
    if (uploadState) {
      uploadState.value = 'uploading'
    }

    uploadInterval = setInterval(() => {
      uploadProgress.value = (uploadProgress.value || 0) + 10
      if ((uploadProgress.value ?? 0) >= 100) {
        stopSimulation()
        if (uploadState) {
          uploadState.value = 'success'
        }
        uploadProgress.value = 100
        onComplete?.()
      }
    }, 400)
  }

  function stopSimulation(): void {
    if (uploadInterval) {
      clearInterval(uploadInterval)
      uploadInterval = null
    }
  }

  onBeforeUnmount(() => {
    stopSimulation()
  })

  return {
    startSimulation,
    stopSimulation,
  }
}
