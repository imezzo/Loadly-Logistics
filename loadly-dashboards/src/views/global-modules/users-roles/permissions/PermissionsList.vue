<template>
  <div class="page-content">
    <PermissionsHeader
      :role-name="roleName"
      @save="savePermissions"
    />

    <PermissionsGrid :permissions="permissions" />

    <PopupError
      v-model="isErrorPopupOpen"
      :message="errorMessage"
    />

    <PopupSuccess
      v-model="isPermissionsSaved"
      :message="$t('success.permissionsUpdated')"
    >
      <template #actions>
        <button
          type="button"
          class="btn-primary mt-3 w-full"
          @click="closeSuccessPopup"
        >
          {{ $t('button.done') }}
        </button>
      </template>
    </PopupSuccess>
  </div>
</template>

<script setup lang="ts">
  import PermissionsHeader from './components/PermissionsHeader.vue'
  import PermissionsGrid from './components/PermissionsGrid.vue'
  import { usePermissionsList } from './composables/usePermissionsList'

  const route = useRoute()
  const router = useRouter()

  const roleId = computed(() => Number(route.params.id))
  const {
    roleName,
    isErrorPopupOpen,
    isPermissionsSaved,
    errorMessage,
    permissions,
    fetchRoleDetails,
    savePermissions,
  } = usePermissionsList(roleId)

  onMounted(() => {
    fetchRoleDetails()
  })

  function closeSuccessPopup(): void {
    isPermissionsSaved.value = false
    router.push({ name: 'UsersRoles' })
  }
</script>

<style scoped>

</style>
