import { toNumber, toString } from '@/utils/transformers'
import { formatDateToISO } from '@/utils/date'

export interface VehicleDocumentPayload {
  documentId: number
  expiryDate?: string
  status?: number
  file?: File
  fileUrl?: string | null
}

export interface VehicleWizard {
  brandID?: number
  brandModelID?: number
  capacity: number
  capacityUnitId?: number | null
  colorId: number
  contractorId?: number | null
  coolingUnitSpecification?: string
  documents?: VehicleDocumentPayload[]
  driverId?: number | null
  fuelTypeId?: number | null
  governorateCode?: string
  headVehicleId?: number | null
  initialOdometerReading: number
  insuranceDetails?: string
  insuranceId?: number | null
  internalHeight?: number
  internalLength?: number
  internalWidth?: number
  loadingMechanismType?: number
  makeYear: number
  maximumLoadHeight?: number
  plateLetters: string
  plateNo: string
  purchaseDate?: string | null
  sectorId?: number | null
  status?: number
  tailVehicleIds?: number[]
  tankCapacity?: number | null
  temperatureMonotoringCapability?: string
  temperatureRangeSettings?: string
  vehicleTypeId: number
  vehicleTypeSettings?: string | null
  vinNo?: string | null
}

function appendDocumentsToFormData(formData: FormData, documents: VehicleDocumentPayload[]): void {
  documents.forEach((doc, index) => {
    formData.append(`documents[${index}].documentId`, String(doc.documentId))
    if (doc.expiryDate) {
      formData.append(`documents[${index}].expiryDate`, formatDateToISO(doc.expiryDate))
    }
    if (doc.status != null) {
      formData.append(`documents[${index}].status`, String(doc.status))
    }
    if (doc.file) {
      formData.append(`documents[${index}].file`, doc.file)
    }
  })
}

export function vehicleFormToApiPayload(
  formValues: Record<string, unknown>
): FormData {
  const capacityUnitId =
    toNumber(formValues.capacityUnitId) ??
    (formValues.capacityUnit === 'volume' ? 1 : 2)
  const loadingMechanismType =
    toNumber(formValues.loadingMechanismType) ??
    (() => {
      const loadingMechanismMap: Record<string, number> = {
        crane: 1,
        hydraulic: 2,
        manual: 3,
      }
      const loadingType =
        formValues.openedLoadingMechanismType ?? formValues.boxLoadingMechanismType

        return loadingType ? loadingMechanismMap[String(loadingType)] ?? 1 : 1
    })()

  const maximumLoadHeight = toNumber(
    formValues.openedMaximumLoadHeight ?? formValues.boxMaximumLoadHeight ?? formValues.maximumLoadHeight
  )
  const internalHeight = toNumber(
    formValues.boxInternalHeight ?? formValues.internalHeight
  )
  const internalWidth = toNumber(
    formValues.boxInternalWidth ?? formValues.internalWidth
  )
  const internalLength = toNumber(
    formValues.boxInternalLength ?? formValues.internalLength
  )

  const rawDocs = formValues._documents as {
    documentId?: number
    id?: number
    expiryDate?: string
    expirationDate?: string
    status?: string | number
    fileUrl?: string | null
  }[] | undefined
  const allDocs = rawDocs?.length
    ? rawDocs.map((d, index) => ({
      documentId: d.documentId ?? d.id ?? 0,
      expiryDate: d.expiryDate ?? d.expirationDate,
      status: d.status != null ? Number(d.status) : undefined,
      file: formValues[`document_${index}`] as File | undefined,
      fileUrl: d.fileUrl ?? undefined,
    }))
    : []
  const documents = allDocs.filter((doc) => doc.file || doc.fileUrl)

  const payload: VehicleWizard = {
    plateNo: toString(formValues.plateNo || formValues.plateNumbers),
    plateLetters: toString(formValues.plateLetters).split('').join(' '),
    makeYear: toNumber(formValues.makeYear ?? formValues.modelYear) ?? 0,
    insuranceId: toNumber(formValues.insuranceId) ?? null,
    purchaseDate: formValues.purchaseDate != null ? String(formValues.purchaseDate) : null,
    initialOdometerReading: toNumber(formValues.initialOdometerReading) ?? 0,
    vinNo: toString(formValues.vinNo ?? formValues.vinNumber) || null,
    colorId: toNumber(formValues.colorId ?? formValues.vehicleColorId) ?? 0,
    capacityUnitId: toNumber(formValues.capacityUnitId) ?? capacityUnitId,
    driverId: toNumber(formValues.driverId) ?? null,
    contractorId: formValues.hasContractor
      ? toNumber(formValues.contractorId) ?? null
      : null,
    sectorId: toNumber(formValues.sectorId) ?? null,
    vehicleTypeId: toNumber(formValues.vehicleTypeId) ?? 0,
    tankCapacity: toNumber(formValues.tankCapacity) ?? null,
    vehicleTypeSettings: toString(formValues.vehicleTypeSettings) || null,
    capacity: toNumber(formValues.capacity) ?? 0,
    insuranceDetails: toString(formValues.insuranceDetails),
    temperatureRangeSettings: toString(formValues.temperatureRangeSettings),
    coolingUnitSpecification: toString(
      formValues.coolingUnitSpecifications ?? formValues.coolingUnitSpecification
    ),
    temperatureMonotoringCapability: toString(
      formValues.temperatureMonitoringCapability ??
      formValues.temperatureMonotoringCapability
    ),
    maximumLoadHeight: maximumLoadHeight ?? undefined,
    internalHeight: internalHeight ?? undefined,
    internalWidth: internalWidth ?? undefined,
    internalLength: internalLength ?? undefined,
    brandID: toNumber(formValues.brandID) ?? undefined,
    brandModelID: toNumber(formValues.brandModelID ?? formValues.vehicleModelId) ?? undefined,
    fuelTypeId: toNumber(formValues.fuelTypeId) ?? null,
    loadingMechanismType,
    status: toNumber(formValues.status) ?? 1,
    headVehicleId: toNumber(formValues.headVehicleId) ?? null,
    tailVehicleIds: (formValues.tailVehicles as { id: number }[] | undefined)?.map((t) => t.id) ?? undefined,
    documents: documents.length ? documents : undefined,
  }

  const formData = new FormData()

  Object.entries(payload).forEach(([key, value]) => {
    if (value === undefined || value === null) return

    if (key === 'documents' && Array.isArray(value)) {
      appendDocumentsToFormData(formData, value as VehicleDocumentPayload[])
    } else if (key === 'tailVehicleIds' && Array.isArray(value)) {
      value.forEach((id) => formData.append(key, String(id)))
    } else {
      formData.append(key, String(value))
    }
  })

  return formData
}
