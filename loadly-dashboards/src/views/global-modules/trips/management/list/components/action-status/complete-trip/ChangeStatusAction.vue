<template>
  <CompleteTripContent
    v-if="showForm"
    :trip-id="tripId"
    @discard="$emit('discard')"
    @completed="$emit('completed')"
  />

  <TripActionStatusCard
    v-else
    :label="$t('button.changeStatus')"
    class="
      group bg-info-500! text-white!
      hover:bg-info-500/95!
    "
    @select="$emit('select')"
  >
    <template #icon>
      <IconTrip
        class="
          size-8 text-white transition-transform duration-300
          group-hover:scale-110
        "
      />
    </template>
  </TripActionStatusCard>
</template>

<script setup lang="ts">
  import TripActionStatusCard from '../TripActionStatusCard.vue'

  const CompleteTripContent = defineAsyncComponent(() => import('./CompleteTripContent.vue'))

  defineProps<{
    tripId: string | number
    showForm?: boolean
  }>()

  defineEmits<{
    select: []
    discard: []
    completed: []
  }>()
</script>

<style scoped>

</style>
