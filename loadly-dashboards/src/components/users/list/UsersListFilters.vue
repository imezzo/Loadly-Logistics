<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
      lg:grid-cols-4
    "
  >
    <BaseCombobox
      :model-value="filters.search"
      :items="users"
      :placeholder="$t('form.searchNameOrEmail')"
      :filter-keys="['fullName', 'email']"
      item-label="fullName"
      @update:model-value="updateFilters({ search: $event })"
    >
      <template #item="{ item }">
        <IconUser class="size-5 shrink-0 text-neutral-800/60" />

        <div class="flex flex-col gap-1">
          <span class="flex-1 shrink-0">{{ item.fullName }}</span>

          <span class="text-xs text-neutral-800/60">{{ item.roles.map(role => role.name).join(', ') }}</span>
        </div>
      </template>
    </BaseCombobox>

    <BaseSelect
      :model-value="filters.status"
      :options="statusOptions"
      @update:model-value="changeStatus"
    />

    <BaseMultiSelect
      v-if="showRoleIds"
      :model-value="filters.roleIds"
      :options="roles"
      :placeholder="$t('form.role')"
      searchable
      :on-search="(search) => getRoles(1, search, 'admin')"
      :on-scroll-to-end="hasMoreRoles ? loadMoreRoles : undefined"
      @update:model-value="updateFilters({ roleIds: $event ?? [] })"
    />

    <button
      v-if="hasActiveFilters"
      type="button"
      class="btn-link me-auto"
      @click="resetFilters"
    >
      {{ $t('button.clearFilters') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import type { UsersFilter } from '@/types/user'
  import type { SelectOption } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { useUserStore } from '@/stores/admin-dashboard/user'
  import type { User } from '@/types/response/user'

  const props = defineProps<{
    showRoleIds?: boolean
  }>()

  const { t } = useI18n()
  const { userStatus } = useEnum()
  const { getRoles, hasMoreRoles, loadMoreRoles, roles } = useLookups()

  const userStore = useUserStore()
  const { filters, usersList } = storeToRefs(userStore)

  const users = computed<User[]>(() => usersList.value?.users || [])

  const hasActiveFilters = computed<boolean>(() => Object.values(filters.value).some((value) => (
    Array.isArray(value) ? value.length > 0 : value !== ''
  )))

  const statusOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.allStatus') },
    ...(userStatus.value ?? []),
  ])

  function updateFilters(data: Partial<UsersFilter>): void {
    userStore.updateFilters(data)
  }

  function changeStatus(value: string): void {
    if (value === filters.value.status) return
    updateFilters({ status: value as UsersFilter['status'] })
  }

  function resetFilters(): void {
    userStore.resetFilters()
  }

  onMounted(() => {
    if (props.showRoleIds) {
      getRoles(1, undefined, 'admin')
    }
  })
</script>

<style scoped>

</style>
