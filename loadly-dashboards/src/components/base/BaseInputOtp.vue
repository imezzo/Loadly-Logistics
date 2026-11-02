<template>
  <div class="flex gap-3">
    <input
      v-for="(_, index) in digits"
      :key="index"
      ref="inputs"
      v-model="digits[index]"
      :type="type"
      :inputmode="type === 'number' ? 'numeric' : 'text'"
      maxlength="1"
      :disabled="($attrs as {disabled?: boolean})?.disabled ?? false"
      class="
        size-13.5 rounded-xl border border-gray-50 text-center text-lg font-bold text-neutral-20
        transition-all outline-none
        focus:border-primary focus:ring-1 focus:ring-primary
        disabled:bg-gray-50
      "
      @input="onInput($event, index)"
      @keydown.backspace="onBackspace(index)"
      @keydown.left="onArrowLeft(index)"
      @keydown.right="onArrowRight(index)"
      @paste="onPaste"
    >
  </div>
</template>

<script setup lang="ts">
  const props = withDefaults(defineProps<{
    modelValue?: string
    length?: number
    type?: 'number' | 'text'
    autoFocus?: boolean
  }>(), {
    modelValue: '',
    length: 6,
    type: 'number',
    autoFocus: true,
  })

  const emit = defineEmits<{
    'update:modelValue': [value: string],
    complete: [value: string]
  }>()

  const digits = ref<string[]>(new Array(props.length).fill(''))
  const inputs = useTemplateRef('inputs')

  watch(() => props.modelValue, (newVal) => {
    const splittedOTP = newVal.split('').slice(0, props.length)
    const remainingDigits = new Array(props.length - splittedOTP.length).fill('')
    digits.value = [...splittedOTP, ...remainingDigits]
  }, { immediate: true })

  watch(digits, (newVal) => {
    const joinedOTP = newVal.join('')
    emit('update:modelValue', joinedOTP)

    if (joinedOTP.length === props.length && !newVal.includes('')) {
      emit('complete', joinedOTP)
    }
  }, { deep: true })

  function focusOnInput(index: number): void {
    inputs.value?.[index]?.focus()
  }

  function fillInput(index: number, value?: string): void {
    digits.value[index] = value || ''
  }

  function onInput(event: Event, index: number): void {
    const input = event.target as HTMLInputElement
    const inputValue = input.value

    if (inputValue && index < props.length - 1) {
      focusOnInput(index + 1)
    }
  }

  function onBackspace(index: number): void {
    if (!digits.value[index] && index > 0) {
      fillInput(index - 1)
      focusOnInput(index - 1)
    }
  }

  function onArrowLeft(index: number): void {
    if (index > 0) focusOnInput(index - 1)
  }

  function onArrowRight(index: number): void {
    if (index < props.length - 1) focusOnInput(index + 1)
  }

  function onPaste(event: ClipboardEvent): void {
    event.preventDefault()
    const pasteData = event.clipboardData?.getData('text') || ''
    const numbers = pasteData.replace(/\D/g, '').split('').slice(0, props.length)

    numbers.forEach((number, index) => fillInput(index, number))

    const nextIndex = Math.min(numbers.length, props.length - 1)
    focusOnInput(nextIndex)
  }

  onMounted(() => {
    if (!props.modelValue && props.autoFocus) {
      focusOnInput(0)
    }
  })
</script>

<style scoped>
  @layer base {
    input[type="number"]::-webkit-outer-spin-button,
    input[type="number"]::-webkit-inner-spin-button {
      -webkit-appearance: none;
      margin: 0;
    }

    input[type="number"] {
      appearance: textfield;
    }
  }
</style>
