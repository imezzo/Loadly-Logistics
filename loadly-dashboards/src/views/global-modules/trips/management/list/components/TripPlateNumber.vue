<template>
  <div class="flex items-center gap-2">
    <div class="w-25 shrink-0">
      <BaseVehiclePlate
        :plate-letters="plateLetters"
        :plate-no="plateNo"
      />
    </div>

    <BaseTooltip
      v-if="hasSnapshot"
      :trigger-label="$t('table.plateNumber')"
    >
      <template #trigger>
        <IconInfo
          variant="filled"
          class="size-5 text-neutral-40"
        />
      </template>

      <p class="mb-1 font-medium">
        {{ $t('content.currentPlate') }}
      </p>

      <ul class="flex flex-col gap-1">
        <li v-if="showHead && headPlate">
          <span>{{ $t('content.head') }}: </span>
          <bdi>{{ headPlate }}</bdi>
          <span v-if="vehicleSnapshot?.headPlateChangedAt">
            ({{ $t('content.changedAt') }}: {{ formatDateLocal(vehicleSnapshot?.headPlateChangedAt) }})
          </span>
        </li>

        <li v-if="showTail && tailPlate">
          <span>{{ $t('content.tail') }}: </span>
          <bdi>{{ tailPlate }}</bdi>
          <span v-if="vehicleSnapshot?.tailPlateChangedAt">
            ({{ $t('content.changedAt') }}: {{ formatDateLocal(vehicleSnapshot?.tailPlateChangedAt) }})
          </span>
        </li>
      </ul>
    </BaseTooltip>
  </div>
</template>

<script setup lang="ts">
  import type { ApiVehicleSnapshot } from '@/types/admin-dashboard/response/trip/trip'
  import { formatDateLocal } from '@/utils/date'

  const props = defineProps<{
    headOnly?: boolean
    plateLetters?: string
    plateNo?: string
    tailOnly?: boolean
    vehicleSnapshot?: ApiVehicleSnapshot | null
  }>()

  const showHead = computed(() => (props.headOnly || !props.tailOnly) && Boolean(props.vehicleSnapshot?.headPlateChangedAt))
  const showTail = computed(() => (props.tailOnly || !props.headOnly) && Boolean(props.vehicleSnapshot?.tailPlateChangedAt))

  function formatPlate(letters?: string | null, plateNo?: string | null): string {
    return [letters, plateNo].filter(Boolean).join(' ')
  }

  const headPlate = computed(() =>
    formatPlate(props.vehicleSnapshot?.headPlateLetters, props.vehicleSnapshot?.headPlateNo)
  )

  const tailPlate = computed(() =>
    formatPlate(props.vehicleSnapshot?.tailPlateLetters, props.vehicleSnapshot?.tailPlateNo)
  )

  const hasSnapshot = computed(() =>
    (showHead.value && Boolean(headPlate.value)) || (showTail.value && Boolean(tailPlate.value))
  )
</script>

<style scoped>

</style>
