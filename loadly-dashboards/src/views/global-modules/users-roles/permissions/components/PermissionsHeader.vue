<template>
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
      {{ roleName }} {{ $t('title.permissionsRole') }}
    </h1>

    <button
      type="button"
      class="btn-success"
      @click="$emit('save')"
    >
      {{ $t('button.saveChanges') }}
      <IconCheckCircle class="size-5" />
    </button>
  </div>
</template>

<script setup lang="ts">
  const { t } = useI18n()

  const props = defineProps<{
    roleName: string
  }>()

  defineEmits<{
    save: []
  }>()

  const breadcrumbItems = computed(() => [
    { label: t('title.usersRoles'), to: '/users-roles' },
    { label: t('title.rolesList'), to: '/users-roles' },
    { label: `${props.roleName} ${t('title.permissionsRole')}` },
  ])
</script>

<style scoped>

</style>

