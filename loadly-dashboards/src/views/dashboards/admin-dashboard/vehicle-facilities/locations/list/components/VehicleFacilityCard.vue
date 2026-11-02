<template>
  <article
    class="
      flex cursor-pointer flex-col rounded-2xl border border-neutral-500 bg-white p-4
      transition-colors
      hover:shadow-xs
    "
    @click="goToView"
  >
    <div class="mb-3 flex items-start justify-between gap-2">
      <BaseChip
        :label="facilityTypeLabel"
        :variant="facilityTypeVariants[facility.facilityType] ?? 'neutral'"
      />

      <div
        class="flex shrink-0 items-center gap-2"
        @click.stop
      >
        <RouterLink
          v-if="hasEditPermission"
          :to="{ name: 'EditVehicleFacility', params: { id: facility.id } }"
          class="
            flex size-8 items-center justify-center rounded-full bg-info-50 transition-colors
            hover:bg-info-500/10
          "
          :aria-label="$t('a11y.edit')"
        >
          <IconCog class="size-4 text-info-500" />
        </RouterLink>

        <button
          v-if="hasDeletePermission"
          type="button"
          class="
            flex size-8 items-center justify-center rounded-full bg-black/3 transition-colors
            hover:bg-error-600/10
          "
          :aria-label="$t('a11y.delete')"
          @click="$emit('delete', facility)"
        >
          <IconTrash class="size-4" />
        </button>
      </div>
    </div>

    <h3 class="mb-3 font-bold text-neutral-10">
      {{ facility.name }}
    </h3>

    <template v-if="facility">
      <a
        :href="`mailto:${facility.email}`"
        class="
          mb-1 flex w-fit items-center gap-2 text-primary
          hover:underline
        "
        @click.stop
      >
        <IconClick class="size-4 shrink-0 text-primary" />

        <span class="truncate">
          {{ $t('form.email') }}: {{ facility.email }}
        </span>
      </a>

      <a
        :href="`tel:${facility.phone}`"
        class="
          flex w-fit items-center gap-2 text-primary
          hover:underline
        "
        @click.stop
      >
        <IconGroup class="size-4 shrink-0 text-primary" />

        <span class="truncate">
          {{ facility.contactPersonName || $t('content.contactPerson') }}
          ({{ facility.phone }})
        </span>
      </a>
    </template>
  </article>
</template>

<script setup lang="ts">
  import type { VehicleFacility } from '@/types/admin-dashboard/response/vehicle-facility'
  import { facilityTypeVariants } from '@/utils/statusChipVariants'
  import {
    Permission,
    VehicleFacility as VehicleFacilityPermission,
  } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useEnum } from '@/composables/useEnum'

  const props = defineProps<{
    facility: VehicleFacility
  }>()

  defineEmits<{
    delete: [facility: VehicleFacility]
  }>()

  const router = useRouter()
  const { getDisplayName } = useEnum()

  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.VehicleFacility, VehicleFacilityPermission.Edit)
  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.VehicleFacility, VehicleFacilityPermission.Delete)

  const facilityTypeLabel = computed(() =>
    getDisplayName('VehicleFacilityType', props.facility.facilityType)
  )

  function goToView(): void {
    router.push({ name: 'ViewVehicleFacility', params: { id: props.facility.id } })
  }
</script>

<style scoped>

</style>
