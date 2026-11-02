<template>
  <div
    class="rounded-2xl border border-neutral-500 bg-white p-4 shadow-sm"
  >
    <h2 class="mb-4 text-lg font-semibold text-neutral-800 capitalize">
      {{ module.name }} {{ $t('title.module') }}
    </h2>

    <BaseCheckbox
      v-model="isAllPermissionsChecked"
      :label="$t('form.selectAll')"
      @change="handleAllPermissionsChange"
    />

    <hr class="my-4 border-neutral-500">

    <div class="flex flex-col gap-3">
      <BaseCheckbox
        v-for="permission in module.permissions"
        :key="permission.id"
        v-model="permission.checked"
        :label="permission.name"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { PermissionModule } from '@/types/response/permission'

  const props = defineProps<{
    module: PermissionModule
  }>()

  const isAllPermissionsChecked = ref(false)

  function handleAllPermissionsChange(value: Event): void {
    props.module.permissions.forEach(permission => {
      permission.checked = (value.target as HTMLInputElement).checked
    })
  }

  watch(props.module.permissions, (newPermissions) => {
    isAllPermissionsChecked.value = newPermissions.every(permission => permission.checked)
  }, { immediate: true })
</script>

<style scoped>

</style>

