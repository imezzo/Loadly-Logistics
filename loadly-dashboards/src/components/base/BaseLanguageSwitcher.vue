<template>
  <button
    type="button"
    class="
      flex items-center gap-1 rounded-full bg-neutral-30 p-3 font-bold text-neutral-800
      transition-colors
      hover:text-primary
      en:font-cairo
    "
    :aria-label="otherLocaleLabel"
    @click="setLocale(otherLocale)"
  >
    {{ otherLocaleLabel }}
    <IconGlobe class="size-4" />
  </button>
</template>

<script setup lang="ts">
  const { locale } = useI18n()
  const router = useRouter()

  const localeOptions: { value: string; label: string }[] = [
    { value: 'en', label: 'English' },
    { value: 'ar', label: 'العربية' },
  ]

  const otherLocale = computed(() =>
    locale.value === 'ar' ? 'en' : 'ar')

  const otherLocaleLabel = computed(() =>
    localeOptions.find(opt => opt.value === otherLocale.value)?.label ?? otherLocale.value)

  function setLocale(value: string): void {
    locale.value = value
    document.documentElement.lang = value
    localStorage.setItem('locale', value)
  }

  watch(locale, () => {
    router.go(0)
  })
</script>

<style scoped>

</style>
