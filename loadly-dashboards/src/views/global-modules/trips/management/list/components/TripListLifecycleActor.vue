<template>
  <span v-if="trip.lifecycleActor === TripLifecycleActor.Driver">
    {{ trip.lifecycleActorName }}
  </span>

  <div
    v-else-if="trip.lifecycleActor"
    class="text-xs"
  >
    <button
      v-if="hasOverflowAdmins"
      type="button"
      class="flex text-xs text-primary"
      :aria-label="$t('table.doneBy')"
      @click="isNamesPopupOpen = true"
    >
      <IconInfo class="me-1 size-3.25 shrink-0" />
      {{ assistedAdminNames[0] }}
    </button>

    <span v-else>{{ assistedAdminNames.join(', ') }}</span>

    <p
      v-if="trip.lifecycleActor === TripLifecycleActor.Assisted"
      class="text-xs"
    >
      <span class="me-1 font-medium">{{ $t('content.driver') }}:</span>
      <span>{{ trip.driverName }}</span>
    </p>
  </div>

  <BasePopup
    v-if="hasOverflowAdmins"
    v-model="isNamesPopupOpen"
    content-class="max-w-md"
  >
    <div class="flex w-full flex-col gap-3 px-6 py-2">
      <h2 class="text-xl font-bold text-neutral-10">
        {{ $t('table.doneBy') }}
      </h2>

      <ul class="flex flex-col gap-2 text-sm text-neutral-800">
        <li
          v-for="(name, index) in assistedAdminNames"
          :key="`${name}-${index}`"
        >
          {{ name }}
        </li>

        <li v-if="trip.lifecycleActor === TripLifecycleActor.Assisted">
          <span class="font-bold">{{ $t('content.driver') }}:</span>
          {{ trip.driverName }}
        </li>
      </ul>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
  import { TripLifecycleActor } from '@/types/admin-dashboard/response/trip/trip-lifecycle-actor.enums'
  import { useTripListLifecycleActor } from '../composables/useTripListLifecycleActor'

  const props = defineProps<{
    trip: Trip
  }>()

  const { getAssistedAdminNames } = useTripListLifecycleActor()

  const isNamesPopupOpen = ref(false)
  const assistedAdminNames = computed(() => getAssistedAdminNames(props.trip))
  const hasOverflowAdmins = computed(() => assistedAdminNames.value.length > 1)
</script>

<style scoped>

</style>
