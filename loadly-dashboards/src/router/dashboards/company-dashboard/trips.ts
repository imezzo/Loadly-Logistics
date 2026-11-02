import type { RouteRecordRaw } from 'vue-router'
import { Permission, Trip } from '@/types/response/permission-group'

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
        path: 'map',
        name: 'TripsOnMap',
        component: () => import('@/views/global-modules/trips/management/company/CompanyTrips.vue'),
        meta: {
          title: 'Trips on Map',
          requiresAuth: true,
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
    ]
  },
]
