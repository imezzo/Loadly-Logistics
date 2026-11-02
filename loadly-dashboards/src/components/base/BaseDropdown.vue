<template>
  <div
    ref="dropdownRef"
    class="relative flex justify-end"
  >
    <button
      type="button"
      class="btn-primary"
      :aria-expanded="isMenuOpened"
      :aria-label="$t('table.action')"
      :disabled="disabled"
      @click.stop="toggleActionMenu"
    >
      <slot name="icon">
        <IconDots class="size-5 text-neutral-800" />
      </slot>
    </button>

    <div
      v-if="isMenuOpened"
      class="
        absolute end-0 top-full z-20 mt-1 min-w-52 rounded-xl border border-neutral-200 bg-white
        py-1 shadow-lg
      "
      role="menu"
      @click.stop
    >
      <template
        v-for="(option, index) in options"
        :key="`dropdown-option-${index}`"
      >
        <BaseLink
          v-if="option.to"
          :to="option.to"
          class="
            hover:bg-primary-50
            block w-full px-4 py-2.5 text-start text-sm text-neutral-800
          "
          role="menuitem"
          @click="onLinkOptionSelect(option)"
        >
          {{ option.label }}
        </BaseLink>

        <button
          v-else
          type="button"
          class="
            hover:bg-primary-50
            w-full px-4 py-2.5 text-start text-sm text-neutral-800
          "
          role="menuitem"
          @click="onSelectOption(option)"
        >
          {{ option.label }}
        </button>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { DropdownMenuOption } from '@/types/ui'
  import { onClickOutside } from '@vueuse/core'

  const props = defineProps<{
    options: DropdownMenuOption[]
    disabled?: boolean
  }>()

  const isMenuOpened = ref<boolean>(false)
  const dropdownRef = useTemplateRef<HTMLElement | null>('dropdownRef')

  onClickOutside(dropdownRef, () => {
    isMenuOpened.value = false
  })

  function toggleActionMenu(): void {
    if (props.disabled) return
    isMenuOpened.value = !isMenuOpened.value
  }

  function onSelectOption(option: DropdownMenuOption): void {
    isMenuOpened.value = false
    option.action?.()
  }

  function onLinkOptionSelect(option: DropdownMenuOption): void {
    option.action?.()
    setTimeout(() => {
      isMenuOpened.value = false
    }, 0)
  }
</script>

<style scoped>

</style>
