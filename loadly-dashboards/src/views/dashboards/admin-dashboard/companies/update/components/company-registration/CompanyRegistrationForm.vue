<template>
  <BaseForm
    ref="companyForm"
    :initial-values="initialValues"
    :validation-schema="validationSchema"
    :groups="groups"
    :errors="props.errors"
    @submit="onSubmit"
  >
    <template #field-phone>
      <div class="mt-6 w-full">
        <SecondaryContactsHeader @add-contact="isAddContactPopupOpen = true" />

        <SecondaryContactsList
          :contacts="secondaryContacts"
          @remove="removeSecondaryContact"
        />
      </div>
    </template>

    <template #field-gpsCoordinates>
      <div class="mt-6 w-full">
        <MultipleLocationsHeader @add-location="isAddLocationPopupOpen = true" />

        <MultipleLocationsList
          :locations="multipleLocations"
          @remove="removeLocation"
        />
      </div>
    </template>

    <AddSecondaryContactPopup
      v-model="isAddContactPopupOpen"
      @add="addSecondaryContact"
    />

    <AddMultipleLocationPopup
      v-model="isAddLocationPopupOpen"
      @add="addLocation"
    />

    <template #submit>
      <BaseActions
        :next-button-options="{ asSubmit: true, hasScrollTop: true }"
        @prev="emit('prev')"
        @next="emit('next')"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useLookups } from '@/composables/useLookups'
  import { useCompanyRegistration } from '../../composables/useCompanyRegistration'
  import AddMultipleLocationPopup from './AddMultipleLocationPopup.vue'
  import MultipleLocationsHeader from './MultipleLocationsHeader.vue'
  import MultipleLocationsList from './MultipleLocationsList.vue'
  import type { CompanyAddressFormData } from '@/types/admin-dashboard/response/company-detail'
  import type { SecondaryContact } from '@/types/secondary-contact.types.ts'

  const AddSecondaryContactPopup = defineAsyncComponent(() => import('@/components/secondary-contact/AddSecondaryContactPopup.vue'))

  interface CompanyForm {
    form: {
      setFieldValue: (field: string, value: unknown) => void
      values: Record<string, unknown>
    }
  }

  interface MultipleLocation {
    id?: number
    city: string
    governorate: string
    district: string
    street?: string
    postalCode?: string
    gpsCoordinates?: string
  }

  useVeeValidate()

  const props = defineProps<{
    formValues?: Record<string, unknown>
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
    next: []
  }>()

  const { getCities } = useLookups()
  const {
    initialValues,
    validationSchema,
    groups,
    fetchDistricts,
  } = useCompanyRegistration(computed(() => props.formValues))

  const companyFormRef = useTemplateRef<CompanyForm>('companyForm')
  const companyFormValues = computed<Record<string, unknown> | undefined>(() => companyFormRef.value?.form?.values)

  const isAddContactPopupOpen = ref(false)
  const isAddLocationPopupOpen = ref(false)
  const secondaryContacts = ref<SecondaryContact[]>([])
  const multipleLocations = ref<MultipleLocation[]>([])

  onMounted(() => {
    fetchDistricts()
  })

  watch(() => companyFormValues.value?.governorate as string | undefined, (newValue, oldValue) => {
    if (newValue
      && newValue !== oldValue
      && oldValue !== undefined
      && (oldValue !== '')
    ) {
      onGovernorateChange(newValue)
    }
  })

  watch(() => companyFormValues.value?.city as string | undefined, async (newValue) => {
    if (newValue) {
      await fetchDistricts(Number(newValue))
    }
  })

  watch(() => props.formValues?.city as string | undefined, async (cityId) => {
    if (cityId) {
      await fetchDistricts(Number(cityId))
    }
  }, { immediate: true })

  watch(() => props.formValues?.secondaryContacts, (contacts) => {
    if (Array.isArray(contacts)) {
      secondaryContacts.value = contacts.map((contact: SecondaryContact) => ({
        id: contact.id,
        name: contact.name || '',
        phone: contact.phone || '',
        email: contact.email || '',
        position: contact.position || '',
        communicationMethod: contact.communicationMethod || '',
      }))
    }
  }, { immediate: true, deep: true })

  watch(() => props.formValues?.multipleLocations, (locations) => {
    if (Array.isArray(locations)) {
      if (locations.length > 0) {
        multipleLocations.value = locations.map((loc: MultipleLocation) => ({
          id: loc.id,
          city: loc.city || '',
          governorate: loc.governorate || '',
          district: loc.district || '',
          street: loc.street || '',
          postalCode: loc.postalCode || '',
          gpsCoordinates: loc.gpsCoordinates || '',
        }))
      } else {
        multipleLocations.value = []
      }
    }
  }, { immediate: true, deep: true })

  watch(() => props.formValues?.addresses, (addresses) => {
    if (!props.formValues?.multipleLocations
      && Array.isArray(addresses)
      && addresses.length) {
      const secondaryAddresses = addresses.slice(1)
      multipleLocations.value = secondaryAddresses.map((addr: CompanyAddressFormData) => ({
        id: addr.id,
        city: addr.city || '',
        governorate: addr.governorate || '',
        district: addr.district || '',
        street: addr.streetName || '',
        postalCode: addr.postalCode || '',
        gpsCoordinates: '',
      }))
    }
  }, { immediate: true, deep: true })

  function addSecondaryContact(contact: SecondaryContact): void {
    secondaryContacts.value.push(contact)
  }

  function removeSecondaryContact(index: number): void {
    secondaryContacts.value.splice(index, 1)
  }

  function addLocation(location: MultipleLocation): void {
    multipleLocations.value.push(location)
  }

  function removeLocation(index: number): void {
    multipleLocations.value.splice(index, 1)
  }

  function onSubmit(values: Record<string, unknown>): void {
    emit('update', {
      ...values,
      secondaryContacts: secondaryContacts.value,
      multipleLocations: multipleLocations.value,
    })
    emit('next')
  }

  function setFieldValue(field: string, value: unknown): void {
    companyFormRef.value?.form?.setFieldValue(field, value)
  }

  async function onGovernorateChange(value: string) {
    if (!value) return

    const id = parseInt(value, 10)
    await getCities(id)

    setFieldValue('city', '')
    setFieldValue('district', '')
  }
</script>

<style scoped>

</style>

