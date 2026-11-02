import { Permission, Trip, Vehicle } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const tripsRoutes: RouteRecordRaw[] = [
  {
    path: '/trips',
    component: () => import('@/views/global-modules/trips/management/TripsLayout.vue'),
    children: [
      {
        path: '',
        name: 'AllTrips',
        component: () => import('@/views/global-modules/trips/management/list/TripsList.vue'),
        meta: {
          title: 'All Trips',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.View,
        },
      },
      {
        path: 'external',
        name: 'ExternalTrips',
        component: () => import('@/views/global-modules/trips/management/list/TripsList.vue'),
        meta: {
          title: 'External Trips',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.View,
        },
      },
      {
        path: 'map',
        name: 'TripsOnMap',
        component: () => import('@/views/global-modules/trips/management/company/CompanyTrips.vue'),
        meta: {
          title: 'Trips on Map',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.View,
        },
      },
    ],
  },
  {
    path: '/trips/requests',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'TripRequests',
        component: () => import('@/views/global-modules/trips/requests/TripRequests.vue'),
        meta: {
          title: 'Trip Requests',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.View,
        },
      },
    ],
  },
  {
    path: '/trips/contractors-requests',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'ContractorsRequests',
        component: () => import('@/views/global-modules/trips/contractors-requests/ContractorsRequests.vue'),
        meta: {
          title: 'Contractors Requests',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.View,
        },
      },
    ],
  },
  {
    path: '/trip',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: 'create',
        name: 'CreateTrip',
        component: () => import('@/views/global-modules/trips/wizard/TripWizard.vue'),
        meta: {
          title: 'Create Trip',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.Create,
        },
      },
      {
        path: 'create/external',
        name: 'CreateExternalTrip',
        component: () => import('@/views/global-modules/trips/wizard/TripWizard.vue'),
        meta: {
          title: 'Create External Trip',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.Create,
        },
      },
      {
        path: 'edit/:id',
        name: 'EditTrip',
        component: () => import('@/views/global-modules/trips/wizard/TripWizard.vue'),
        meta: {
          title: 'Edit Trip',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.Edit,
        },
      },
      {
        path: 'view/:id',
        name: 'ViewTrip',
        component: () => import('@/views/global-modules/trips/trip-details/TripDetails.vue'),
        meta: {
          title: 'View Trip',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.View,
        },
      },
      {
        path: 'vehicle-status',
        name: 'TruckStatus',
        component: () => import('@/views/global-modules/trips/vehicle-status/VehicleStatus.vue'),
        meta: {
          title: 'Vehicles manage',
          requiresAuth: true,
          module: Permission.Vehicle,
          permission: Vehicle.View,
        }
      },
    ]
  },
]
