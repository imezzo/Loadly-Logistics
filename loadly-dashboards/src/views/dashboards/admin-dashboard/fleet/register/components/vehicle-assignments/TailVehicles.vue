<template>
  <div class="flex flex-col">
    <h2 class="mb-2 font-bold text-neutral-10">
      {{ $t('content.tailVehicle') }}
    </h2>

    <div class="mb-6 flex flex-wrap gap-4">
      <template
        v-for="(tail, index) in tailVehicles"
        :key="tail.id ?? index"
      >
        <div
          class="flex w-full items-start gap-4 rounded-2xl bg-white p-4"
        >
          <div class="flex flex-1 flex-col items-start">
            <div class="flex flex-1 items-start gap-4">
              <IconVehicle class="size-15 text-neutral-600" />

              <BaseVehiclePlate
                :plate-letters="tail.plateLetters ?? ''"
                :plate-no="tail.plateNo ?? ''"
              />
            </div>

            <p class="font-bold text-neutral-600">
              {{ tail.vehicleTypeName || tail.displayText }}
            </p>
          </div>

          <button
            type="button"
            class="btn-action-error"
            :aria-label="$t('button.remove')"
            :disabled="isFormDisabled"
            @click="emit('delete', index)"
          >
            <IconTrash class="size-5" />
          </button>
        </div>
      </template>

      <button
        v-if="tailVehicles.length < 1"
        type="button"
        class="
          mb-6 flex w-full flex-col items-center justify-center gap-2 rounded-2xl bg-white py-8
          md:w-1/2
        "
        :disabled="isFormDisabled"
        @click="emit('add')"
      >
        <span
          class="flex size-11 items-center justify-center rounded-full bg-primary text-white"
        >
          <IconPlus class="size-6" />
        </span>
        <span class="font-bold text-neutral-10">
          {{ $t('content.addTailVehicle') }}
        </span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useVehicleWizard } from '../../composables/useVehicleWizard'

  export interface TailVehicleItem {
    id: number
    plateNo?: string
    plateLetters?: string
    displayText?: string
    vehicleTypeName?: string
  }

  defineProps<{
    tailVehicles: TailVehicleItem[]
  }>()

  const emit = defineEmits<{
    add: []
    delete: [index: number]
  }>()

  const { isFormDisabled } = useVehicleWizard()
</script>

<style scoped>

</style>
