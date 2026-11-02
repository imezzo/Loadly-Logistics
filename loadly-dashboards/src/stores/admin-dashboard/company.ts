import type { CompaniesListRequest, CompanyCreateRequest } from "@/types/admin-dashboard/request/company"
import type { CompanyUpdateRequest } from "@/types/admin-dashboard/request/company-update"
import { companiesListRequestAdapter, companyCreateRequestAdapter } from "@/types/admin-dashboard/request/company"
import { companyUpdateRequestAdapter } from "@/types/admin-dashboard/request/company-update"
import { companiesListResponseAdapter, companiesListAsLookupResponseAdapter } from "@/types/admin-dashboard/response/company"
import type { CompaniesListResponse } from "@/types/admin-dashboard/response/company"
import type { ApiCompany } from "@/types/admin-dashboard/response/company-detail"
import { api } from "@/utils/api"
import type { LookupResponse } from "@/types/response/lookup"

export const useCompanyStore = defineStore('company', () => {
  const companiesList = ref<CompaniesListResponse | null>(null)
  const company = ref<ApiCompany | null>(null)
  const companiesListAsLookup = ref<LookupResponse | null>(null)

  async function getList(data: CompaniesListRequest) {
    return api({
      url: '/companies',
      method: 'GET',
      params: companiesListRequestAdapter(data),
      responseAdapter: companiesListResponseAdapter,
    })
  }

  async function getListAsLookup(data: CompaniesListRequest) {
    return api({
      url: '/companies',
      method: 'GET',
      params: companiesListRequestAdapter(data),
      responseAdapter: companiesListAsLookupResponseAdapter,
    })
  }

  async function get(id: string | number) {
    return api({
      url: `/companies/${id}`,
      method: 'GET',
    })
  }

  async function create(data: CompanyCreateRequest) {
    return api({
      url: '/companies',
      method: 'POST',
      data: companyCreateRequestAdapter(data),
    })
  }

  async function update(id: string | number, data: CompanyUpdateRequest) {
    return api({
      url: `/companies/${id}`,
      method: 'PUT',
      data: companyUpdateRequestAdapter(data),
    })
  }

  async function deleteCompany(id: string) {
    return api({
      url: `/companies/${id}`,
      method: 'DELETE',
    })
  }

  async function importCompanies(file: File) {
    const formData = new FormData()
    formData.append('ExcelFile', file)

    return api({
      url: '/companies/import',
      method: 'POST',
      data: formData,
    })
  }

  return {
    companiesList,
    companiesListAsLookup,
    company,
    create,
    deleteCompany,
    get,
    getList,
    getListAsLookup,
    importCompanies,
    update,
  }
})
