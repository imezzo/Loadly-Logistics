<template>
  <span
    ref="triggerRef"
    class="inline-flex"
    @mouseenter="openTooltip"
    @mouseleave="closeTooltip"
    @focusin="openTooltip"
    @focusout="closeTooltip"
  >
    <button
      type="button"
      class="inline-flex cursor-help border-0 bg-transparent p-0"
      :aria-describedby="isOpen ? tooltipId : undefined"
      :aria-label="triggerLabel"
    >
      <slot name="trigger" />
    </button>
  </span>

  <Teleport to="body">
    <div
      v-if="isOpen"
      :id="tooltipId"
      ref="tooltipRef"
      role="tooltip"
      class="
        fixed z-50 max-w-xs rounded-lg border border-neutral-200 bg-white px-3 py-2 text-sm
        text-neutral-10 shadow-lg
      "
      :class="{ 'pointer-events-none': !interactive }"
      :style="position"
      @mouseenter="openTooltip"
      @mouseleave="closeTooltip"
    >
      <slot />
    </div>
  </Teleport>
</template>

<script setup lang="ts">
  import { useEventListener } from '@vueuse/core'

  const props = defineProps<{
    interactive?: boolean
    triggerLabel: string
  }>()

  const triggerRef = useTemplateRef<HTMLElement>('triggerRef')
  const tooltipRef = useTemplateRef<HTMLElement>('tooltipRef')

  const tooltipId = useId()

  const isOpen = ref(false)
  const position = ref({ top: '0px', left: '0px' })
  let closeTimer: ReturnType<typeof setTimeout> | undefined

  function clearCloseTimer(): void {
    if (closeTimer == null) return

    clearTimeout(closeTimer)
    closeTimer = undefined
  }

  function updatePosition(): void {
    const trigger = triggerRef.value
    const tooltip = tooltipRef.value
    if (!trigger || !tooltip) return

    const rect = trigger.getBoundingClientRect()
    const gap = 8
    const tooltipWidth = tooltip.offsetWidth
    const tooltipHeight = tooltip.offsetHeight
    let left = rect.left + rect.width / 2 - tooltipWidth / 2
    left = Math.max(gap, Math.min(left, window.innerWidth - tooltipWidth - gap))

    let top = rect.top - tooltipHeight - gap
    if (top < gap) {
      top = rect.bottom + gap
    }

    position.value = {
      top: `${top}px`,
      left: `${left}px`,
    }
  }

  async function openTooltip(): Promise<void> {
    clearCloseTimer()
    isOpen.value = true
    await nextTick()
    updatePosition()
  }

  function closeTooltip(): void {
    clearCloseTimer()

    if (!props.interactive) {
      isOpen.value = false

      return
    }

    closeTimer = setTimeout(() => {
      isOpen.value = false
    }, 150)
  }

  onUnmounted(clearCloseTimer)

  useEventListener(window, 'scroll', () => {
    if (isOpen.value) updatePosition()
  }, true)

  useEventListener(window, 'resize', () => {
    if (isOpen.value) updatePosition()
  })
</script>

<style scoped>

</style>
