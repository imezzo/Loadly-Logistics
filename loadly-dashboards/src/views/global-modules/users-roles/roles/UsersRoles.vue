<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <div
      class="
        mb-6 flex flex-col gap-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <h1 class="text-2xl font-bold text-neutral-800">
        {{ $t('title.allPermissionsRoles') }}
      </h1>

      <button
        type="button"
        class="btn-primary"
        @click="addNewRole"
      >
        {{ $t('button.addNewRole') }}
        <IconPlus class="size-5" />
      </button>
    </div>

    <BaseInputSearch
      :model-value="search"
      :placeholder="$t('form.searchHere')"
      wrapper-class="
        mb-6 flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
        w-full 2md:w-1/3
      "
      @update:model-value="search = $event || ''"
    />

    <div
      v-if="roles.length"
      class="
        grid grid-cols-1 gap-4 bg-neutral-100 px-3.75 py-6
        lg:grid-cols-2
      "
    >
      <RoleCard
        v-for="role in roles"
        :id="role.id"
        :key="role.id"
        :title="role.name"
        :description="role.description || ''"
        :show-delete="!role.isSuperAdmin"
        @edit="editRole(role.id)"
        @delete="deleteRole(role.id)"
      />
    </div>

    <div
      v-else
      class="flex items-center justify-center py-12 text-neutral-600"
    >
      <p>{{ $t('content.noRolesFound') }}</p>
    </div>

    <BasePagination
      v-if="roles.length"
      :pagination="pagination"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
    />

    <AddRolePopup
      v-model="isAddRolePopupOpen"
      @refresh="fetchRoles"
    />

    <EditRolePopup
      v-if="selectedRole"
      v-model="isEditRolePopupOpen"
      :role="selectedRole"
      @refresh="fetchRoles"
    />

    <DeleteRolePopup
      v-if="roleToDelete"
      v-model="isDeleteRolePopupOpen"
      :role-name="roleToDelete.name"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <DeleteRoleErrorPopup
      v-model="isDeleteErrorPopupOpen"
      :message="deleteErrorMessage"
    />
  </div>
</template>

<script setup lang="ts">
  import RoleCard from './components/RoleCard.vue'
  import AddRolePopup from './components/AddRolePopup.vue'
  import EditRolePopup from './components/EditRolePopup.vue'
  import DeleteRolePopup from './components/DeleteRolePopup.vue'
  import DeleteRoleErrorPopup from './components/DeleteRoleErrorPopup.vue'
  import { useRolesList } from './composables/useRolesList'
  import { useRoleStore } from '@/stores/role'
  import { useLoading } from '@/composables/useLoading'
  import type { Role } from '@/types/response/role'

  const { t } = useI18n()

  const breadcrumbItems = [
    { label: t('title.usersRoles'), to: '/' },
    { label: t('title.rolesList') },
  ]

  const {
    roles: apiRoles,
    fetchRoles,
    nextPage,
    pagination,
    prevPage,
    search,
    setPage,
    updateItemsPerPage,
  } = useRolesList()
  const roleStore = useRoleStore()
  const { isLoading: isDeleting, setLoading } = useLoading()

  const isAddRolePopupOpen = ref(false)
  const isEditRolePopupOpen = ref(false)
  const isDeleteRolePopupOpen = ref(false)
  const isDeleteErrorPopupOpen = ref(false)
  const selectedRole = ref<Role | null>(null)
  const roleToDelete = ref<Role | null>(null)
  const deleteErrorMessage = ref<string>('')

  const roles = computed(() => {
    return apiRoles.value.map(role => ({
      id: role.id,
      name: role.name,
      description: role.description,
      isSuperAdmin: role.nameEn.toLowerCase().includes('super'),
    }))
  })

  onMounted(() => {
    fetchRoles()
  })

  function addNewRole(): void {
    isAddRolePopupOpen.value = true
  }

  function editRole(roleId: number): void {
    const role = apiRoles.value.find(r => r.id === roleId)
    if (role) {
      selectedRole.value = role
      isEditRolePopupOpen.value = true
    }
  }

  function deleteRole(roleId: number): void {
    const role = apiRoles.value.find(r => r.id === roleId)
    if (role) {
      roleToDelete.value = role
      isDeleteRolePopupOpen.value = true
    }
  }

  async function confirmDelete(): Promise<void> {
    if (!roleToDelete.value) return

    setLoading(true)
    const { success, message } = await roleStore.deleteRole(roleToDelete.value.id)

    if (success) {
      isDeleteRolePopupOpen.value = false
      roleToDelete.value = null
      fetchRoles()
    } else {
      isDeleteRolePopupOpen.value = false
      deleteErrorMessage.value = message || t('error.networkError')
      isDeleteErrorPopupOpen.value = true
    }

    setLoading(false)
  }

  watch(isEditRolePopupOpen, (newValue) => {
    if (!newValue) {
      selectedRole.value = null
    }
  })

  watch(isDeleteRolePopupOpen, (newValue) => {
    if (!newValue) {
      roleToDelete.value = null
    }
  })

  onUnmounted(() => {
    search.value = ''
  })
</script>

<style scoped>

</style>
