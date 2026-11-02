import type { RouteRecordRaw } from 'vue-router'
import {
  Address,
  Color as ColorPermission,
  Discount as DiscountPermission,
  Document as DocumentPermission,
  IndustryType as IndustryTypePermission,
  Lead as LeadPermission,
  Permission,
  Sector as SectorPermission,
  Tax as TaxPermission,
  VehicleBrand as VehicleBrandPermission,
  VehicleBrandModel as VehicleBrandModelPermission,
  VehicleCapacityUnit as VehicleCapacityUnitPermission,
  VehicleInsurance as VehicleInsurancePermission,
  VehicleType as VehicleTypePermission,
} from '@/types/response/permission-group'

export const settingsRoutes: RouteRecordRaw[] = [
  {
    path: '/settings/governments',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsGovernments',
        component: () => import('@/views/dashboards/admin-dashboard/settings/addresses/governorates/GovernoratesList.vue'),
        meta: {
          title: 'Governorates',
          requiresAuth: true,
          module: Permission.Address,
          permission: Address.ViewGovernerate,
        }
      },
    ]
  },
  {
    path: '/settings/cities',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsCities',
        component: () => import('@/views/dashboards/admin-dashboard/settings/addresses/cities/CitiesList.vue'),
        meta: {
          title: 'Cities',
          requiresAuth: true,
          module: Permission.Address,
          permission: Address.ViewCity,
        }
      },
    ]
  },
  {
    path: '/settings/districts',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsDistricts',
        component: () => import('@/views/dashboards/admin-dashboard/settings/addresses/districts/DistrictsList.vue'),
        meta: {
          title: 'Districts',
          requiresAuth: true,
          module: Permission.Address,
          permission: Address.ViewDistrict,
        }
      },
    ]
  },
  {
    path: '/settings/documents',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsDocuments',
        component: () => import('@/views/dashboards/admin-dashboard/settings/documents/DocumentsList.vue'),
        meta: {
          title: 'Documents',
          requiresAuth: true,
          module: Permission.Document,
          permission: DocumentPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/industry-types',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsIndustryTypes',
        component: () => import('@/views/dashboards/admin-dashboard/settings/industry-types/IndustryTypesList.vue'),
        meta: {
          title: 'Industry Types',
          requiresAuth: true,
          module: Permission.IndustryType,
          permission: IndustryTypePermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/sectors',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsSectors',
        component: () => import('@/views/dashboards/admin-dashboard/settings/sectors/SectorsList.vue'),
        meta: {
          title: 'Sectors',
          requiresAuth: true,
          module: Permission.Sector,
          permission: SectorPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/taxes',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsTaxes',
        component: () => import('@/views/dashboards/admin-dashboard/settings/taxes/TaxesList.vue'),
        meta: {
          title: 'Taxes',
          requiresAuth: true,
          module: Permission.Tax,
          permission: TaxPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/discounts',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsDiscounts',
        component: () => import('@/views/dashboards/admin-dashboard/settings/discounts/DiscountsList.vue'),
        meta: {
          title: 'Discounts',
          requiresAuth: true,
          module: Permission.Discount,
          permission: DiscountPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/leads',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsLeads',
        component: () => import('@/views/dashboards/admin-dashboard/settings/leads/LeadsList.vue'),
        meta: {
          title: 'Leads',
          requiresAuth: true,
          module: Permission.Lead,
          permission: LeadPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/colors',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsColors',
        component: () => import('@/views/dashboards/admin-dashboard/settings/vehicles/colors/ColorsList.vue'),
        meta: {
          title: 'Colors',
          requiresAuth: true,
          module: Permission.Color,
          permission: ColorPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/vehicle-types',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsVehicleTypes',
        component: () => import('@/views/dashboards/admin-dashboard/settings/vehicles/vehicle-types/VehicleTypesList.vue'),
        meta: {
          title: 'Vehicle Types',
          requiresAuth: true,
          module: Permission.VehicleType,
          permission: VehicleTypePermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/vehicle-capacity-units',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsVehicleCapacityUnits',
        component: () => import('@/views/dashboards/admin-dashboard/settings/vehicles/vehicle-capacity-units/VehicleCapacityUnitsList.vue'),
        meta: {
          title: 'Vehicle Capacity Units',
          requiresAuth: true,
          module: Permission.VehicleCapacityUnit,
          permission: VehicleCapacityUnitPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/vehicle-brands',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsVehicleBrands',
        component: () => import('@/views/dashboards/admin-dashboard/settings/vehicles/vehicle-brands/VehicleBrandsList.vue'),
        meta: {
          title: 'Vehicle Brands',
          requiresAuth: true,
          module: Permission.VehicleBrand,
          permission: VehicleBrandPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/vehicle-brand-models',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsVehicleBrandModels',
        component: () => import('@/views/dashboards/admin-dashboard/settings/vehicles/vehicle-brand-models/VehicleBrandModelsList.vue'),
        meta: {
          title: 'Vehicle Brand Models',
          requiresAuth: true,
          module: Permission.VehicleBrandModel,
          permission: VehicleBrandModelPermission.View,
        }
      },
    ]
  },
  {
    path: '/settings/vehicle-insurances',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'SettingsVehicleInsurances',
        component: () => import('@/views/dashboards/admin-dashboard/settings/vehicles/vehicle-insurances/VehicleInsurancesList.vue'),
        meta: {
          title: 'Vehicle Insurances',
          requiresAuth: true,
          module: Permission.VehicleInsurance,
          permission: VehicleInsurancePermission.View,
        }
      },
    ]
  },
  {
    path: '/vehicle-tags',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'VehicleTagsList',
        component: () => import('@/views/dashboards/admin-dashboard/settings/vehicle-tags/VehicleTagsList.vue'),
        meta: {
          title: 'Vehicle tags',
          requiresAuth: true,
        }
      },
    ]
  },
]

