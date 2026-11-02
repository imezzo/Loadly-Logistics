<template>
  <a
    v-if="isExternalLink"
    v-bind="$attrs"
    :href="link.toString()"
    target="_blank"
  >
    <slot />
  </a>

  <RouterLink
    v-else
    v-slot="{ isActive, href, navigate }"
    v-bind="$props"
    :to="link"
    custom
  >
    <a
      v-bind="$attrs"
      :href="href"
      :class="isActive ? activeClass : inactiveClass"
      @click="navigate"
    >
      <slot />
    </a>
  </RouterLink>
</template>

<script setup lang="ts">
import type { RouterLinkProps } from 'vue-router'

defineOptions({
  inheritAttrs: false,
})

const props = defineProps<{
  to: RouterLinkProps['to'],
  inactiveClass?: string,
  activeClass?: string,
}>()

const isExternalLink = computed(() => {
  return typeof props.to === 'string'
    && (props.to.startsWith('http') || /\.([a-z]{2,})$/i.test(props.to))
})

const link = computed(() => {
  if (isExternalLink.value && !props.to.toString().startsWith('http')) {
    return `https://${props.to}`
  }

  return props.to
})
</script>

<style scoped>

</style>
