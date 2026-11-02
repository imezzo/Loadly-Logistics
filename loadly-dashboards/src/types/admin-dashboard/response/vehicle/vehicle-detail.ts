import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
import type { ApiDocumentInfo } from "../document"
import { getLocalizedValue } from '@/utils/i18n'
import { sectorAdapter, type ApiSector } from '../sector/sector'
import { VehicleStatus } from './vehicle.enums'

export interface ApiVehicleDetailColor {
  colorHash: string
  id: number
  name: string
  nameAr: string
  nameEn: string
  status: number
}

export interface ApiVehicleDetailVehicleType {
  category: number | string
  id: number
  name: string
  nameAr: string
  nameEn: string
  status: number | string
  vehicleUnitType: number | string | null
}

export interface ApiVehicleDetailExtra {
  coolingUnitSpecification?: string | null
  governorateCode?: string
  insuranceDetails?: string | null
  internalHeight?: number
  internalLength?: number
  internalWidth?: number
  loadingMechanismType?: number
  maximumLoadHeight?: number
  temperatureMonotoringCapability?: string | null
  temperatureRangeSettings?: string
  temperatureRangeSettng?: string
  vehicleId?: number
}

export interface ApiVehicleMinimal {
  displayText?: string
  id: number
  plateLetters?: string
  plateNo?: string
  vehicleTypeName?: string
}

export interface ApiDriverMinimal {
  displayName?: string
  fullName?: string
  fullNameAr?: string
  fullNameEn?: string
  id: number
}

export interface ApiVehicleDetailDocument {
  documentId: number
  expiryDate?: string
  fileUrl?: string | null
  status?: number
}

export interface ApiContractor {
  email: string
  fullName: string
  id: number
  name: string
  phone: string
  userName: string
}

export interface ApiPlateChangeHistory {
  addedBy: number
  plateLetters: string
  plateNo: string
  supersededAt: string
}

export interface ApiVehicleDetail {
  id: number
  completedTripsCount: number
  plateNo: string
  plateLetters: string
  makeYear: number
  insuranceId: number | null
  purchaseDate: string | null
  initialOdometerReading: number
  vinNo: string | null
  colorId: number
  capacityUnitId: number | null
  driverId: number | null
  vehicleTypeId: number
  fuelTypeId?: number | null
  sector?: ApiSector | null
  tankCapacity: number | null
  vehicleTypeSettings: string | null
  capacity: number
  waitingForLoading: boolean
  contractor: ApiContractor | null
  status: VehicleStatus
  addedBy: number | null
  createdAt: string
  updatedAt: string
  headVehicleId: number | null
  tailVehicleId: number | null
  vehicleInsurance: unknown
  color: ApiVehicleDetailColor | null
  vehicleType: ApiVehicleDetailVehicleType | null
  headVehicle: ApiVehicleMinimal | null
  tailVehicles?: ApiVehicleMinimal[]
  tailVehicle?: ApiVehicleMinimal | null
  driver: ApiDriverMinimal | null
  vehicleCapacityUnit: unknown
  vehicleDocuments: ApiVehicleDetailDocument[]
  vehicleDetail?: ApiVehicleDetailExtra | null
  vehicleBrandModel?: { id: number; name?: string; vehicleBrandId?: number } | null
  brandModelID: number
  plateChangeHistory?: ApiPlateChangeHistory[]
}

export interface VehicleFormData {
  id?: number
  contractorId?: number | null
  sectorId?: string | number | null
  vehicleUnitType?: string
  vehicleTypeCategory?: string
  vehicleTypeId?: number | null
  plateLetters?: string
  plateNo?: string
  plateNumbers?: string
  governorateCode?: string
  vehicleModelId?: string
  makeYear?: string | number
  modelYear?: string | number
  capacityUnit?: string
  capacity?: string | number
  colorId?: string | number
  vehicleColorId?: string | number
  vinNo?: string
  vinNumber?: string
  initialOdometerReading?: string | number
  insuranceDetails?: string
  purchaseDate?: string | null
  insuranceId?: number | null
  capacityUnitId?: number | null
  driverId?: number | null
  fuelTypeId?: number | null
  tankCapacity?: number | null
  vehicleTypeSettings?: string | null
  headVehicleId?: number | null
  status?: VehicleStatus
  temperatureRangeSettings?: string
  coolingUnitSpecifications?: string
  temperatureMonitoringCapability?: string
  openedMaximumLoadHeight?: string
  boxInternalHeight?: string
  boxInternalWidth?: string
  boxInternalLength?: string
  boxLoadingMechanismType?: string
  boxMaximumLoadHeight?: string
  openedLoadingMechanismType?: string
  brandID?: string | number
  brandModelID?: string | number
  loadingMechanismType?: number
  maximumLoadHeight?: string | number
  internalHeight?: string | number
  internalWidth?: string | number
  internalLength?: string | number
  plateChangeHistory?: ApiPlateChangeHistory[]
  headVehicle?: { id: number; plateNo?: string; plateLetters?: string; displayText?: string; vehicleTypeName?: string } | null
  tailVehicles?: { id: number; plateNo?: string; plateLetters?: string; displayText?: string; vehicleTypeName?: string }[]
  currentDriver?: { id: number; fullName?: string; displayName?: string } | null
  _documents?: ApiDocumentInfo[]
}

function mapApiVehicleToForm(v: ApiVehicleMinimal | null | undefined): VehicleFormData['headVehicle'] {
  if (!v) return null

  return {
    id: v.id,
    plateNo: v.plateNo ?? '',
    plateLetters: v.plateLetters ?? '',
    displayText: v.displayText,
    vehicleTypeName: v.vehicleTypeName,
  }
}

function mapApiDriverToForm(driver: ApiDriverMinimal | null | undefined): VehicleFormData['currentDriver'] {
  if (!driver) return null

  const fullName = getLocalizedValue(driver?.fullNameEn ?? '', driver?.fullNameAr ?? '')

  return {
    id: driver.id,
    fullName,
    displayName: fullName,
  }
}

function formatVehicleDisplayTextFromDetail(api: ApiVehicleDetail): string {
  const plateInfo = `${api.plateNo} ${api.plateLetters}`.trim()
  const maintenanceDate = api.purchaseDate
    ? new Date(api.purchaseDate).toLocaleDateString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    })
    : null
  const odometer = api.initialOdometerReading.toLocaleString('en-US')
  const statusText = api.status === VehicleStatus.Available ? 'Available' : 'Unavailable'

  const parts = [
    `Car ID - ${plateInfo}`,
    maintenanceDate ? `maintenance ${maintenanceDate}` : null,
    `${odometer} KM`,
    statusText,
  ].filter(Boolean)

  return parts.join(' - ')
}

export function vehicleDetailToVehicleAdapter(api: ApiVehicleDetail): Vehicle {
  const driver = api.driver
  const driverPhone = (driver as { phone?: string })?.phone ?? null

  return {
    currentServiceFacility: null,
    tail: {
      id: undefined,
      plateLetters: undefined,
      plateNo: undefined,
      vehicleTypeId: undefined,
      vehicleType: undefined,
    },
    companyName: api.contractor?.name ?? '',
    sector: api.sector ? sectorAdapter(api.sector) : null,
    vehicleFacilityVisits: [],
    vehicleUnitType: String(api.vehicleType?.vehicleUnitType ?? ''),
    assignedParkingFacility: null,
    id: api.id,
    driverId: api.driverId,
    contract: null,
    plateNo: api.plateNo,
    plateLetters: api.plateLetters,
    makeYear: api.makeYear,
    initialOdometerReading: api.initialOdometerReading,
    vinNo: api.vinNo,
    status: api.status,
    headVehicleId: api.headVehicleId,
    waitingForLoading: api.waitingForLoading,
    contractorId: api.contractor?.id ?? null,
    tailVehicleId: api.tailVehicles?.[0]?.id ?? api.tailVehicle?.id ?? null,
    displayText: formatVehicleDisplayTextFromDetail(api),
    vehicleTypeName: api.vehicleType?.nameEn ?? api.vehicleType?.name ?? '',
    brandModelName: api.vehicleBrandModel?.name ?? '',
    brandModelId: api.brandModelID,
    driverName: getLocalizedValue(driver?.fullNameEn ?? '', driver?.fullNameAr ?? ''),
    driverPhone,
    capacityUnitId: api.capacityUnitId,
    completedTripsCount: api.completedTripsCount,
  }
}

export function vehicleDetailToFormAdapter(api: ApiVehicleDetail): VehicleFormData {
  const detail = api.vehicleDetail
  const brandModelId = api.brandModelID ?? api.vehicleBrandModel?.id
  const brandId = api.vehicleBrandModel?.vehicleBrandId

  return {
    id: api.id,
    vehicleUnitType: api.vehicleType?.vehicleUnitType != null ? String(api.vehicleType.vehicleUnitType) : undefined,
    vehicleTypeCategory: api.vehicleType?.category != null ? String(api.vehicleType.category) : undefined,
    vehicleTypeId: api.vehicleTypeId,
    fuelTypeId: api.fuelTypeId != null ? Number(api.fuelTypeId) : null,
    plateLetters: api.plateLetters.replace(/\s/g, '').trim() ?? '',
    plateNo: api.plateNo ?? '',
    plateNumbers: api.plateNo ?? '',
    plateChangeHistory: api.plateChangeHistory ?? [],
    governorateCode: detail?.governorateCode ?? '',
    brandID: brandId != null ? String(brandId) : '',
    brandModelID: brandModelId != null ? String(brandModelId) : '',
    makeYear: api.makeYear,
    modelYear: api.makeYear,
    capacityUnit: api.capacityUnitId === 1 ? 'volume' : 'weight',
    capacityUnitId: api.capacityUnitId,
    capacity: api.capacity,
    colorId: api.colorId,
    vehicleColorId: api.colorId,
    vinNo: api.vinNo ?? '',
    vinNumber: api.vinNo ?? '',
    initialOdometerReading: api.initialOdometerReading,
    insuranceDetails: detail?.insuranceDetails ?? '',
    purchaseDate: api.purchaseDate ?? null,
    insuranceId: api.insuranceId,
    driverId: api.driverId,
    tankCapacity: api.tankCapacity,
    contractorId: api.contractor?.id ?? null,
    sectorId: api.sector?.id ?? null,
    vehicleTypeSettings: api.vehicleTypeSettings,
    headVehicleId: api.headVehicleId,
    headVehicle: api.headVehicle ? mapApiVehicleToForm(api.headVehicle) : null,
    tailVehicles: (() => {
      let sources: ApiVehicleMinimal[] = []
      if (api.tailVehicles?.length) sources = api.tailVehicles
      else if (api.tailVehicle) sources = [api.tailVehicle]

      return sources.map(mapApiVehicleToForm) as VehicleFormData['tailVehicles']
    })(),
    currentDriver: api.driver ? mapApiDriverToForm(api.driver) : null,
    status: api.status,
    temperatureRangeSettings: detail?.temperatureRangeSettng ?? detail?.temperatureRangeSettings ?? '',
    coolingUnitSpecifications: detail?.coolingUnitSpecification ?? '',
    temperatureMonitoringCapability: detail?.temperatureMonotoringCapability ?? '',
    loadingMechanismType: detail?.loadingMechanismType,
    maximumLoadHeight: detail?.maximumLoadHeight,
    openedMaximumLoadHeight: detail?.maximumLoadHeight != null ? String(detail.maximumLoadHeight) : '',
    boxInternalHeight: detail?.internalHeight != null ? String(detail.internalHeight) : '',
    boxInternalWidth: detail?.internalWidth != null ? String(detail.internalWidth) : '',
    boxInternalLength: detail?.internalLength != null ? String(detail.internalLength) : '',
    boxLoadingMechanismType: detail?.loadingMechanismType != null ? String(detail.loadingMechanismType) : '',
    boxMaximumLoadHeight: detail?.maximumLoadHeight != null ? String(detail.maximumLoadHeight) : '',
    openedLoadingMechanismType: '',
    internalHeight: detail?.internalHeight,
    internalWidth: detail?.internalWidth,
    internalLength: detail?.internalLength,
    _documents: api.vehicleDocuments?.map((d) => ({
      id: d.documentId,
      documentId: d.documentId,
      expirationDate: d.expiryDate,
      status: d.status?.toString(),
      label: '',
      required: false,
      fileUrl: d.fileUrl ?? null,
    })),
  }
}
