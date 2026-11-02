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
        v-if="options.find((opt) => opt.value === modelValue?.toString())?.content"
        v-html="options.find((opt) => opt.value === modelValue?.toString())?.content?.()"
      />
      <span
        v-else
        class="w-full truncate text-start"
      >{{ selectedLabel }}</span>

      <IconChevronDown
        v-if="!$attrs.disabled"
        class="transition-transform"
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
            :is-selected="modelValue?.toString() === option.value.toString()"
            @select="selectOption"
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
    modelValue?: string | number
    options: SelectOption[]
    placeholder?: string
    wrapperClass?: string
    onScrollToEnd?: () => void
    onChange?: (value: string | number) => void
    searchable?: boolean
    onSearch?: (searchKeyword: string) => void
  }>(), {
    modelValue: '',
    placeholder: undefined,
    wrapperClass: '',
    onScrollToEnd: undefined,
    onChange: undefined,
    searchable: false,
    onSearch: undefined,
  })

  defineOptions({
    inheritAttrs: false,
  })

  const emit = defineEmits<{
    'update:modelValue': [value: string]
    scrollToEnd: []
  }>()

  const attrs = useAttrs()
  const { t } = useI18n()

  const {
    isOpen,
    openUpward,
    searchKeyword,
    selectRef,
    dropdownRef,
    optionsListRef,
    filteredOptions,
    toggleDropdown,
    closeDropdown,
    onScroll,
  } = useSelectDropdown({
    options: toRef(() => props.options),
    searchable: toRef(() => props.searchable),
    disabled: computed(() => !!attrs.disabled),
    onScrollToEnd: toRef(() => props.onScrollToEnd),
    onSearch: toRef(() => props.onSearch),
  })

  const selectedLabel = computed(() => {
    return props.options.find((option) => option.value === props.modelValue?.toString())?.label
      || props.placeholder || t('form.select')
  })

  function selectOption(option: SelectOption): void {
    emit('update:modelValue', option.value)
    props.onChange?.(option.value as string | number)
    closeDropdown()
  }
</script>

<style scoped>

</style>
