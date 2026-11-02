<template>
  <div class="h-auto min-w-0 flex-1">
    <button
      v-if="!hasNote && !isDocumentStatusApproved && trip.isEditable"
      type="button"
      class="
        flex size-full items-center justify-center rounded-xl border border-dashed border-primary
        bg-primary/5 px-4 text-base font-medium text-primary transition-colors
        hover:bg-primary/10
      "
      @click="openPopup"
    >
      <IconPlus class="size-4 text-primary" />
      {{ $t('button.addNote') }}
    </button>

    <div
      v-if="hasNote"
      class="flex h-full flex-col justify-center rounded-xl border border-primary bg-white p-4"
    >
      <div class="flex items-center gap-2">
        <p class="text-base font-bold text-neutral-10">
          {{ $t('content.theNote') }}
        </p>

        <button
          v-if="!isDocumentStatusApproved && trip.isEditable"
          type="button"
          class="
            flex size-8 shrink-0 items-center justify-center rounded-full bg-primary/10
            text-neutral-10 transition-colors
            hover:bg-primary/20
          "
          :aria-label="$t('title.editNote')"
          @click="openPopup"
        >
          <IconEditDocument
            color="currentColor"
            class="size-4"
          />
        </button>
      </div>

      <p class="mt-1 line-clamp-3 max-h-[100px] overflow-y-auto text-sm break-all text-neutral-10">
        {{ trip.dispatcherNotes }}
      </p>
    </div>

    <TripNotesPopup
      v-if="showPopup && trip.isEditable"
      v-model="showPopup"
      :initial-notes="trip.dispatcherNotes"
    />
  </div>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { TripDocumentStatus } from '@/types/admin-dashboard/response/trip/trip-document-status.enums'

  const TripNotesPopup = defineAsyncComponent(() => import('./TripNotesPopup.vue'))

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const showPopup = ref(false)

  const hasNote = computed(() => Boolean(trip.value.dispatcherNotes?.trim()))

  const isDocumentStatusApproved = computed(() => trip.value.tripDocumentStatus === TripDocumentStatus.Approved)

  function openPopup(): void {
    showPopup.value = true
  }
</script>

<style scoped>

</style>
