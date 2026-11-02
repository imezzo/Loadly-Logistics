<template>
  <div
    class="input"
    :class="wrapperClass"
  >
    <div
      class="input-container"
      :class="[containerClasses, { 'disabled': attrs.disabled }]"
    >
      <slot name="left-icon" />

      <input
        v-if="!attrs.disabled"
        v-bind="$attrs"
        :value="inputModel"
        :type="inputType"
        :inputmode="inputMode"
        @change="onChange?.(model ?? null)"
        @input="setSeparatedNumber"
      >

      <span v-else>
        {{ model }}
      </span>

      <button
        v-if="attrs.type === 'password'"
        :aria-label="$t('a11y.togglePasswordVisibility')"
        type="button"
        class="flex size-5! w-6! cursor-pointer items-center justify-center pt-0.75"
        @click="showPassword = !showPassword"
      >
        <slot name="right-icon">
          <IconEye
            :visible="!showPassword"
          />
        </slot>
      </button>

      <span
        v-else-if="suffix"
        class="px-2 font-medium whitespace-nowrap text-neutral-600"
      >
        {{ suffix }}
      </span>

      <slot
        v-else
        name="right-icon"
      />
    </div>

    <p
      v-if="hint"
      :class="hintClass || 'hint'"
    >
      {{ hint }}
    </p>
  </div>
</template>

<script setup lang="ts">
import type { InputHTMLAttributes } from 'vue'

  type BaseInputValue = string | number | null

  defineOptions({
    inheritAttrs: false,
  })

  const props = defineProps<{
    commaSeparated?: boolean
    containerClasses?: string
    hint?: string
    hintClass?: string
    label?: string
    modelValue: BaseInputValue
    onChange?: (value: BaseInputValue) => void
    suffix?: string | null
    wrapperClass?: string
  }>()

  const model = defineModel<BaseInputValue>()
  const attrs = useAttrs()

  const showPassword = ref(false)

  const inputModel = computed(() => {
    return props.commaSeparated ? model.value?.toLocaleString() : model.value
  })

  const inputType = computed(() => {
    if (attrs?.type === 'password') {
      return showPassword.value ? 'text' : 'password'
    }

    if (props.commaSeparated) return 'text'

    return (attrs.type as InputHTMLAttributes['type']) || 'text'
  })

  const inputMode = computed(() => {
    if (props.commaSeparated) return 'decimal'

    return attrs.inputmode as InputHTMLAttributes['inputmode']
  })

  function setSeparatedNumber(event: Event): void {
    const value = (event.target as HTMLInputElement).value

    model.value = props.commaSeparated
      ? Number(value.replace(/,/g, ''))
      : value

    const onInput = attrs.onInput
    if (typeof onInput === 'function') onInput(event)
  }
</script>

<style scoped>

</style>
