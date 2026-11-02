<template>
  <BasePopup
    v-model="model"
    content-class="max-w-100 [&>div]:border-none [&>div]:p-0"
  >
    <div class="w-full px-6 py-5">
      <h2 class="text-xl font-bold text-neutral-10">
        {{ isEdit ? $t('title.editNote') : $t('title.addNote') }}
      </h2>

      <label class="mt-5 mb-1 flex items-center text-sm font-medium text-neutral-10">
        {{ $t('form.writeHere') }}
        <span class="ms-1 text-error-600">*</span>
      </label>

      <BaseTextarea
        v-model="dispatcherNotes"
        :maxlength="NOTE_MAX_LENGTH"
        show-counter
        wrapper-class="[&_textarea]:min-h-30"
      />

      <BaseValidation
        :model-value="validationError"
        :message="validationError"
      />

      <button
        type="button"
        class="btn-primary mt-8 h-12 w-full rounded-2xl"
        :disabled="isLoading"
        @click="save"
      >
        {{ $t('button.saveNote') }}
      </button>
    </div>
  </BasePopup>

  <PopupError
    v-if="errorMessage"
    :model-value="!!errorMessage"
    :message="errorMessage"
    @update:model-value="errorMessage = null"
  />
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useTripStore } from '@/stores/admin-dashboard/trip'

  const NOTE_MAX_LENGTH = 255

  const props = defineProps<{
    initialNotes?: string | null
  }>()

  const emit = defineEmits<{
    saved: [note: string]
  }>()

  const PopupError = defineAsyncComponent(() => import('@/components/popup/PopupError.vue'))

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const model = defineModel<boolean>('modelValue', { required: true })

  const dispatcherNotes = ref(props.initialNotes?.trim() ?? '')
  const validationError = ref<string | null>(null)
  const errorMessage = ref<string | null>(null)

  const isEdit = computed(() => Boolean(props.initialNotes?.trim()))

  function resetForm(): void {
    dispatcherNotes.value = props.initialNotes?.trim() ?? ''
    validationError.value = null
  }

  function validateNote(note: string): boolean {
    if (!note) {
      validationError.value = t('validation.required', { field: t('form.writeHere') })

      return false
    }

    if (note.length > NOTE_MAX_LENGTH) {
      validationError.value = t('validation.max', { max: NOTE_MAX_LENGTH })

      return false
    }

    validationError.value = null

    return true
  }

  async function save(): Promise<void> {
    const note = dispatcherNotes.value.trim()

    if (!validateNote(note) || !trip.value.id || isLoading.value) return

    errorMessage.value = null
    setLoading(true)

    const { success, message } = await tripStore.addDispatcherNotes(
      Number(trip.value.id),
      note,
    )

    setLoading(false)

    if (success) {
      trip.value.dispatcherNotes = note
      model.value = false
      emit('saved', note)

      return
    }

    errorMessage.value = message || t('error.somethingWentWrong')
  }

  watch(model, (open) => {
    if (open) {
      resetForm()
      errorMessage.value = null

      return
    }

    validationError.value = null
  })
</script>

<style scoped>

</style>
