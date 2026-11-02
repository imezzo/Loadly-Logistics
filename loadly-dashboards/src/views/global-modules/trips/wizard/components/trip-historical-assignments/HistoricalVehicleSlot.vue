<template>
  <div class="flex flex-col">
    <h2 class="mb-2 font-bold text-neutral-10">
      {{ label }}
    </h2>

    <div
      v-if="vehicle"
      class="flex w-full items-start gap-4 rounded-2xl bg-white p-4"
    >
      <div class="flex flex-1 flex-col items-start">
        <div class="flex flex-1 items-start gap-4">
          <IconVehicle class="size-15 text-neutral-600" />

          <BaseVehiclePlate
            :plate-letters="vehicle.plateLetters ?? ''"
            :plate-no="vehicle.plateNo ?? ''"
          />
        </div>

        <p class="font-bold text-neutral-600">
          {{ vehicle.vehicleTypeName }}
        </p>
      </div>

      <button
        type="button"
        class="btn-action-error"
        :aria-label="$t('button.remove')"
        :disabled="disabled"
        @click="$emit('remove')"
      >
        <IconTrash class="size-5" />
      </button>
    </div>

    <button
      v-else
      type="button"
      class="flex w-full flex-col items-center justify-center gap-2 rounded-2xl bg-white py-8"
      :disabled="disabled"
      @click="$emit('add')"
    >
      <span
        class="flex size-11 items-center justify-center rounded-full bg-primary text-white"
      >
        <IconPlus class="size-6" />
      </span>
      <span class="font-bold text-neutral-10">
        {{ addLabel }}
      </span>
    </button>
  </div>
</template>

<script setup lang="ts">
  export interface AssignedUnit {
    id: number
    plateLetters?: string
    plateNo?: string
    vehicleTypeName?: string
  }

  defineProps<{
    addLabel: string
    disabled?: boolean
    label: string
    vehicle: AssignedUnit | null
  }>()

  defineEmits<{
    add: []
    remove: []
  }>()
</script>

<style scoped>

</style>
