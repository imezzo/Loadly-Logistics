export enum DriverStatus {
  AVAILABLE = 1,
  BUSY = 2,
  ON_BREAK = 3,
  OFFLINE = 4,
  OFF_DUTY = 5,
}

export interface DriversFilter {
  contractorIds: string[]
  search: string
  workingStatuses: DriverStatus | ''
  licenseCategory?: string
  rate?: string
}
