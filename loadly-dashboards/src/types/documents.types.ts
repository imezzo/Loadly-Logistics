export interface DocumentUploaderItem {
  documentId?: number
  expirationDate?: string
  file?: File
  fileUrl?: string | null
  id?: number
  label?: string
  required?: boolean
  status?: string
  uploadProgress?: number
  uploadState?: 'idle' | 'uploading' | 'success' | 'error'
}

export interface ApiDocumentUploaderItem {
  addedBy: number
  contractorId: number
  createdAt: string
  document: {
    entityId: string
    id: number
    nameAr: string
    nameEn: string
    required: boolean
    status: string
  }
  documentId: number
  expiryDate: string | null
  fileUrl: string | null
  id: number
  path: string | null
  status: number
  updatedAt: string
}
