import type { DriversListRequest } from '@/types/admin-dashboard/request/driver/driver'
import { driversListRequestAdapter } from '@/types/admin-dashboard/request/driver/driver'
import type { DriverWizardRequest } from '@/types/admin-dashboard/request/driver/driver-wizard'
import { driverWizardRequestAdapter } from '@/types/admin-dashboard/request/driver/driver-wizard'
import { driversListResponseAdapter } from '@/types/admin-dashboard/response/driver/driver'
import type { DriversListResponse } from '@/types/admin-dashboard/response/driver/driver'
import type { DriversStatisticsResponse } from '@/types/admin-dashboard/response/driver/driver-statistics'
import { driverDetailAdapter } from '@/types/admin-dashboard/response/driver/driver-detail'
import type { DriverDetail } from '@/types/admin-dashboard/response/driver/driver-detail'
import type { DriversFilter } from '@/types/admin-dashboard/driver'
import { api } from '@/utils/api'

export const useDriverStore = defineStore('driver', () => {
  const driversList = ref<DriversListResponse | null>(null)
  const driversStatistics = ref<DriversStatisticsResponse | null>(null)
  const driver = ref<DriverDetail | null>(null)
  const filters = ref<DriversFilter>({
    contractorIds: [],
    search: '',
    workingStatuses: '',
    licenseCategory: '',
    rate: '',
  })

  async function getList(data: DriversListRequest) {
    return api({
      url: '/drivers',
      method: 'GET',
      params: driversListRequestAdapter(data),
      responseAdapter: driversListResponseAdapter,
    })
  }

  async function get(id: string) {
    return api({
      url: `/drivers/${id}`,
      method: 'GET',
      responseAdapter: driverDetailAdapter,
    })
  }

  async function getStatistics() {
    return api({
      url: '/drivers/statistics',
      method: 'GET',
    })
  }

  async function create(data: DriverWizardRequest) {
    return api({
      url: '/drivers',
      method: 'POST',
      data: driverWizardRequestAdapter(data),
    })
  }

  async function update(data: DriverWizardRequest) {
    return api({
      url: `/drivers/${data.id}`,
      method: 'PUT',
      data: driverWizardRequestAdapter(data),
    })
  }

  async function exportExcel(filters: DriversListRequest) {
    return api({
      url: '/drivers/excel',
      method: 'GET',
      responseType: 'blob',
      params: driversListRequestAdapter(filters),
      headers: {
        Accept: '*/*',
      },
    })
  }

  async function importDrivers(file: File) {
    const formData = new FormData()
    formData.append('ExcelFile', file)

    return api({
      url: '/drivers/import',
      method: 'POST',
      data: formData,
    })
  }

  function updateFilters(newFilters: Partial<DriversFilter>) {
    filters.value = { ...filters.value, ...newFilters }
  }

  function resetFilters() {
    updateFilters({
      contractorIds: [],
      search: '',
      workingStatuses: '',
      licenseCategory: '',
      rate: '',
    })
  }

  return {
    create,
    driver,
    driversList,
    driversStatistics,
    exportExcel,
    filters,
    get,
    getList,
    getStatistics,
    importDrivers,
    resetFilters,
    update,
    updateFilters,
  }
})
