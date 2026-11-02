<template>
  <ul class="mt-8">
    <li
      v-for="(waypoint, index) in waypoints"
      :key="index"
      class="flex gap-4 rounded-sm bg-white"
      :class="{ 'cursor-grab active:cursor-grabbing': !readOnly }"
      :draggable="!readOnly"
      @dragstart="onDragStart($event, index)"
      @dragend="onDragEnd"
      @dragover.prevent="onDragOver($event, index)"
      @drop.prevent="onDrop($event, index)"
      @dragleave="onDragLeave"
    >
      <span
        class="
          relative flex size-4 shrink-0 flex-col items-center rounded-full bg-primary
          before:absolute before:bottom-1 before:left-1 before:z-2 before:size-2 before:rounded-full
          before:bg-white before:content-['']
          after:absolute after:z-1 after:h-25 after:content-['']
        "

        :class="{'after:min-h-4 after:w-0.5 after:flex-1 after:bg-primary': index < waypoints.length - 1}"
      />

      <div class="flex-1 py-0.5">
        <div class="mb-1 flex items-start justify-between gap-2">
          <span class="font-semibold text-neutral-800">
            {{ index + 1 }}. {{ waypointTypeLabel(waypoint.stopType) }}
          </span>

          <BaseChip
            v-if="trip.destinationStatus"
            class="ms-auto shrink-0 self-start"
            :label="trip.destinationStatusName"
            :variant="contractDestinationStatusVariants[trip.destinationStatus]"
          />

          <div
            v-if="showWaypointActions(waypoint)"
            class="flex shrink-0 items-center gap-1"
          >
            <button
              type="button"
              class="btn-info-outline"
              :title="$t('button.edit')"
              :aria-label="$t('button.edit')"
              @click="$emit('edit', index)"
            >
              <IconEdit class="size-5" />
            </button>

            <button
              type="button"
              class="btn-action-error"
              :title="$t('button.delete')"
              :aria-label="$t('button.delete')"
              @click="$emit('remove', index)"
            >
              <IconTrash class="size-5" />
            </button>
          </div>
        </div>

        <div class="flex items-start gap-2 text-neutral-700">
          <IconLocation
            filled
            class="mt-0.5 size-5 shrink-0 text-neutral-10"
          />
          <span class="text-sm">{{ waypoint.addressName || '—' }}</span>
        </div>

        <div class="mt-1 flex items-start gap-2 text-neutral-600">
          <IconEditDocument class="mt-0.5 size-5 shrink-0 text-neutral-400" />
          <span class="text-sm">
            {{ $t('form.note') }}: {{ waypoint.taskDetails || $t('form.noNote') }}
          </span>
        </div>
      </div>
    </li>
  </ul>
</template>

<script setup lang="ts">
  import { useEnum } from '@/composables/useEnum'
  import type { TripWizardWaypoint } from '@/types/admin-dashboard/request/trip/trip'
  import type { EnumItem } from '@/types/response/enum'
  import { useTripWizard } from '../../composables/useTripWizard'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { TripStatus, tripWaypointStatus } from '@/types/admin-dashboard/response/trip/trip'
  import { contractDestinationStatusVariants } from '@/utils/statusChipVariants'
  import { ContractDestinationStatus } from '@/types/admin-dashboard/response/contract/contract-destination.enums'

  const props = defineProps<{
    waypoints: TripWizardWaypoint[]
    readOnly?: boolean
  }>()

  const emit = defineEmits<{
    edit: [index: number]
    remove: [index: number]
    reorder: [waypoints: TripWizardWaypoint[]]
  }>()

  const { enums } = useEnum()
  const { isFormDisabled } = useTripWizard()

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const isAllowedStatus = computed(() => (
    [
      TripStatus.REQUESTED,
      TripStatus.PENDING,
      TripStatus.SCHEDULED
    ].includes(Number(trip.value.status))
  ))

  const hasWaypointActions = computed(() => (
    !isFormDisabled.value
    && !props.readOnly
    && (!trip.value.status || isAllowedStatus.value)
  ))

  const showWaypointActions = computed(() => {
    const isEditableDestination = !props.readOnly
      && trip.value.destinationStatus !== ContractDestinationStatus.Archived
    const isInProgress = Number(trip.value.status) === TripStatus.IN_PROGRESS
    const isCompleted = Number(trip.value.status) === TripStatus.COMPLETED

    return (waypoint: TripWizardWaypoint) => isEditableDestination
      && (hasWaypointActions.value
        || (isInProgress && Number(waypoint.status) === tripWaypointStatus.Pending)
        || isCompleted
        || !waypoint.status)
  })

  const dragIndex = ref<number | null>(null)
  const dropIndex = ref<number | null>(null)

  function onDragStart(event: DragEvent, index: number): void {
    if (props.readOnly) return
    dragIndex.value = index
    event.dataTransfer?.setData('text/plain', String(index))
    if (event.dataTransfer) event.dataTransfer.effectAllowed = 'move'
    if (event.target instanceof HTMLElement) {
      event.target.style.opacity = '0.6'
    }
  }

  function onDragEnd(event: DragEvent): void {
    dragIndex.value = null
    dropIndex.value = null
    if (event.target instanceof HTMLElement) {
      event.target.style.opacity = ''
    }
  }

  function onDragOver(_event: DragEvent, index: number): void {
    if (props.readOnly || dragIndex.value === null) return
    dropIndex.value = index
  }

  function onDragLeave() {
    dropIndex.value = null
  }

  function onDrop(_event: DragEvent, toIndex: number): void {
    if (props.readOnly) return
    const fromIndex = dragIndex.value

    if (fromIndex === null || fromIndex === toIndex) {
      dragIndex.value = null
      dropIndex.value = null

      return
    }
    const list = [...props.waypoints]
    const [removed] = list.splice(fromIndex, 1)

    if (!removed) {
      dragIndex.value = null
      dropIndex.value = null

      return
    }

    list.splice(toIndex, 0, removed)
    emit('reorder', list)
    dragIndex.value = null
    dropIndex.value = null
  }

  function waypointTypeLabel(stopType: string): string {
    const options = (enums.value?.TripWaypointStopType ?? []) as EnumItem[]

    return options.find((option) => option.value === stopType)?.label ?? stopType
  }
</script>

<style scoped>

</style>
