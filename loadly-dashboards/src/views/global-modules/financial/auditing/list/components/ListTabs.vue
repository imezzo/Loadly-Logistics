<template>
  <div class="col-span-full flex items-center gap-2">
    <RouterLink
      :to="{ name: 'FinancialTripAuditingCompany' }"
      class="tab-primary-outline shrink-0"
      :class="{ active: !isExternalTrip }"
    >
      {{ $t('content.company') }}
    </RouterLink>

    <RouterLink
      :to="{ name: 'FinancialTripAuditingClients' }"
      class="tab-primary-outline shrink-0"
      :class="{ active: isExternalTrip }"
    >
      {{ $t('title.clients') }}
    </RouterLink>
  </div>
</template>

<script setup lang="ts">
  import { useTripAuditingList } from '../composables/useTripAuditingList'

  const { isExternalTrip } = useTripAuditingList()
</script>

<style scoped>

</style>
