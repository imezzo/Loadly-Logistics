import { Permission, Vehicle } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const fleetRoutes: RouteRecordRaw[] = [
  {
    path: '/fleet',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'FleetManagement',
        component: () => import('@/views/dashboards/admin-dashboard/fleet/list/FleetManagement.vue'),
        meta: {
          title: 'Fleet Management',
          requiresAuth: true,
          module: Permission.Vehicle,
          permission: Vehicle.View,
        }
      },
      {
        path: 'register',
        name: 'RegisterVehicle',
        component: () => import('@/views/dashboards/admin-dashboard/fleet/register/VehicleWizard.vue'),
        meta: {
          title: 'Register New Vehicle',
          requiresAuth: true,
          module: Permission.Vehicle,
          permission: Vehicle.Create,
        }
      },
      {
        path: ':id/view',
        name: 'ViewVehicle',
        component: () => import('@/views/dashboards/admin-dashboard/fleet/register/VehicleWizard.vue'),
        meta: {
          title: 'View Vehicle',
          requiresAuth: true,
          module: Permission.Vehicle,
          permission: Vehicle.View,
        }
      },
      {
        path: ':id/edit',
        name: 'UpdateVehicle',
        component: () => import('@/views/dashboards/admin-dashboard/fleet/register/VehicleWizard.vue'),
        meta: {
          title: 'Update Vehicle',
          requiresAuth: true,
          module: Permission.Vehicle,
          permission: Vehicle.Edit,
        }
      },
    ]
  },
]
