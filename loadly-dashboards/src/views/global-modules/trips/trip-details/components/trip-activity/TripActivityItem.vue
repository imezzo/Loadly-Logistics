<template>
  <li
    class="
      flex flex-col gap-y-3 rounded-xl border border-gray-50 bg-white p-4 text-sm text-neutral-800
      shadow-sm
    "
  >
    <p>
      <span class="font-bold">{{ $t('content.addedBy') }}:</span>
      ({{ userType }}) {{ adminName }}
    </p>

    <p>{{ activityDescription }}</p>

    <div class="space-y-1 rounded-2xl border border-gray-100 bg-neutral-30 p-3">
      <p class="font-medium capitalize">
        {{ activityStatus }}
      </p>

      <p v-if="recording.data.waypoint?.addressName">
        {{ recording.data.waypoint.addressName }}
      </p>

      <ul
        v-if="activityDetails.length"
        class="space-y-2 text-xs"
      >
        <li
          v-for="(detail, index) in activityDetails"
          :key="index"
        >
          {{ detail }}
        </li>
      </ul>

      <template v-if="recordingAttachmentPaths.length">
        <p class="mt-3 text-xs font-semibold">
          {{ $t('content.tripActivityPhotos') }}
        </p>

        <div class="mt-2 flex flex-wrap gap-2">
          <a
            v-for="(path, index) in recordingAttachmentPaths"
            :key="`${recording.id}-${index}`"
            :href="path"
            target="_blank"
            rel="noopener noreferrer"
            class="
              size-16 shrink-0 overflow-hidden rounded-md border border-gray-100 bg-white
              transition-opacity
              hover:opacity-90
            "
          >
            <img
              v-if="isImagePath(path)"
              :src="path"
              :alt="extractFileName(path)"
              :aria-label="extractFileName(path)"
              class="size-full object-cover"
              loading="lazy"
            >

            <div
              v-else
              class="flex size-full items-center justify-center text-xs"
            >
              {{ extractFileName(path) }}
            </div>
          </a>
        </div>
      </template>
    </div>
  </li>
</template>

<script setup lang="ts">
  import type { TripWizardRecording } from '@/types/admin-dashboard/response/trip/trip'
  import { extractFileName, isImagePath } from '@/utils/file'
  import { formatDate, formatTime } from '@/utils/time'
  import { RecordingModelType } from '@/types/admin-dashboard/response/trip/trip'
  import { useEnum } from '@/composables/useEnum'
  import type { EnumItem } from '@/types/response/enum'

  const props = defineProps<{
    adminName?: string
    recording: TripWizardRecording
  }>()

  const { locale, t } = useI18n()
  const { enums } = useEnum()

  const userType = computed(() => (
    props.recording.addedByModelType.toLowerCase() === RecordingModelType.Admin
      ? t('form.operationAdmin')
      : t('content.driver')
  ))

  const recordingDate = computed(() => new Date(props.recording.createdAt))

  const activityDescription = computed(() => t('content.tripActivityDriverSent', {
    time: formatTime(recordingDate.value, locale.value),
    date: formatDate(recordingDate.value, locale.value),
  }))

  const activityStatus = computed(() => {
    const enumItem = enums.value?.RecordableEventType?.find(
      (item: EnumItem) => item.value === String(props.recording.eventType)
    )

    return enumItem ? enumItem.label : props.recording.eventType
  })

  const activityDetails = computed(() => {
    const { note, odometer, tollgatesTotalAmount } = props.recording.data

    return [
      ...(note ? [ `${t('content.note')}: ${note}` ]: []),
      ...(odometer ? [ t('content.tripActivityOdometer', { value: odometer }) ]: []),
      ...(tollgatesTotalAmount ? [`${t('content.tripActivityTollTotal', {
        amount: tollgatesTotalAmount,
        currency: t('form.egp'),
      })}`]: []),
    ]
  })

  const recordingAttachmentPaths = computed(() => {
    const {
      deliveryNoteImagePath = '',
      odometerImagePath = '',
      receiptNoteImagePath = '',
      shipmentImagePaths = [],
      tollgateImagePaths = [],
      unloadingImagePaths = [],
    } = props.recording.data

    return [
    ...shipmentImagePaths,
    ...unloadingImagePaths,
    ...tollgateImagePaths,
    receiptNoteImagePath,
    odometerImagePath,
    deliveryNoteImagePath,
    ].filter(Boolean) as string[]
  })
</script>

<style scoped>

</style>
