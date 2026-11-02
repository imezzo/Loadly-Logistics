<template>
  <div
    ref="comboboxRef"
    class="combobox"
    :class="wrapperClass"
  >
    <div
      class="combobox-input-wrapper"
      :class="{ 'is-open': isOpen }"
    >
      <IconSearch class="size-5 shrink-0 text-neutral-800 opacity-40" />

      <input
        v-model="searchQuery"
        type="text"
        :placeholder="placeholder || $t('form.search')"
        class="combobox-input"
        @focus="onFocus"
        @input="onInput"
      >

      <button
        v-if="searchQuery"
        type="button"
        class="combobox-clear"
        @click="clearSearch"
      >
        <IconCross />
      </button>
    </div>

    <Transition
      enter-active-class="transition ease-out duration-200"
      enter-from-class="opacity-0 translate-y-1"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition ease-in duration-150"
      leave-from-class="opacity-100 translate-y-0"
      leave-to-class="opacity-0 translate-y-1"
    >
      <ul
        v-if="isOpen && filteredItems.length"
        class="combobox-dropdown"
      >
        <li
          v-for="(item, index) in filteredItems"
          :key="getItemKey(item, index)"
          class="combobox-option"
          @click="selectItem(item)"
        >
          <slot
            name="item"
            :item="item"
          >
            {{ getItemLabel(item) }}
          </slot>
        </li>
      </ul>

      <div
        v-else-if="isOpen && searchQuery && !filteredItems.length"
        class="combobox-no-results"
      >
        <slot name="no-results">
          <p class="text-sm text-neutral-800/60">
            {{ noResultsText || $t('form.noResultsFound') }}
          </p>
        </slot>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts" generic="T extends Record<string, any>">
  import { onClickOutside } from '@vueuse/core'

  const props = withDefaults(defineProps<{
    filterKeys?: string[]
    itemKey?: string
    itemLabel?: string
    items: T[]
    maxResults?: number
    noResultsText?: string
    placeholder?: string
    wrapperClass?: string
  }>(), {
    filterKeys: () => [],
    itemKey: 'id',
    itemLabel: 'name',
    maxResults: 10,
    noResultsText: '',
    placeholder: '',
    wrapperClass: '',
  })

  const emit = defineEmits<{
    select: [item: T]
    search: [query: string]
  }>()

  const searchQuery = defineModel<string>({ default: '' })
  const comboboxRef = useTemplateRef<HTMLElement>('comboboxRef')
  const isOpen = ref(false)

  const filteredItems = computed(() => {
    if (!searchQuery.value.trim()) return []

    const query = searchQuery.value.toLowerCase()
    const keysToSearch = props.filterKeys.length
      ? props.filterKeys
      : [props.itemLabel]

    return props.items
      .filter((item) =>
        keysToSearch.some((key) => {
          const value = item[key]

          return value && String(value).toLowerCase().includes(query)
        }),
      )
      .slice(0, props.maxResults)
  })

  function toggleDropdown(value: boolean): void {
    isOpen.value = value
  }

  function onFocus(): void {
    toggleDropdown(true)
  }

  function onInput(): void {
    toggleDropdown(true)
    emit('search', searchQuery.value)
  }

  function selectItem(item: T): void {
    searchQuery.value = getItemLabel(item)
    toggleDropdown(false)
    emit('select', item)
  }

  function clearSearch(): void {
    searchQuery.value = ''
    toggleDropdown(false)
    emit('search', '')
  }

  function getItemKey(item: T, index: number): string | number {
    return item[props.itemKey] ?? index
  }

  function getItemLabel(item: T): string {
    return String(item[props.itemLabel] ?? '')
  }

  onClickOutside(comboboxRef, () => {
    toggleDropdown(false)
  })

  watch(() => searchQuery.value, (newValue) => {
    if (!newValue) toggleDropdown(false)
  })
</script>

<style scoped>

</style>

