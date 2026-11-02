<template>
  <div class="mb-8 max-w-100 text-center">
    <IconLogo
      class="
        mx-auto mb-4 justify-center
        [&>svg]:h-27.75 [&>svg]:w-23.75
      "
      :aria-label="$t('a11y.brandLogo', { brand: appName })"
      role="img"
    />

    <p
      v-if="header"
      class="mb-2 text-4xl text-neutral-20"
    >
      {{ header }}
    </p>

    <p
      v-if="subHeader"
      class="mb-2 text-lg font-bold text-neutral-20"
    >
      {{ subHeader }}
    </p>

    <p
      v-if="description || $slots.description"
      class="text-content-secondary"
    >
      <slot name="description">
        {{ description }}
      </slot>
    </p>
  </div>
</template>

<script setup lang="ts">
  import { useApp } from '@/composables/useApp'

  defineProps<{
    header?: string,
    subHeader?: string,
    description?: string,
  }>()

  const { appName } = useApp()
</script>

<style scoped>

</style>

