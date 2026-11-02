<template>
  <div
    v-if="model"
    class="flex items-center gap-3 rounded-lg px-4 py-2"
    :class="alertClasses"
    role="alert"
  >
    <slot name="icon">
      <component
        :is="iconComponent"
        class="size-6 shrink-0"
      />
    </slot>

    <div class="flex-1">
      <p
        v-if="title"
        class="mb-1 font-semibold"
      >
        {{ title }}
      </p>

      <p class="text-sm">
        <slot>{{ message }}</slot>
      </p>
    </div>

    <button
      v-if="dismissible"
      type="button"
      class="
        shrink-0 transition-opacity
        hover:opacity-70
      "
      :aria-label="$t('a11y.close')"
      @click="model = false"
    >
      <IconCross class="size-5" />
    </button>
  </div>
</template>

<script setup lang="ts">
  import type { AlertVariant } from '@/types/ui'
  import IconAlert from '@/components/icon/IconAlert.vue'
  import IconCheckCircle from '@/components/icon/IconCheckCircle.vue'

  const props = defineProps<{
    variant?: AlertVariant
    title?: string
    message?: string
    dismissible?: boolean
  }>()

  const model = defineModel<boolean>('modelValue', { default: true })

  const iconComponent = computed(() => {
    const icons: Record<AlertVariant, Component> = {
      success: IconCheckCircle,
      error: IconAlert,
      warning: IconAlert,
      info: IconAlert,
      default: IconAlert,
    }

    return icons[props.variant ?? 'default']
  })

  const alertClasses = computed(() => {
    const variants: Record<AlertVariant, string> = {
      error: 'bg-error-50 text-neutral-50 text-sm font-medium',
      success: 'bg-success-500 text-white',
      warning: 'bg-warning-10 text-warning-40',
      info: 'bg-info-50 text-neutral-10',
      default: 'bg-error-10 text-error-600',
    }

    return variants[props.variant ?? 'default']
  })
</script>

<style scoped>

</style>
