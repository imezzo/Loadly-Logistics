import {
  tripChargesToApiPayload,
  tripListRequestAdapter,
  tripWizardRequestAdapter,
} from "@/types/admin-dashboard/request/trip/trip"
import type {
  TripFilter,
  TripListRequest,
  TripWizard
} from "@/types/admin-dashboard/request/trip/trip"
import type { Vehicle } from "@/types/admin-dashboard/response/vehicle/vehicle"
import { DispatchingMode, TripCustomsStatus, tripListResponseAdapter, tripResponseAdapter } from "@/types/admin-dashboard/response/trip/trip"
import type { TripList, TripTrackItem, TripWizardTripCharge } from "@/types/admin-dashboard/response/trip/trip"
import { api } from "@/utils/api"
import type { UpdateTripChargesInvoiceTypeRequest } from "@/types/admin-dashboard/request/trip/trip-invoice"
import { updateTripChargesInvoiceTypeRequestAdapter } from "@/types/admin-dashboard/request/trip/trip-invoice"
import type { TripAuditRequest } from "@/types/admin-dashboard/request/trip/trip-audit"
import { tripAuditRequestAdapter } from "@/types/admin-dashboard/request/trip/trip-audit"
import type { TripInvoiceSummary } from "@/types/admin-dashboard/response/trip/trip-invoice"
import { tripInvoiceSummaryResponseAdapter } from "@/types/admin-dashboard/response/trip/trip-invoice"
import type { TripBulkCreateRequest } from "@/types/admin-dashboard/request/trip/bulk-import"
import { tripBulkCreateRequestAdapter } from "@/types/admin-dashboard/request/trip/bulk-import"
import type { PushToContractorsRequest } from "@/types/admin-dashboard/request/trip/push-to-contractors"
import { pushToContractorsRequestAdapter } from "@/types/admin-dashboard/request/trip/push-to-contractors"
import type { ContractorPriceCalculationRequest } from "@/types/admin-dashboard/request/trip/contractor-price-calculation"
import { contractorPriceCalculationRequestAdapter } from "@/types/admin-dashboard/request/trip/contractor-price-calculation"
import { contractorPriceCalculationResponseAdapter } from "@/types/admin-dashboard/response/trip/contractor-price-calculation"
export const useTripStore = defineStore('trip', () => {
  const filters = ref<TripFilter>({
    companyId: '',
    contractId: '',
    contractorIds: [],
    currentDriverStatus: '',
    dispatchingMode: '',
    driverId: '',
    leadIds: [],
    lifecycleActor: '',
    search: '',
    sectorIds: [],
    financialDateFrom: '',
    financialDateTo: '',
    status: '',
    tripDocumentStatuses: [],
    vehicleTypeId: '',
  })

  const tripInvoiceFilters = ref<TripFilter>({
    destinationIds: [],
    financialDateFrom: '',
    financialDateTo: '',
  })

  const tripFinancialReportsFilters = ref<TripFilter>({
    companyId: '',
    contractorIds: [],
    destinationIds: [],
    sectorIds: [],
    contractId: '',
    driverId: '',
    leadIds: [],
    overNightCostBearer: '',
    search: '',
    financialDateFrom: '',
    financialDateTo: '',
    status: '',
    tripDocumentStatuses: [],
    vehicleTypeId: '',
  })

  const tripAuditingFilters = ref<TripFilter>({
    companyId: '',
    contractorIds: [],
    sectorIds: [],
    contractId: '',
    driverId: '',
    leadIds: [],
    search: '',
    financialDateFrom: '',
    financialDateTo: '',
    status: '',
    tripDocumentStatuses: [],
    vehicleTypeId: '',
  })

  const contractorTripFilters = ref<TripFilter>({
    companyId: '',
    search: '',
    financialDateFrom: '',
    financialDateTo: '',
  })

  const tripRequestsFilters = ref<TripFilter>({
    companyId: '',
    financialDateFrom: '',
    financialDateTo: '',
  })

  const contractorsRequestsFilters = ref<TripFilter>({
    companyId: '',
    contractorIds: [],
    search: '',
    vehicleTypeId: '',
  })

  const trip = ref<TripWizard>({
    actualCargoWeightTons: '',
    cargoPrice: '',
    companyId: '',
    contactPersonMobile: '',
    contactPersonName: '',
    contractDistanceKm: '',
    contractId: '',
    contractorId: '',
    contractorPrice: '',
    currentDriverStatus: '',
    customsStatus: TripCustomsStatus.NONE,
    contractConditions: null,
    destinationId: '',
    documents: [],
    dispatcherNotes: null,
    dispatchingMode: DispatchingMode.Loadly,
    driverId: '',
    endDate: '',
    financialDate: '',
    hasCustoms: false,
    isExternalTrip: false,
    isHistorical: false,
    leadId: '',
    recordings: [],
    returnDestinationId: '',
    spotPrice: '',
    startDate: '',
    submitRequest: false,
    type: '',
    vehicleId: null,
    tailVehicleId: null,
    vehicleTypeId: '',
    waypoints: [],
  })

  const selectedDriverVehicle = ref<Vehicle | null>(null)
  const tripsList = ref<TripList | null>(null)
  const selectedTripsIds = ref<string[]>([])
  const trackList = ref<TripTrackItem[]>([])
  const tripsInvoiceSummary = ref<TripInvoiceSummary | null>(null)

  function resetTrip(): void {
    trip.value = {
      actualCargoWeightTons: '',
      cargoPrice: '',
      companyId: '',
      contactPersonMobile: '',
      contactPersonName: '',
      contractDistanceKm: '',
      contractId: '',
      contractorId: '',
      contractorPrice: '',
      customsStatus: TripCustomsStatus.NONE,
      contractConditions: null,
      destinationId: '',
      documents: [],
      dispatcherNotes: null,
      dispatchingMode: DispatchingMode.Loadly,
      driverId: '',
      endDate: '',
      financialDate: '',
      hasCustoms: false,
      isExternalTrip: false,
      isHistorical: false,
      leadId: '',
      recordings: [],
      returnDestinationId: '',
      spotPrice: '',
      startDate: '',
      submitRequest: false,
      type: '',
      vehicleId: null,
      tailVehicleId: null,
      vehicleTypeId: '',
      waypoints: [],
    }
  }

  function resetFilters(): void {
    filters.value = {
      companyId: '',
      contractorIds: [],
      sectorIds: [],
      contractId: '',
      driverId: '',
    }
  }

  async function fetchTrips(data: TripListRequest) {
    const response = await api({
      method: 'GET',
      url: '/trips',
      params: tripListRequestAdapter(data),
      responseAdapter: tripListResponseAdapter,
    })

    if (response.success && response.data) {
      tripsList.value = response.data
    }

    return response
  }

  async function fetchTrip(id: number) {
    const response = await api({
      method: 'GET',
      url: `/trips/${id}`,
      responseAdapter: tripResponseAdapter,
    })

    trip.value = response.data

    return response
  }

  async function createTrip(vehicleId?: number | null) {
    return api({
      method: 'POST',
      url: '/trips',
      data: tripWizardRequestAdapter(trip.value, vehicleId),
    })
  }

  async function createHistoricalTrip(vehicleId?: number | null) {
    return api({
      method: 'POST',
      url: '/trips/historical',
      data: tripWizardRequestAdapter(trip.value, vehicleId),
    })
  }

  async function bulkCreateTrips(data: TripBulkCreateRequest) {
    return api({
      method: 'POST',
      url: '/trips/bulk-create',
      data: tripBulkCreateRequestAdapter(data),
    })
  }

  async function updateTrip(tripId: number, vehicleId?: number | null, reviewCompleted?: boolean) {
    return api({
      method: 'PUT',
      url: `/trips/${tripId}`,
      data: tripWizardRequestAdapter(trip.value, vehicleId, reviewCompleted),
    })
  }

  async function updateOvernightCostBearer(tripId: number, overNightCostBearer: number) {
    return api({
      method: 'PATCH',
      url: `/trips/${tripId}/overnight-cost-bearer`,
      data: { overNightCostBearer },
    })
  }

  async function addDispatcherNotes(tripId: number, dispatcherNotes: string) {
    return api({
      method: 'PATCH',
      url: `/trips/${tripId}/dispatcher-notes`,
      data: { dispatcherNotes },
    })
  }

  async function updateTripCharges(tripId: number, charges: TripWizardTripCharge[]) {
    return api({
      method: 'PUT',
      url: `/trips/${tripId}/charges`,
      data: {
        tripCharges: tripChargesToApiPayload(charges)
      },
    })
  }

  async function auditTrip(tripId: number, data: TripAuditRequest) {
    return api({
      method: 'POST',
      url: `/trips/${tripId}/audit`,
      data: tripAuditRequestAdapter(data),
    })
  }

  async function cancelTrip(id: number, cancellationReason: string) {
    return api({
      method: 'POST',
      url: `/trips/${id}/cancel`,
      data: { cancellationReason },
    })
  }

  async function completeTrip(id: number) {
    return api({
      method: 'POST',
      url: `/trips/${id}/complete`,
    })
  }

  async function deleteTrip(tripId: number) {
    return api({
      method: 'DELETE',
      url: `/trips/${tripId}`,
    })
  }

  async function exportExcel(data: TripListRequest) {
    return api({
      url: '/trips/excel',
      method: 'GET',
      responseType: 'blob',
      params: tripListRequestAdapter(data),
      headers: {
        Accept: '*/*',
      },
    })
  }

  async function fetchTrack() {
    const response = await api({
      method: 'GET',
      url: '/trips/track',
      params: tripListRequestAdapter(filters.value),
    })

    const payload = response.data as { data?: TripTrackItem[] } | TripTrackItem[] | undefined

    const list: TripTrackItem[] = Array.isArray(payload)
      ? payload
      : (payload as { data?: TripTrackItem[] })?.data ?? []

    if (response.success && list.length > 0) {
      trackList.value = list
    } else {
      trackList.value = []
    }

    return response
  }

  function getContractorPriceCalculation(data: ContractorPriceCalculationRequest) {
    return api({
      url: '/trips/contractor-price-preview',
      method: 'GET',
      params: contractorPriceCalculationRequestAdapter(data),
      responseAdapter: contractorPriceCalculationResponseAdapter,
    })
  }

  async function getTripsInvoiceSummary(tripIds: string[]) {
    return api({
      url: '/trips/charges/invoice-summary',
      method: 'POST',
      data: { tripIds },
      responseAdapter: tripInvoiceSummaryResponseAdapter,
    })
  }

  async function updateTripChargesInvoiceType(data: UpdateTripChargesInvoiceTypeRequest) {
    return api({
      url: '/trips/charges/invoice-type',
      method: 'PATCH',
      data: updateTripChargesInvoiceTypeRequestAdapter(data),
    })
  }

  async function pushToContractors(tripId: number | string, data: PushToContractorsRequest) {
    return api({
      method: 'POST',
      url: `/trips/${tripId}/push-to-contractors`,
      data: pushToContractorsRequestAdapter(data),
    })
  }

  async function approveBookingRequest(tripId: number | string, bookingRequestId: number | string) {
    return api({
      method: 'POST',
      url: `/trips/${tripId}/booking-requests/${bookingRequestId}/approve`,
    })
  }

  return {
    addDispatcherNotes,
    approveBookingRequest,
    auditTrip,
    bulkCreateTrips,
    cancelTrip,
    completeTrip,
    contractorTripFilters,
    contractorsRequestsFilters,
    createHistoricalTrip,
    createTrip,
    deleteTrip,
    exportExcel,
    fetchTrack,
    fetchTrip,
    fetchTrips,
    filters,
    getContractorPriceCalculation,
    getTripsInvoiceSummary,
    pushToContractors,
    resetFilters,
    resetTrip,
    selectedDriverVehicle,
    selectedTripsIds,
    trackList,
    trip,
    tripAuditingFilters,
    tripFinancialReportsFilters,
    tripInvoiceFilters,
    tripRequestsFilters,
    tripsInvoiceSummary,
    tripsList,
    updateOvernightCostBearer,
    updateTrip,
    updateTripCharges,
    updateTripChargesInvoiceType,
  }
})
