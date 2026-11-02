import { api } from '@/utils/api'
import type { LeadWizardRequest, LeadsListRequest } from '@/types/admin-dashboard/request/lead-settings'
import { leadsListRequestAdapter } from '@/types/admin-dashboard/request/lead-settings'
import type { ApiLead, LeadsListResponse } from '@/types/admin-dashboard/response/lead/lead'
import { leadAdapter, leadsListResponseAdapter } from '@/types/admin-dashboard/response/lead/lead'

export const useLeadSettingsStore = defineStore('lead-settings', () => {
  const leadsList = ref<LeadsListResponse | null>(null)

  async function getList(data: LeadsListRequest) {
    return api({
      url: '/leads',
      method: 'GET',
      params: leadsListRequestAdapter(data),
      responseAdapter: leadsListResponseAdapter,
    })
  }

  async function getById(id: number) {
    return api({
      url: `/leads/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiLead) => leadAdapter(data),
    })
  }

  async function getByPhone(phone: string) {
    return api({
      url: '/leads/by-phone',
      method: 'GET',
      params: { phone },
      responseAdapter: (data: ApiLead) => leadAdapter(data),
    })
  }

  async function create(data: LeadWizardRequest) {
    return api({
      url: '/leads',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: LeadWizardRequest) {
    return api({
      url: `/leads/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteLead(id: number) {
    return api({
      url: `/leads/${id}`,
      method: 'DELETE',
    })
  }

  return {
    create,
    deleteLead,
    getById,
    getByPhone,
    getList,
    leadsList,
    update,
  }
})
