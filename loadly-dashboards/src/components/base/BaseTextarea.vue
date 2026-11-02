<template>
  <div
    class="input"
    :class="wrapperClass"
  >
    <div
      class="relative flex min-h-24 w-full items-start rounded-xl border border-gray-50 p-3"
      :class="Boolean(attrs.disabled)
        ? 'bg-black/3'
        : 'bg-white'"
    >
      <textarea
        v-model="model"
        v-bind="$attrs"
        :maxlength="maxlength"
        :disabled="Boolean(attrs.disabled)"
        class="min-h-20 w-full resize-none outline-none"
      />

      <div
        v-if="maxlength && showCounter"
        class="absolute end-2 bottom-2 text-xs text-neutral-600"
      >
        {{ (model?.toString() || '').length }}/{{ maxlength }}
      </div>
    </div>

    <p
      v-if="hint"
      class="hint"
    >
      {{ hint }}
    </p>
  </div>
</template>

<script setup lang="ts">
  defineOptions({
    inheritAttrs: false,
  })

  defineProps<{
    modelValue: string | number | null
    wrapperClass?: string
    label?: string
    hint?: string
    maxlength?: number
    showCounter?: boolean
  }>()

  const model = defineModel<string | number | null>()
  const attrs = useAttrs()
</script>

<style scoped>

</style>

