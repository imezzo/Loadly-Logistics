<template>
  <PopupSuccess
    v-if="isSuccess"
    v-model="isSuccess"
    :message="successMessage"
  />

  <button
    v-if="isClickableStatus"
    type="button"
    class="
      inline-flex items-center gap-1 rounded-lg transition-opacity
      hover:opacity-80
    "
    :aria-label="$t('title.actionStatus')"
    @click="isMenuOpen = true"
  >
    <BaseChip
      :label
      :variant
    >
      <IconClick class="me-1 size-3.5 shrink-0" />

      {{ label }}
    </BaseChip>
  </button>

  <BaseChip
    v-else
    :label
    :variant
  />

  <TripActionStatusPopup
    v-model="isMenuOpen"
    :trip-id
    :can-cancel="canCancel"
    @cancelled="onCancelled"
    @completed="onCompleted"
    @updated="emit('updated')"
  />
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useApp } from '@/composables/useApp'
  import { Permission, Trip as TripPermission } from '@/types/response/permission-group'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'
  import type { ChipVariant } from '@/types/ui'
  import { tripStatusVariants } from '@/utils/statusChipVariants'

  const TripActionStatusPopup = defineAsyncComponent(() => import('./action-status/TripActionStatusPopup.vue'))
  const PopupSuccess = defineAsyncComponent(() => import('@/components/popup/PopupSuccess.vue'))

  const props = defineProps<{
    label: string
    status: TripStatus
    tripId: string | number
  }>()

  const emit = defineEmits<{
    updated: []
  }>()

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const { hasPermission: hasSelfCompleteTripPermission } = usePermission(
    Permission.Trip,
    TripPermission.SelfCompleteTrip,
  )
  const { hasPermission: hasCancelPermission } = usePermission(
    Permission.Trip,
    TripPermission.Cancel,
  )

  const isMenuOpen = ref(false)
  const isSuccess = ref(false)
  const successMessage = ref('')

  const variant = computed<ChipVariant>(() => tripStatusVariants[props.status] ?? 'neutral')
  const isScheduled = computed(() => props.status === TripStatus.SCHEDULED)
  const isInProgress = computed(() => props.status === TripStatus.IN_PROGRESS)
  const canCancel = computed(() => isAdminDashboard && hasCancelPermission.value)

  const isClickableStatus = computed(() => {
    return isAdminDashboard
      && hasSelfCompleteTripPermission.value
      && (isScheduled.value || isInProgress.value)
  })

  function onCompleted(): void {
    successMessage.value = t('success.tripCompleted')
    isSuccess.value = true
    emit('updated')
  }

  function onCancelled(): void {
    successMessage.value = t('success.tripCancelled')
    isSuccess.value = true
    emit('updated')
  }
</script>

<style scoped>

</style>
