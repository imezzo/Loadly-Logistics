import type { RouteRecordRaw } from 'vue-router'
import {
  Permission,
  VehicleFacility as VehicleFacilityPermission,
} from '@/types/response/permission-group'

export const vehicleFacilitiesRoutes: RouteRecordRaw[] = [
  {
    path: '/vehicle-facilities/locations',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: 'list',
        name: 'VehicleFacilitiesLocationsList',
        component: () => import(
          '@/views/dashboards/admin-dashboard/vehicle-facilities/locations/list/LocationsList.vue'
        ),
        meta: {
          title: 'Vehicle Locations',
          requiresAuth: true,
          module: Permission.VehicleFacility,
          permission: VehicleFacilityPermission.View,
        },
      },
      {
        path: 'create',
        name: 'CreateVehicleFacility',
        component: () => import(
          '@/views/dashboards/admin-dashboard/vehicle-facilities/locations/wizard/VehicleFacilityWizard.vue'
        ),
        meta: {
          title: 'Add Vehicles locations',
          requiresAuth: true,
          module: Permission.VehicleFacility,
          permission: VehicleFacilityPermission.Create,
        },
      },
      {
        path: 'edit/:id',
        name: 'EditVehicleFacility',
        component: () => import(
          '@/views/dashboards/admin-dashboard/vehicle-facilities/locations/wizard/VehicleFacilityWizard.vue'
        ),
        meta: {
          title: 'Edit Vehicles locations',
          requiresAuth: true,
          module: Permission.VehicleFacility,
          permission: VehicleFacilityPermission.Edit,
        },
      },
      {
        path: 'view/:id',
        name: 'ViewVehicleFacility',
        component: () => import(
          '@/views/dashboards/admin-dashboard/vehicle-facilities/locations/wizard/VehicleFacilityWizard.vue'
        ),
        meta: {
          title: 'View Vehicles locations',
          requiresAuth: true,
          module: Permission.VehicleFacility,
          permission: VehicleFacilityPermission.View,
        },
      },
    ],
  },
]
