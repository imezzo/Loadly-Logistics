export enum MonthlyContractStatus {
  Draft,
  Finalized,
  PartiallyInvoiced
}

export enum ContractSlotDayDisposition {
  NotActive = 0,
  OutOfService = 1,
  Working = 2,
  Unknown = 3,
  HalfWorking = 4,
}

export enum ContractSlotDayDispositionSource {
  Computed = 0,
  Manual = 1,
}
