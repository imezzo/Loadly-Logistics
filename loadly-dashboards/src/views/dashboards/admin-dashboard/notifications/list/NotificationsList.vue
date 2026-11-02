<template>
  <div class="page-content">
    <NotificationsListHeader />
    <NotificationsListFilters />
    <NotificationsTable />
  </div>
</template>

<script setup lang="ts">
  import NotificationsListHeader from './components/NotificationsListHeader.vue'
  import NotificationsListFilters from './components/NotificationsListFilters.vue'
  import NotificationsTable from './components/NotificationsTable.vue'
  import { useList } from './composables/useList'

  const { fetchNotifications } = useList()

  onMounted(fetchNotifications)
</script>

<style scoped>

</style>
