export interface ExtraFeeTypeOption {
  description?: string | null
  label: string
  optionEnum: number
  requiresValue: boolean
  valueType: string | null
}

export interface ExtraFeeType {
  nameAr: string
  nameEn: string
  options: ExtraFeeTypeOption[]
  requiresThreshold: boolean
  thresholdDescription: string | null
  thresholdLabel: string | null
  thresholdValuePlaceHolder: string | null
  typeId: number
}

export type ExtraFeeTypesResponse = ExtraFeeType[]

export function extraFeeTypesApiResponseAdapter(data: unknown): ExtraFeeType[] {
  return Array.isArray(data) ? (data as ExtraFeeType[]) : []
}
