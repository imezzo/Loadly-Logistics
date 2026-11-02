<template>
  <h1 class="mb-6 font-bold text-neutral-10">
    {{ $t('content.contractVehicleSlot') }}
  </h1>

  <SlotsTable
    @add="addSlot"
    @assign="assignVehicle"
    @duplicate="duplicateSlot"
    @remove="removeSlot"
    @set-type="setSlotVehicleType"
  />

  <BaseActions
    :prev-button-options="{ hasScrollTop: true }"
    :next-button-options="{ label: saveButtonTitle, hasScrollTop: true }"
    @prev="emit('prev')"
    @next="next"
  />

  <EmptySlotsConfirmPopup
    v-model="isEmptySlotsPopup"
    @confirm="proceed"
  />
</template>

<script setup lang="ts">
  import { ContractVehicleSlotStatus } from '@/types/admin-dashboard/response/contract/contract-slots.enums'
  import type { ContractVehicleSlot } from '@/types/admin-dashboard/response/contract/contract-slots'
  import { useContractWizard } from '../../composables/useContractWizard'

  const SlotsTable = defineAsyncComponent(() => import('./slots-table/SlotsTable.vue'))
  const EmptySlotsConfirmPopup = defineAsyncComponent(() => import('./EmptySlotsConfirmPopup.vue'))

  const emit = defineEmits<{
    prev: []
    next: []
    update: [data: Record<string, unknown>]
  }>()

  const { formData, saveButtonTitle } = useContractWizard()

  const isEmptySlotsPopup = ref(false)

  const vehicleSlots = computed(() => (formData.value.vehicleSlots as ContractVehicleSlot[]) ?? [])

  const activeVehicleSlots = computed(() =>
    vehicleSlots.value.filter((slot) => slot.status !== ContractVehicleSlotStatus.Archived)
  )

  function persistSlots(slots: ContractVehicleSlot[]): void {
    emit('update', { vehicleSlots: slots })
  }

  function addSlot(): void {
    persistSlots([...vehicleSlots.value, {
      assignedAt: new Date().toISOString(),
      status: ContractVehicleSlotStatus.Active,
      vehicleTypeId: 0,
    }])
  }

  function duplicateSlot(index: number): void {
    const source = vehicleSlots.value[index]
    if (!source) return

    const raw = formData.value.vehiclesCount

    if (raw != null && String(raw).trim() !== ''
      && Number.isFinite(Number(raw)) && activeVehicleSlots.value.length >= Number(raw)) {
      return
    }

    const duplicatedSlot: ContractVehicleSlot = {
      assignedAt: source.assignedAt ?? new Date().toISOString(),
      status: ContractVehicleSlotStatus.Active,
      vehicleTypeId: source.vehicleTypeId,
    }

    const slots = [...vehicleSlots.value]
    slots.splice(index + 1, 0, duplicatedSlot)
    persistSlots(slots)
  }

  function setSlotVehicleType(index: number, vehicleTypeId: number): void {
    persistSlots(vehicleSlots.value.map((slot, slotIndex) =>
      slotIndex === index
        ? {
          ...slot,
          vehicleTypeId,
        }
        : slot
    ))
  }

  function assignVehicle(
    index: number,
    vehicle: Pick<ContractVehicleSlot, 'vehicleId' | 'plateNo' | 'plateLetters'>,
  ): void {
    persistSlots(vehicleSlots.value.map((slot, slotIndex) =>
      slotIndex === index
        ? { ...slot, ...vehicle, status: ContractVehicleSlotStatus.Active }
        : slot
    ))
  }

  function removeSlot(index: number): void {
    const slot = vehicleSlots.value[index]
    if (!slot) return

    if (!slot.id) {
      persistSlots(vehicleSlots.value.filter((_, slotIndex) => slotIndex !== index))

      return
    }

    persistSlots(vehicleSlots.value.map((item, slotIndex) =>
      slotIndex === index
        ? {
            ...item,
            archivedAt: new Date().toISOString(),
            status: ContractVehicleSlotStatus.Archived,
          }
        : item
    ))
  }

  function proceed(): void {
    persistSlots(vehicleSlots.value)
    emit('next')
  }

  function next(): void {
    if (activeVehicleSlots.value.some(slot => !slot.vehicleId || !slot.vehicleTypeId)) {
      isEmptySlotsPopup.value = true

      return
    }

    proceed()
  }

  defineExpose({ submitStep: next })
</script>

<style scoped>

</style>
