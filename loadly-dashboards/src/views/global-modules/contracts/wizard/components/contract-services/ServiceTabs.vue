<template>
  <nav class="mb-6 flex gap-3 overflow-x-auto border-b border-gray-50">
    <button
      v-for="(tab, index) in tabs"
      :key="index"
      type="button"
      class="tab-item"
      :class="{'active': activeTab === index}"
      @click="$emit('select-tab', index)"
    >
      {{ tab.title }}
    </button>
  </nav>
</template>

<script setup lang="ts">
  defineProps<{
    tabs: {
      title: string
    }[]
    activeTab: number
  }>()

  defineEmits<{
    'select-tab': [index: number]
  }>()
</script>

<style scoped>

</style>
