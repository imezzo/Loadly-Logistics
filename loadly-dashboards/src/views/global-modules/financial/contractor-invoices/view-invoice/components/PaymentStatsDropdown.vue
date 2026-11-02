<template>
  <div
    ref="dropdownRef"
    class="relative"
  >
    <button
      type="button"
      class="flex items-center gap-3 rounded-full border border-neutral-500 px-4 py-2"
      :aria-expanded="isMenuOpen"
      :disabled="isSettingPaid || !isPending"
      @click.stop="toggleMenu"
    >
      <span class="text-sm text-neutral-700">
        {{ $t('content.paymentStats') }}
      </span>

      <span
        class="text-sm font-semibold"
        :class="status === ContractorPayoutStatus.Paid ? 'text-success-600' : 'text-error-500'"
      >
        {{ statusName }}
      </span>

      <IconChevronDown
        v-if="isPending"
        class="size-4 text-neutral-700 transition-transform"
        :class="{ 'rotate-180': isMenuOpen }"
      />
    </button>

    <div
      v-if="isMenuOpen && isPending"
      class="
        absolute end-0 top-full z-20 mt-1 min-w-40 rounded-xl border border-neutral-200 bg-white
        py-1 shadow-lg
      "
      role="menu"
      @click.stop
    >
      <button
        type="button"
        class="
          hover:bg-primary-50
          w-full px-4 py-2.5 text-start text-sm text-neutral-800
        "
        role="menuitem"
        :disabled="isSettingPaid"
        @click="onSetPaid"
      >
        {{ $t('content.paid') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ContractorPayoutStatus } from '@/types/response/financial/contractor-payout.enums';
  import { onClickOutside } from '@vueuse/core'

  const props = defineProps<{
    isSettingPaid: boolean
    status: ContractorPayoutStatus
    statusName: string
  }>()

  const emit = defineEmits<{
    setPaid: []
  }>()

  const isMenuOpen = ref(false)
  const dropdownRef = useTemplateRef<HTMLElement | null>('dropdownRef')

  onClickOutside(dropdownRef, () => {
    isMenuOpen.value = false
  })

  const isPending = computed(() => props.status === ContractorPayoutStatus.Pending)

  function toggleMenu(): void {
    if (!isPending.value || props.isSettingPaid) return

    isMenuOpen.value = !isMenuOpen.value
  }

  function onSetPaid(): void {
    isMenuOpen.value = false
    emit('setPaid')
  }
</script>

<style scoped>

</style>
