import type { UserCreateRequest, UsersListRequest, UserUpdateRequest } from "@/types/request/user"
import { userCreateRequestAdapter, usersListRequestAdapter } from "@/types/request/user"
import { userAdapter, usersListResponseAdapter } from "@/types/response/user"
import type { UsersListResponse } from "@/types/response/user"
import type { User, UsersFilter } from "@/types/user"
import type { LookupResponse } from "@/types/response/lookup"
import { api } from "@/utils/api"

export const useUserStore = defineStore('user', () => {
  const resourceUrl = ref<string>('/admins')
  const usersList = ref<UsersListResponse | null>(null)
  const usersListAsLookup = ref<LookupResponse | null>(null)
  const user = ref<User | null>(null)
  const filters = ref<UsersFilter>({
    roleIds: [],
    search: '',
    status: '',
  })

  async function getList(data: UsersListRequest) {
    return api({
      url: resourceUrl.value,
      method: 'GET',
      params: usersListRequestAdapter(data),
      responseAdapter: usersListResponseAdapter,
    })
  }

  async function get(id: string) {
    return api({
      url: resourceUrl.value + '/' + id,
      method: 'GET',
      responseAdapter: userAdapter,
    })
  }

  async function create(data: UserCreateRequest) {
    return api({
      url: resourceUrl.value,
      method: 'POST',
      data: userCreateRequestAdapter(data),
    })
  }

  async function update(data: UserUpdateRequest) {
    const id = data.id
    delete data.id

    return api({
      url: resourceUrl.value + '/' + id,
      method: 'PUT',
      data: userCreateRequestAdapter(data),
    })
  }

  async function activate(ids: string[]) {
    return api({
      url: resourceUrl.value + '/activate',
      method: 'POST',
      data: { adminIds: ids },
    })
  }

  async function deleteUser(id: string) {
    return api({
      url: '/users/' + id,
      method: 'DELETE',
    })
  }

  async function deactivate(ids: string[]) {
    return api({
      url: resourceUrl.value + '/deactivate',
      method: 'POST',
      data: { adminIds: ids },
    })
  }

  function updateFilters(newFilters: Partial<UsersFilter>) {
    filters.value = { ...filters.value, ...newFilters }
  }

  function resetFilters() {
    updateFilters({ roleIds: [], search: '', status: '' })
  }

  return {
    activate,
    create,
    deactivate,
    deleteUser,
    filters,
    get,
    getList,
    resetFilters,
    resourceUrl,
    update,
    updateFilters,
    user,
    usersList,
    usersListAsLookup,
  }
})
