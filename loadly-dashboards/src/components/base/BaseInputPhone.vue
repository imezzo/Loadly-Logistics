<template>
  <div
    class="
      flex flex-wrap
      xs:flex-nowrap
    "
  >
    <BaseSelect
      v-model="code"
      :options="codes"
      wrapper-class="select-button-phone w-full xs:w-50"
      :disabled="$attrs.disabled"
      searchable
    />

    <BaseInput
      :id="fieldId"
      v-model="phone"
      type="tel"
      :placeholder="$t('form.phonePlaceholder')"
      container-classes="h-full py-1.5 xs:ps-3 pe-2.5 xs:rounded-s-none rounded-lg"
      wrapper-class="flex-1 mt-2 xs:mt-0"
      :disabled="$attrs.disabled"
    />
  </div>

  <p
    v-if="hint"
    :class="hintClass"
  >
    {{ hint }}
  </p>
</template>

<script setup lang="ts">
  import countries from 'country-flags-dial-code'

  interface Country {
    code: string;
    country: string;
    dialCode: string;
    flag: string;
  }
  type PhoneNumber = string | number | null

  const emit = defineEmits<{
    'update:modelValue': [value: PhoneNumber],
  }>()

  const props = withDefaults(defineProps<{
    modelValue?: PhoneNumber
    fieldId: string
    allowedCountryCode?: string
    hint?: string
    hintClass?: string
  }>(), {
    allowedCountryCode: undefined,
    modelValue: '',
    hint: undefined,
    hintClass: '',
  })

  const code = ref('+20')
  const phone = ref('')
  const list = countries.getCountryListMap() as Record<string, Country>

  if (props.allowedCountryCode && !code.value) {
    code.value = props.allowedCountryCode
  }

  parsePhoneNumber()

  function parsePhoneNumber(): void {
    if (!props.modelValue) return

    const fullPhoneNumber = String(props.modelValue).trim()

    if (!fullPhoneNumber || !fullPhoneNumber.startsWith('+')) return

    const matchedCountry = Object.values(list).find((country) =>
      fullPhoneNumber.startsWith(country.dialCode)
    )

    code.value = matchedCountry?.dialCode ?? ''
    phone.value = matchedCountry
      ? fullPhoneNumber.slice(matchedCountry.dialCode.length)
      : ''
  }

  const codes = computed(() => {
    return Object.values(list).map((value: Country) => ({
      value: value.dialCode,
      searchableText: value.country,
      label: `
        <span class="flex items-center gap-1">
          <span>${value.flag}</span>
          <span>${value.dialCode}</span>
        </span>`,
      content: () => `
        <span class="flex items-center w-full">
          <span class="size-4.5 flex items-center shrink-0 bg-contain bg-no-repeat">${value.flag}</span>
          <span class="ms-3">
            ${value.country}
          </span>
        </span>`,
    }))
  })

  const PHONE_NUMBER_PATTERN = /^\+\d{4,}$/

  watch([code, phone], ([newCode, newPhone], [oldCode, oldPhone]) => {
    const hasCodeChanged = newCode !== oldCode
    const hasPhoneChanged = newPhone !== oldPhone

    if (!hasCodeChanged && !hasPhoneChanged) return

    const sanitizedPhone = newPhone.replace(/^0+/, '')
    const fullPhoneNumber = newCode + sanitizedPhone

    const isValidPhoneNumber = PHONE_NUMBER_PATTERN.test(fullPhoneNumber)
    emit('update:modelValue', isValidPhoneNumber ? fullPhoneNumber : '')
  });

  watch(() => props.modelValue, (newValue) => {
    if (newValue) {
      parsePhoneNumber();
    }
  });
</script>

<style scoped>

</style>
