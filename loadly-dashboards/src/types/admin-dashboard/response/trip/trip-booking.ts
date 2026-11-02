export interface TripBookingRequest {
  contractorId: number
  contractorName: string
  contractorPhone?: string | null
  createdAt: string
  driverId: number
  driverName: string
  driverPhone: string
  id: number
  note: string | null
  reviewedAt: string | null
  reviewedByAdminId: number | null
  status: number
  statusName: string
  totalTrips?: number | null
  tripId: number
}

export interface ApiTripBookingRequest {
  contractorId: number
  contractorName: string
  contractorPhone?: string | null
  createdAt: string
  driverId: number
  driverName: string
  driverPhone: string
  id: number
  note: string | null
  reviewedAt: string | null
  reviewedByAdminId: number | null
  status: number
  statusName: string
  totalTrips?: number | null
  tripId: number
}

export function tripBookingRequestAdapter(request: ApiTripBookingRequest): TripBookingRequest {
  return {
    contractorId: request.contractorId,
    contractorName: request.contractorName,
    contractorPhone: request.contractorPhone ?? null,
    createdAt: request.createdAt,
    driverId: request.driverId,
    driverName: request.driverName,
    driverPhone: request.driverPhone,
    id: request.id,
    note: request.note,
    reviewedAt: request.reviewedAt,
    reviewedByAdminId: request.reviewedByAdminId,
    status: request.status,
    statusName: request.statusName,
    totalTrips: request.totalTrips ?? null,
    tripId: request.tripId,
  }
}
