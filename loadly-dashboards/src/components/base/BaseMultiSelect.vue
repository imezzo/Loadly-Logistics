<!-- eslint-disable vue/no-v-html -->
<template>
  <div
    ref="selectRef"
    class="select"
    :class="wrapperClass"
  >
    <button
      v-bind="$attrs"
      type="button"
      class="select-button"
      :class="{
        'is-open': isOpen,
        'disabled': $attrs.disabled
      }"
      @click="toggleDropdown"
    >
      <span
        class="
          flex max-h-25 min-h-5 flex-1 flex-wrap items-center gap-1.5 overflow-y-auto text-start
        "
      >
        <template v-if="selectedValues.length">
          <span
            v-for="value in selectedValues"
            :key="value"
            class="
              inline-flex items-center gap-1 rounded-md border border-neutral-500 bg-neutral-400
              px-2 py-0.5 text-xs
            "
          >
            <span
              v-if="getOptionContent(value)"
              v-html="getOptionContent(value)"
            />
            <template v-else>
              {{ getOptionLabel(value) }}
            </template>

            <button
              type="button"
              class="
                -me-0.5 rounded-sm p-0.5 transition-colors
                hover:bg-neutral-500
              "
              @click.stop="removeValue(value)"
            >
              <IconCross class="size-3" />
            </button>
          </span>
        </template>

        <span
          v-else
          class="text-neutral-800/60"
        >
          {{ placeholder || $t('form.select') }}
        </span>
      </span>

      <IconChevronDown
        class="shrink-0 transition-transform"
        :class="{ 'rotate-180': isOpen }"
      />
    </button>

    <Transition
      enter-active-class="transition ease-out duration-200"
      :enter-from-class="openUpward
        ? 'opacity-0 -translate-y-1'
        : 'opacity-0 translate-y-1'"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition ease-in duration-150"
      leave-from-class="opacity-100 translate-y-0"
      :leave-to-class="openUpward
        ? 'opacity-0 -translate-y-1'
        : 'opacity-0 translate-y-1'"
    >
      <div
        v-if="isOpen"
        ref="dropdownRef"
        class="select-dropdown"
        :class="{ 'is-upward': openUpward }"
      >
        <ul
          ref="optionsListRef"
          class="select-options"
          @scroll="onScroll"
        >
          <SelectSearch
            v-if="searchable"
            v-model="searchKeyword"
            @search="onSearch?.(searchKeyword)"
          />

          <li
            v-if="!filteredOptions.length"
            class="px-2 py-1 text-sm text-neutral-800/60"
          >
            {{ $t('form.noResultsFound') }}
          </li>

          <SelectOption
            v-for="option in filteredOptions"
            :key="option.value"
            :option="option"
            :is-selected="selectedValues.includes(option.value)"
            is-multiple
            @select="toggleOption"
          />
        </ul>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { useSelectDropdown } from '@/composables/ui/useSelectDropdown'

  const props = withDefaults(defineProps<{
    modelValue?: string[]
    options: SelectOption[]
    placeholder?: string
    wrapperClass?: string
    onScrollToEnd?: () => void
    searchable?: boolean
    onSearch?: (searchKeyword: string) => void
  }>(), {
    modelValue: () => [],
    placeholder: undefined,
    wrapperClass: '',
    onScrollToEnd: undefined,
    searchable: false,
    onSearch: undefined,
  })

  defineOptions({
    inheritAttrs: false,
  })

  const emit = defineEmits<{
    'update:modelValue': [value: string[]]
    scrollToEnd: []
  }>()

  const attrs = useAttrs()

  const {
    isOpen,
    openUpward,
    searchKeyword,
    selectRef,
    dropdownRef,
    optionsListRef,
    filteredOptions,
    toggleDropdown,
    onScroll,
  } = useSelectDropdown({
    options: toRef(() => props.options),
    searchable: toRef(() => props.searchable),
    disabled: computed(() => !!attrs.disabled),
    onScrollToEnd: toRef(() => props.onScrollToEnd),
    onSearch: toRef(() => props.onSearch),
  })

  const selectedValues = computed(() => {
    const value = props.modelValue

    return Array.isArray(value) ? value : []
  })

  function getOptionContent(value: string): string | undefined {
    return props.options.find((option) => option.value === value)?.content?.()
  }

  function getOptionLabel(value: string): string {
    return props.options.find((opt) => opt.value === value)?.label ?? value
  }

  function toggleOption(option: SelectOption): void {
    const current = [...selectedValues.value]
    const index = current.indexOf(option.value)

    if (index >= 0) {
      current.splice(index, 1)
    } else {
      current.push(option.value)
    }

    emit('update:modelValue', current)
  }

  function removeValue(value: string): void {
    const current = selectedValues.value.filter((v) => v !== value)

    emit('update:modelValue', current)
  }
</script>

<style scoped>

</style>
