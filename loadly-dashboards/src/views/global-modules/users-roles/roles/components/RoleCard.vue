<template>
  <div class="rounded-xl border border-gray-50 p-1">
    <RouterLink
      :to="{ name: 'RolePermissions', params: { id: id } }"
      class="
        flex min-h-38 flex-col rounded-2xl border border-gray-50 bg-white p-4 transition-colors
        hover:shadow-xs
      "
    >
      <div class="flex items-start justify-between gap-4">
        <div class="flex-1">
          <h3 class="mb-2 text-lg font-bold text-neutral-800">
            {{ title }}
          </h3>

          <p class="text-sm leading-relaxed text-neutral-700">
            {{ description }}
          </p>
        </div>

        <div
          class="flex gap-2"
          @click.stop
        >
          <button
            class="btn-action-success"
            :aria-label="$t('a11y.edit')"
            @click.prevent="$emit('edit')"
          >
            <IconEdit class="size-4" />
          </button>

          <button
            v-if="showDelete"
            class="btn-action-error"
            :aria-label="$t('a11y.delete')"
            @click.prevent="$emit('delete')"
          >
            <IconTrash class="size-4" />
          </button>
        </div>
      </div>
    </RouterLink>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    title: string
    description: string
    id: number
    showDelete?: boolean
  }>()

  defineEmits<{
    edit: []
    delete: []
  }>()
</script>

<style scoped>

</style>
