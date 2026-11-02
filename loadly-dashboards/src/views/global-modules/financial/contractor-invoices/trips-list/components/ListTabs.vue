<template>
  <div class="mb-6 flex items-center gap-2">
    <RouterLink
      :to="{ name: 'FinancialContractorTrips', params: { contractorId } }"
      class="tab-primary-outline shrink-0"
      :class="{ active: !isPayouts }"
    >
      {{ $t('content.allTrips') }}
    </RouterLink>

    <RouterLink
      :to="{ name: 'FinancialContractorPayouts', params: { contractorId } }"
      class="tab-primary-outline shrink-0"
      :class="{ active: isPayouts }"
    >
      {{ $t('content.payouts') }}
    </RouterLink>
  </div>
</template>

<script setup lang="ts">
  import { useContractorTripsList } from '../all-trips/composables/useContractorTripsList'

  const { contractorId, isPayouts } = useContractorTripsList()
</script>

<style scoped>

</style>
