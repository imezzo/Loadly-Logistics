export interface ApiResponse<T> {
  additional?: Record<string, unknown>
  blob?: Blob
  data: T
  errors?: Record<string, string[]>
  message: string
  success: boolean
}
