<template>
  <div class="flex items-center gap-2">
    <div class="w-25 shrink-0">
      <BaseVehiclePlate
        :plate-letters="plateLetters"
        :plate-no="plateNo"
      />
    </div>

    <BaseTooltip
      v-if="plateChangeHistory?.length"
      :trigger-label="$t('content.oldPlates')"
    >
      <template #trigger>
        <IconInfo
          variant="filled"
          class="size-5 text-neutral-40"
        />
      </template>

      <p class="mb-1 font-medium">
        {{ $t('content.oldPlates') }}
      </p>
      <ul class="flex flex-col gap-1">
        <li
          v-for="(item, index) in plateChangeHistory"
          :key="`${item.plateNo}-${item.supersededAt}-${index}`"
        >
          <bdi>
            {{ [item.plateLetters, item.plateNo].filter(Boolean).join(' ') }}
          </bdi>
          <bdi class="text-neutral-700">
            · {{ formatSupersededAt(item.supersededAt) }}
          </bdi>
        </li>
      </ul>
    </BaseTooltip>
  </div>
</template>

<script setup lang="ts">
  import type { ApiPlateChangeHistory } from '@/types/admin-dashboard/response/vehicle/vehicle-detail'
  import { formatDate, formatTime } from '@/utils/time'

  defineProps<{
    plateLetters?: string
    plateNo?: string
    plateChangeHistory?: ApiPlateChangeHistory[]
  }>()

  const { t, locale } = useI18n()

  function formatSupersededAt(value: string): string {
    const date = new Date(value)
    if (Number.isNaN(date.getTime())) return value

    return `${formatDate(date, locale.value)} ${t('content.at')} ${formatTime(date, locale.value)}`
  }
</script>

<style scoped>

</style>
