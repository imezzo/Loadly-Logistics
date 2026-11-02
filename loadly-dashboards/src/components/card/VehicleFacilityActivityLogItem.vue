<template>
  <CardPreview title="">
    <template #title>
      <span class="text-sm font-medium">
        {{ $t('content.user') }}: {{ userName }}
      </span>

      <span
        class="mx-2 text-sm font-normal text-neutral-500"
        aria-hidden="true"
      >|</span>

      <time
        class="text-sm font-normal text-neutral-600"
        :datetime="visit.eventDate"
      >
        {{ formattedDate }}
      </time>

      <span
        class="mx-2 text-sm font-normal text-neutral-500"
        aria-hidden="true"
      >|</span>

      <span
        class="
          text-sm font-medium text-neutral-10
          md:col-span-3
        "
      >
        {{ getDisplayName('VehicleFacilityVisitEvent', props.visit.event) }}
      </span>
    </template>

    <template #content>
      <template v-if="visit.vehicle">
        <CardPreviewLabel
          :label="$t('content.vehicleNumber')"
          :content="vehiclePlate"
        />

        <CardPreviewLabel
          v-if="visit.vehicle.vehicleTypeName"
          :label="$t('table.vehicleType')"
          :content="visit.vehicle.vehicleTypeName"
        />

        <CardPreviewLabel
          v-if="visit.reason"
          :label="$t('content.reason')"
          :content="visit.reason"
        />
      </template>
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import type { VehicleFacilityVisit } from '@/types/admin-dashboard/response/vehicle-facility'
  import { useEnum } from '@/composables/useEnum'
  import { formatDate, formatTime } from '@/utils/time'

  const props = defineProps<{
    visit: VehicleFacilityVisit
  }>()

  const { locale, t } = useI18n()
  const { getDisplayName } = useEnum()

  const userName = computed(() => props.visit.addedBy?.fullName || '—')

  const formattedDate = computed(() => {
    const date = new Date(props.visit.eventDate)
    if (Number.isNaN(date.getTime())) return '—'

    return `${formatDate(date, locale.value)} ${t('content.at')} ${formatTime(date, locale.value)}`
  })

  const vehiclePlate = computed(() =>
    [props.visit.vehicle?.plateNo, props.visit.vehicle?.plateLetters]
      .filter(Boolean)
      .join(' '),
  )
</script>

<style scoped>

</style>
