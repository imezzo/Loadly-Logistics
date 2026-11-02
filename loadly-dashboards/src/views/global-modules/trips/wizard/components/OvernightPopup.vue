<template>
  <PopupError
    v-if="confirmError"
    :model-value="!!confirmError"
    :message="confirmError"
    @update:model-value="confirmError = null"
  />

  <button
    v-if="trip.id"
    type="button"
    class="
      inline-flex items-center gap-2 rounded-full border border-warning-300 bg-warning-50 px-3
      py-1.5 text-sm font-medium text-warning-500/50 transition-colors
      hover:bg-warning-50/80
    "
    @click="openPopup"
  >
    <IconOvernight class="size-4 shrink-0" />
    {{ $t('form.overNightQuestion') }}
  </button>

  <BasePopup
    v-model="showOvernightPopup"
    content-class="max-w-100 [&>div]:border-none [&>div]:p-0"
    :show-close-button="false"
  >
    <div class="w-full px-6 py-5">
      <h2 class="text-xl font-bold text-neutral-10">
        {{ $t('form.overNight') }}
      </h2>

      <label class="mt-5 flex items-center text-sm font-medium text-neutral-10">
        {{ $t('form.costOn') }}
        <span class="ms-1 text-error-600">*</span>
      </label>

      <BaseRadioGroup
        v-model="overnightCost"
        wrapper-class="mt-3"
        :options="costOnOptions"
      />

      <button
        type="button"
        class="btn-primary mt-8 h-12 w-full rounded-2xl"
        :disabled="isLoading || overnightCost === null"
        @click="confirm"
      >
        {{ $t('button.confirm') }}
      </button>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useEnum } from '@/composables/useEnum'
  import { useLoading } from '@/composables/useLoading'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { SelectOption } from '@/types/ui'
  import type { EnumItem } from '@/types/response/enum'

  const PopupError = defineAsyncComponent(() => import('@/components/popup/PopupError.vue'))

  const { t } = useI18n()
  const { enums } = useEnum()
  const { isLoading, setLoading } = useLoading()

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const showOvernightPopup = ref(false)
  const overnightCost = ref<string | number>()
  const confirmError = ref<string | null>(null)

  const costOnOptions = computed<SelectOption[]>(
    () => enums.value?.OverNightCostBearer?.filter((option: EnumItem) => option.value !== '0') ?? []
  )

  function openPopup(): void {
    confirmError.value = null
    showOvernightPopup.value = true
  }

  async function confirm(): Promise<void> {
    if (!trip.value.id || overnightCost.value == null || isLoading.value) return

    confirmError.value = null

    setLoading(true)
    const { success, message } = await tripStore.updateOvernightCostBearer(
      Number(trip.value.id),
      Number(overnightCost.value),
    )
    setLoading(false)

    if (success) {
      showOvernightPopup.value = false
      await tripStore.fetchTrip(Number(trip.value.id))

      return
    }

    confirmError.value = message || t('error.somethingWentWrong')
  }
</script>

<style scoped>

</style>
