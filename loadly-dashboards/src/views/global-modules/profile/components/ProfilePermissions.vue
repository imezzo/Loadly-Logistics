<template>
  <section
    v-if="permissionGroups.length"
    class="mt-8 border-t border-neutral-200 pt-8"
  >
    <h2 class="mb-4 text-lg font-semibold text-neutral-800">
      {{ $t('title.profilePermissions') }}
    </h2>
    <div class="flex flex-col gap-2">
      <details
        v-for="group in permissionGroups"
        :key="group.moduleKey"
        class="group rounded-lg border border-neutral-200 bg-neutral-30/50"
      >
        <summary
          class="
            flex cursor-pointer list-none items-center gap-2 px-4 py-3 text-start font-medium
            text-neutral-800 transition-colors
            hover:bg-neutral-200/50
            [&::-webkit-details-marker]:hidden
          "
        >
          <IconChevron
            class="
              size-5 shrink-0 text-neutral-500 transition-transform
              group-open:rotate-90
            "
          />
          <span class="flex-1">{{ group.moduleName }}</span>
          <span class="text-sm font-normal text-neutral-800">
            {{ group.permissions.length }}
          </span>
        </summary>
        <ul class="border-t border-neutral-200 px-4 py-3 ps-11">
          <li
            v-for="perm in group.permissions"
            :key="perm"
            class="py-1.5 text-sm text-neutral-700"
          >
            {{ formatPermissionLabel(perm) }}
          </li>
        </ul>
      </details>
    </div>
  </section>
</template>

<script setup lang="ts">
  import IconChevron from '@/components/icon/IconChevron.vue'

  const props = defineProps<{
    permissions: Record<string, string[]>
  }>()

  function formatGroupName(key: string): string {
    return key
      .split('_')
      .map(part => part.charAt(0).toUpperCase() + part.slice(1).toLowerCase())
      .join(' ')
  }

  function formatPermissionLabel(permission: string): string {
    return formatGroupName(permission.replace(/\./g, '_'))
  }

  const permissionGroups = computed(() => {
    const perms = props.permissions
    if (!perms || typeof perms !== 'object') return []

    return Object.entries(perms)
      .filter(([, list]) => Array.isArray(list) && list.length > 0)
      .map(([moduleKey, list]) => ({
        moduleKey,
        moduleName: formatGroupName(moduleKey),
        permissions: list as string[],
      }))
      .sort((a, b) => a.moduleName.localeCompare(b.moduleName))
  })
</script>
