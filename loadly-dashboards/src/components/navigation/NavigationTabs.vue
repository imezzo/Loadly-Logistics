<template>
  <div class="mb-6 border-b border-neutral-200">
    <nav class="-mb-px flex gap-8">
      <button
        v-for="(tab, index) in tabs"
        :key="index"
        type="button"
        class="
          flex items-center gap-2 border-b-2 px-1 py-4 text-sm font-medium transition-colors
          hover:text-primary
        "
        :class="activeTab === index
          ? 'border-primary text-primary'
          : 'border-transparent text-neutral-600'"
        @click="$emit('update:activeTab', index)"
      >
        <component
          :is="tab.icon"
          v-if="tab.icon"
          class="size-4"
          :color="activeTab === index
            ? 'var(--color-primary)'
            : 'rgba(0, 0, 0, 0.6)'"
        />
        {{ tab.title }}
      </button>
    </nav>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    tabs: {
      title: string
      icon?: Component
    }[]
    activeTab: number
  }>()

  defineEmits<{
    'update:activeTab': [index: number]
  }>()
</script>

<style scoped>

</style>
