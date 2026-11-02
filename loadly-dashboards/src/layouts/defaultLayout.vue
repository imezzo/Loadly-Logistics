<template>
  <BaseHeader @open-mobile-sidebar="isMobileSidebarOpen = true" />

  <BaseSidebar v-model="isMobileSidebarOpen" />

  <main class="page">
    <slot>
      <RouterView />
    </slot>
  </main>
</template>

<script setup lang="ts">
  const isMobileSidebarOpen = ref(false)
</script>

<style scoped>

</style>
