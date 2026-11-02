<template>
  <BaseTable
    :columns="tableColumns"
    :data="users"
  >
    <template
      v-if="hasActivationActions"
      #header-checkbox
    >
      <BaseCheckbox
        :model-value="isAllSelected"
        :indeterminate="isIndeterminate"
        @update:model-value="toggleSelectAll"
      />
    </template>

    <template
      v-if="hasActivationActions"
      #cell-checkbox="{ row }"
    >
      <BaseCheckbox
        :model-value="isSelected(row.id)"
        @update:model-value="toggleSelectUser(row.id)"
      />
    </template>

    <template #cell-status="{ row }">
      <BaseChip
        :label="getDisplayName('UserStatus', row.status)"
        :variant="userStatusVariants[Number(row.status)]"
      />
    </template>

    <template #cell-lastLogin="{ row }">
      <span class="text-sm text-neutral-800">
        {{ row.lastLogin || $t('content.never') }}
      </span>
    </template>

    <template
      v-if="hasEditPermission || hasViewPermission"
      #cell-action="{ row }"
    >
      <div class="flex items-center gap-2">
        <BaseLink
          v-if="hasEditPermission"
          :to="{ name: editRouteName ?? 'EditUser', params: { id: row.id } }"
          class="btn-action-success"
        >
          <IconEdit />
        </BaseLink>

        <BaseLink
          v-if="hasViewPermission"
          :to="{ name: viewRouteName ?? 'ViewUser', params: { id: row.id } }"
          class="btn-info-outline"
        >
          <IconEye visible />
        </BaseLink>
      </div>
    </template>
  </BaseTable>
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'
  import type { User } from '@/types/response/user'
  import type { Pagination } from '@/types/response/pagination'
  import { userStatusVariants } from '@/utils/statusChipVariants'

  const props = defineProps<{
    editRouteName?: string
    hasActivationActions?: boolean
    hasEditPermission?: boolean
    hasViewPermission?: boolean
    pagination: Pagination
    users: User[]
    viewRouteName?: string
  }>()

  const emit = defineEmits<{
    select: [selectedIds: Set<string>]
  }>()

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const Route = useRoute()

  const isUsersList = computed(() => Route.name === 'UsersList')

  const selectedIds = ref<Set<string>>(new Set())
  const tableColumns = ref<TableColumn[]>([
    ...(props.hasActivationActions ? [{ key: 'checkbox', class: 'w-12 px-6 py-4' }] : []),
    { key: 'fullName', label: t('table.fullName'), cellClass: 'font-medium text-neutral-20' },
    ...(!isUsersList.value ? [{ key: 'email', label: t('table.email') }] : []),
    ...(isUsersList.value ? [{ key: 'phone', label: t('table.phoneNumber') }] : []),
    { key: 'status', label: t('table.status') },
    { key: 'lastLogin', label: t('table.lastLogin') },
    ...(props.hasEditPermission || props.hasViewPermission
      ? [{ key: 'action', label: t('table.action') }]
      : []),
  ])

  const isAllSelected = computed(
    () => props.users.length > 0 && selectedIds.value.size === props.users.length,
  )

  const isIndeterminate = computed(
    () => selectedIds.value.size > 0 && selectedIds.value.size < props.users.length,
  )

  function isSelected(id: string): boolean {
    return selectedIds.value.has(id)
  }

  function toggleSelectUser(id: string): void {
    if (isSelected(id)) {
      selectedIds.value.delete(id)
    } else {
      selectedIds.value.add(id)
    }
    emit('select', selectedIds.value)
  }

  function toggleSelectAll(): void {
    if (isAllSelected.value) {
      selectedIds.value.clear()
    } else {
      props.users.forEach(user => selectedIds.value.add(user.id))
    }
    emit('select', selectedIds.value)
  }

  watch(() => props.pagination, () => {
    selectedIds.value.clear()
    emit('select', selectedIds.value)
  })
</script>

<style scoped>

</style>
