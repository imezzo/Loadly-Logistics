import { useCompanyStore } from '@/stores/admin-dashboard/company'
import { companyToFormDataAdapter } from '@/types/admin-dashboard/response/company-detail'
import type { ApiCompanyWithDetails } from '@/types/admin-dashboard/response/company-detail'
import { getLocalizedValue } from '@/utils/i18n'

export function useCompanyData() {
  const companyStore = useCompanyStore()
  const { company } = storeToRefs(companyStore)

  async function fetchCompany(companyId: string | number) {
    const { success, data, message, errors } = await companyStore.get(companyId)

    if (!success) return null

    const companyName = getLocalizedValue(
      data.companyDetail?.nameEn || '',
      data.companyDetail?.nameAr || ''
    )

    const companyStatus = data.status

    company.value = data

    const formData = companyToFormDataAdapter(data as ApiCompanyWithDetails)

    return {
      companyName,
      companyStatus,
      data,
      formData,
      message,
      errors
    }
  }

  return {
    fetchCompany,
    company,
  }
}
