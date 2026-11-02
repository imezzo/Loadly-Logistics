import type { Pagination } from '@/types/response/pagination'
import type { LookupResponse } from '@/types/response/lookup'
import { formatDate } from '@/utils/date'
import { getLocalizedValue } from '@/utils/i18n'

export interface CompanyContactPerson {
  id: string
  fullName: string
  userName: string
  email: string
  phone: string
  emailVerified: boolean
  phoneVerified: boolean
  status: number
  type: number
}

export interface Company {
  id: string
  name: string
  registrationNumber: string
  taxId: string
  website: string
  industryTypeId: number | null
  companySize: string | null
  serviceRequirementsIds: number[] | null
  businessHourId: number | null
  communicationsMethodId: number | null
  verificationStatus: number
  qrCode: string | null
  createdAt: string
  contactPerson: CompanyContactPerson
}

export interface CompaniesListResponse {
  companies: Company[]
  pagination: Pagination
}

export interface ApiCompanyDetail {
  id: number
  companyId: number
  nameEn: string
  nameAr: string
  registrationNumber: string
  taxId: string
  websiteUrl: string
  industryTypeId: number | null
  companySize: string | null
  serviceRequirementsIds: number[] | null
  businessHourId: number | null
  communicationsMethodId: number | null
  verificationStatus: number
  qrCode: string | null
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiCompany {
  id: number
  fullName: string
  userName: string
  email: string
  phone: string
  mfaChannel: number
  emailVerified: boolean
  phoneVerified: boolean
  status: number
  type: number
  companyId: number | null
  createdAt: string
  updatedAt: string
  companyDetail: ApiCompanyDetail
}

export interface ApiCompaniesListResponse {
  data: ApiCompany[]
  pagination: Pagination
}

export function contactPersonAdapter(company: ApiCompany): CompanyContactPerson {
  return {
    id: company.id.toString(),
    fullName: company.fullName,
    userName: company.userName,
    email: company.email,
    phone: company.phone,
    emailVerified: company.emailVerified,
    phoneVerified: company.phoneVerified,
    status: company.status,
    type: company.type,
  }
}

export function companyAdapter(company: ApiCompany): Company {
  const { companyDetail } = company

  if (!companyDetail) return {} as Company

  return {
    id: companyDetail?.companyId.toString(),
    name: getLocalizedValue(companyDetail.nameEn, companyDetail.nameAr),
    registrationNumber: companyDetail.registrationNumber,
    taxId: companyDetail.taxId,
    website: companyDetail.websiteUrl,
    industryTypeId: companyDetail.industryTypeId,
    companySize: companyDetail.companySize,
    serviceRequirementsIds: companyDetail.serviceRequirementsIds,
    businessHourId: companyDetail.businessHourId,
    communicationsMethodId: companyDetail.communicationsMethodId,
    verificationStatus: companyDetail.verificationStatus,
    qrCode: companyDetail.qrCode,
    createdAt: formatDate(companyDetail.createdAt),
    contactPerson: contactPersonAdapter(company),
  }
}

export function companiesListResponseAdapter(response: ApiCompaniesListResponse): CompaniesListResponse {
  return {
    companies: response.data.map(companyAdapter),
    pagination: response.pagination,
  }
}

export function companiesListAsLookupResponseAdapter(response: ApiCompaniesListResponse): LookupResponse {
  return {
    items: response.data.map(company => ({
      label: company.companyDetail
        ? getLocalizedValue(company.companyDetail.nameEn, company.companyDetail.nameAr)
        : company.fullName,
      value: company.companyDetail?.companyId.toString() ?? company.id.toString(),
    })),
    pagination: response.pagination,
  }
}
