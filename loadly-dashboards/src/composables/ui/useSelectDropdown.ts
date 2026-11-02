import type { SelectOption } from '@/types/ui'
import { onClickOutside } from '@vueuse/core'
import { createScrollToEndHandler } from './useScrollToEnd'

export interface UseSelectDropdownOptions {
  options: Ref<SelectOption[]>
  searchable: Ref<boolean>
  disabled?: Ref<boolean>
  onScrollToEnd?: Ref<(() => void) | undefined>
  onSearch?: Ref<((searchKeyword: string) => void) | undefined>
}

export function useSelectDropdown(selectOptions: UseSelectDropdownOptions) {
  const { options, searchable, disabled, onScrollToEnd, onSearch } = selectOptions

  const isOpen = ref(false)
  const openUpward = ref(false)
  const searchKeyword = ref('')
  const selectRef = useTemplateRef<HTMLElement>('selectRef')
  const dropdownRef = useTemplateRef<HTMLElement>('dropdownRef')
  const optionsListRef = useTemplateRef<HTMLUListElement>('optionsListRef')

  const filteredOptions = computed(() => {
    if (onSearch?.value) return options.value

    const seen = new Set<string>()
    const query = searchable.value && searchKeyword.value.trim()
      ? searchKeyword.value.toString().toLowerCase().trim()
      : ''

    return options.value.filter((option) => {
      if (seen.has(option.value)) return false

      seen.add(option.value)
      if (!query) return true

      const text = (option.searchableText ?? option.label)?.toString().toLowerCase().trim()

      return text === query
        || text?.startsWith(query)
        || text?.split(/\s+/).some((word) => word.startsWith(query))
    })
  })

  function checkDropdownPosition(): void {
    if (!selectRef.value) return

    const rect = selectRef.value.getBoundingClientRect()
    const windowHeight = window.innerHeight
    const spaceBelow = windowHeight - rect.bottom
    const dropdownHeight = options.value.length * 44 + 16

    openUpward.value = spaceBelow < dropdownHeight && rect.top > dropdownHeight
  }

  function resetSearch(): void {
    if (searchable.value) {
      searchKeyword.value = ''
    }
  }

  function toggleDropdown(): void {
    if (disabled?.value) return

    if (!isOpen.value) {
      checkDropdownPosition()
    }

    resetSearch()
    isOpen.value = !isOpen.value
  }

  function closeDropdown(): void {
    isOpen.value = false
  }

  const onScroll = createScrollToEndHandler(() => onScrollToEnd?.value?.())

  onClickOutside(selectRef, () => {
    isOpen.value = false
  })

  return {
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
  }
}
