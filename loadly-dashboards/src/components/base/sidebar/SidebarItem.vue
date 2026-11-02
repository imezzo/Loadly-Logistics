<template>
  <li class="mb-1">
    <component
      :is="getComponentType"
      :type="getComponentType === 'button' ? 'button' : undefined"
      :to="getComponentType === BaseLink ? item.href : undefined"
      :class="[
        `
          relative flex w-full items-center border-none px-4.5 py-2.5 text-start text-sm
          text-neutral-10 no-underline transition-all duration-200
        `,
        !item.comingSoon && `
          cursor-pointer
          hover:rounded-full
        `,
        isActive && !item.comingSoon
          ? 'w-auto rounded-full bg-theme px-3 font-bold'
          : !item.comingSoon && `
            font-medium
            hover:bg-theme
          `,
        item.isExpandable && 'justify-between',
        item.comingSoon && 'cursor-default text-xs text-content-secondary',
      ]"
      :aria-expanded="item.isExpandable ? isExpanded : undefined"
      :aria-disabled="item.comingSoon ? true : undefined"
      @click="onClick"
    >
      <component
        :is="iconComponent"
        v-if="iconComponent"
        class="me-3 size-4 shrink-0"
      />

      <span class="flex-1">{{ $t(item.label) }}</span>

      <span
        v-if="item.comingSoon"
        class="rounded-full bg-primary p-1 text-xs font-bold text-white"
      >
        {{ $t('sidebar.soon') }}
      </span>
      <IconChevron
        v-else-if="item.isExpandable"
        class="
          size-4 transition-transform duration-200
          ar:rotate-180
        "
        :class="{ 'rotate-90!': isExpanded }"
      />
    </component>

    <Transition
      enter-active-class="transition-all duration-200"
      enter-from-class="max-h-0 opacity-0"
      enter-to-class="max-h-screen opacity-100"
      leave-active-class="transition-all duration-200"
      leave-from-class="max-h-screen opacity-100"
      leave-to-class="max-h-0 opacity-0"
    >
      <ul
        v-if="item.children && item.children.length > 0 && isExpanded"
        class="m-0 list-none py-2 ps-6 pe-0"
        role="group"
      >
        <SidebarItem
          v-for="child in item.children"
          :key="child.id"
          :item="child"
          :is-active="child.routeName === route.name"
          :is-expanded="isNestedExpanded(child)"
          @toggle="toggleNested(child.id)"
        />
      </ul>
    </Transition>
  </li>
</template>

<script setup lang="ts">
  import type { SidebarItem } from '@/types/sidebarNavigation'
  import type { Component } from 'vue'
  import BaseLink from '@/components/base/BaseLink.vue'

  interface Props {
    item: SidebarItem
    isActive?: boolean
    isExpanded?: boolean
  }

  const props = withDefaults(defineProps<Props>(), {
    isActive: false,
    isExpanded: false,
  })

  const emit = defineEmits<{
    toggle: []
  }>()

  const route = useRoute()
  const { item } = toRefs(props)

  const nestedExpanded = reactive<Record<string, boolean>>({})

  const getComponentType = computed(() => {
    if (item.value.comingSoon) return 'span'
    if (item.value.action || item.value.isExpandable) return 'button'

    return BaseLink
  })

  const iconComponent = computed<Component | null>(() => {
    if (!props.item.icon) return null

    return defineAsyncComponent(() =>
      import(`@/components/icon/${props.item.icon}.vue`),
    )
  })

  function isNestedExpanded(child: SidebarItem): boolean {
    if (!child.isExpandable) return false

    return !!nestedExpanded[child.id]
  }

  function toggleNested(childId: string): void {
    nestedExpanded[childId] = !nestedExpanded[childId]
  }

  function onClick() {
    if (item.value.comingSoon) return
    if (item.value.isExpandable) {
      emit('toggle')
    } else if (item.value.action) {
      item.value.action()
    }
  }
</script>

<style scoped>

</style>
