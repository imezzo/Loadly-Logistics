<template>
  <LazyPopupSuccess
    v-if="isSuccess"
    :model-value="isSuccess"
    @update:model-value="onSuccess"
  >
    <template #message>
      <p class="text-center text-2xl font-bold text-neutral-10">
        {{ isEditMode ? $t('success.vehicleLocationUpdated') : $t('success.vehicleLocationCreated') }}
      </p>
    </template>
  </LazyPopupSuccess>

  <BaseAlert
    v-if="errorMessage"
    :model-value="!!errorMessage"
    :message="errorMessage"
    variant="error"
    class="mb-4 w-full"
  />

  <BaseForm
    ref="formRef"
    :validation-schema
    :initial-values
    :groups="fieldsGroup"
    :errors="formErrors"
    @submit="submit"
  >
    <template #field-after-communicationMethod>
      <div class="col-span-full mt-6 w-full">
        <SecondaryContactsHeader
          :is-readonly="isViewMode"
          @add-contact="isAddContactPopupOpen = true"
        />

        <SecondaryContactsList
          :contacts="secondaryContacts"
          :is-readonly="isViewMode"
          @remove="removeSecondaryContact"
        />
      </div>
    </template>

    <template #submit>
      <BaseActions
        container-class="flex items-center justify-end gap-3 sm:gap-6"
        :prev-button-options
        :next-button-options="isViewMode ? undefined : nextButtonOptions"
        @prev="router.push({ name: 'VehicleFacilitiesLocationsList' })"
      />
    </template>
  </BaseForm>

  <AddSecondaryContactPopup
    v-if="!isViewMode"
    v-model="isAddContactPopupOpen"
    @add="addSecondaryContact"
  />
</template>

<script setup lang="ts">
  import { useDebounceFn } from '@vueuse/core'
  import type { SecondaryContact } from '@/types/secondary-contact.types'
  import { useMapsStore } from '@/stores/admin-dashboard/maps'
  import { useVehicleFacilityForm } from '../composables/useVehicleFacilityForm'

  const AddSecondaryContactPopup = defineAsyncComponent(
    () => import('@/components/secondary-contact/AddSecondaryContactPopup.vue'))
  const LazyPopupSuccess = defineAsyncComponent(
    () => import('@/components/popup/PopupSuccess.vue'))

  const { t } = useI18n()
  const router = useRouter()

  const mapsStore = useMapsStore()

  const isAddContactPopupOpen = ref(false)

  const formRef = useTemplateRef<{
    form?: {
      getValues: () => Record<string, unknown>
      setValues: (v: Record<string, unknown>, validate?: boolean) => void
    }
  }>('formRef')

  async function extractFromMapsUrl(url: string): Promise<void> {
    const res = await mapsStore.extractPlace(url)
    const form = formRef.value?.form
    if (!res.success || !res.data || !form) return

    const current = form.getValues()
    form.setValues({
      ...current,
      ...(res.data.address ? { address: res.data.address } : {}),
    }, false)
    coordinates.value = {
      lat: String(res.data.lat),
      lng: String(res.data.lng),
    }
  }

  const debouncedExtract = useDebounceFn(extractFromMapsUrl, 450)

  const {
    coordinates,
    errorMessage,
    fieldsGroup,
    formErrors,
    initialValues,
    isEditMode,
    isLoading,
    isSuccess,
    isViewMode,
    onSubmit,
    onSuccess,
    secondaryContacts,
    validationSchema,
  } = useVehicleFacilityForm(debouncedExtract)

  const prevButtonOptions = computed(() => ({
    label: isViewMode.value ? t('button.back') : t('button.discard'),
    class: 'flex h-13.5 w-1/2 items-center justify-center gap-2 rounded-2xl bg-black/15'
      + ' px-4 py-4.25 font-medium text-content-secondary transition-colors hover:bg-black/20 sm:w-52.5',
  }))

  const nextButtonOptions = computed(() => ({
    asSubmit: true,
    class: 'btn-primary h-13.5 w-1/2 py-3.75 sm:w-75',
    disabled: isLoading.value,
    label: isEditMode.value ? t('button.saveChanges') : t('button.save'),
  }))

  function addSecondaryContact(contact: SecondaryContact): void {
    secondaryContacts.value.push(contact)
  }

  function removeSecondaryContact(index: number): void {
    secondaryContacts.value.splice(index, 1)
  }

  async function submit(values: Record<string, unknown>): Promise<void> {
    if (isViewMode.value) return

    await onSubmit({
      ...values,
      secondaryContacts: secondaryContacts.value,
    })
  }
</script>

<style scoped>

</style>
