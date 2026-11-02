import { useDebounceFn } from '@vueuse/core'
import { isValidPhoneNumber, validatePhoneNumberLength } from 'libphonenumber-js'
import { boolean, date, number, object, string } from 'yup'
import { useApp } from '@/composables/useApp'
import { useLookups } from '@/composables/useLookups'
import { useLeadSettingsStore } from '@/stores/admin-dashboard/lead-settings'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import type { FieldsGroup } from '@/types/form'
import { isBeforeToday } from '@/utils/date'
import { createPhoneSchema } from '@/utils/validation/phone'
import { useTripWizard } from './useTripWizard'

function isLeadIdEmpty(leadId: unknown): boolean {
  return leadId == null || String(leadId).trim() === ''
}

export function useTripExternalInformation() {
  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const {
    getLeads,
    hasMoreLeads,
    leads,
    loadMoreLeads,
  } = useLookups()
  const { historicalGroup, isFormDisabled, tripDateInputBounds } = useTripWizard()

  const leadSettingsStore = useLeadSettingsStore()
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const checkLeadPhoneNumber = useDebounceFn(async (value: string) => {
    if (!isValidPhoneNumber(value) || validatePhoneNumberLength(value)) return

    const { success, data } = await leadSettingsStore.getByPhone(value)

    if (!success || !data) return

    trip.value.leadId = data.id.toString()
    trip.value.contactPersonName = data.fullName
    trip.value.contactPersonMobile = data.phone
  }, 500)

  const leadOptions = computed(() => [
    { label: t('form.salesCustomer'), value: '' },
    ...leads.value?.items ?? [],
  ])

  const validationSchema = computed(() => object({
    isHistorical: boolean(),
    leadId: string().optional(),
    contactPersonName: string()
      .when('leadId', {
        is: isLeadIdEmpty,
        then: (schema) => schema.required(t('validation.required', { field: t('form.clientName') })),
        otherwise: (schema) => schema.optional(),
      }),
    contactPersonMobile: string().when('leadId', {
      is: isLeadIdEmpty,
      then: () => createPhoneSchema(t, true),
      otherwise: () => createPhoneSchema(t, false),
    }),
    startDate: string()
      .required(t('validation.required', { field: t('form.startDate') }))
      .when('isHistorical', {
        is: true,
        then: (schema) => schema.test(
          'past-only',
          () => t('validation.maxDate', { max: t('content.today') }),
          (value) => !value || isBeforeToday(value),
        ),
      }),
    endDate: date()
      .required(t('validation.required', { field: t('form.endDate') }))
      .typeError(() => t('validation.invalidDate'))
      .when('isHistorical', {
        is: true,
        then: (schema) => schema.test(
          'past-only',
          () => t('validation.maxDate', { max: t('content.today') }),
          (value) => !value || isBeforeToday(value),
        ),
      })
      .when('startDate', (startDate, schema) => {
        if (startDate && startDate.toString()) {
          return schema.min(
            new Date(startDate.toString()),
            () => t('validation.minDate', { min: t('form.startDate') })
          )
        }

        return schema
      }),
    ...(isAdminDashboard ? {
      spotPrice: number()
        .min(0)
        .required(t('validation.required', { field: t('form.tripPrice') }))
        .typeError(t('validation.integer')),
    } : {}),
    ...(isAdminDashboard ? {
      dispatcherNotes: string()
        .optional()
        .nullable()
        .max(255, t('validation.max', { max: 255 })),
    } : {}),
  }))

  const groups = computed<Record<string, FieldsGroup>>(() => ({
    ...(historicalGroup.value ? { historical: historicalGroup.value } : {}),
    customer: {
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-col flex-wrap gap-4',
      fields: [
        {
          name: 'leadId',
          as: 'BaseSelect',
          label: t('form.searchForCustomer'),
          labelClass: 'mb-1 inline-block',
          placeholder: t('form.salesCustomer'),
          wrapperClass: 'w-full',
          searchable: true,
          options: leadOptions.value,
          disabled: isFormDisabled.value,
          onScrollToEnd: () => {
            if (hasMoreLeads.value) {
              loadMoreLeads()
            }
          },
          onSearch: (searchKeyword: string) => {
            getLeads(1, searchKeyword)
          },
          onChange: (value: string | number) => {
            trip.value.leadId = value?.toString()
            const lead = leads.value?.items.find((lead) => lead.value === value)

            if (!lead) return

            const [name, mobile] = lead.label.split(' - ')
            trip.value.contactPersonName = name?.toString() ?? ''
            trip.value.contactPersonMobile = mobile?.toString() ?? ''
          },
        },
      ],
    },
    addClientData: {
      title: t('form.addClientData'),
      titleClass: 'font-bold',
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-wrap gap-4',
      fields: [
        {
          name: 'contactPersonName',
          as: 'BaseInput',
          type: 'text',
          label: t('form.clientName'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          disabled: !!trip.value.leadId || isFormDisabled.value,
        },
        {
          name: 'contactPersonMobile',
          as: 'BaseInputPhone',
          label: t('form.phoneNumber'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          disabled: !!trip.value.leadId || isFormDisabled.value,
          'onUpdate:modelValue': (value: string) => {
            if (trip.value.leadId) return

            checkLeadPhoneNumber(value)
          },
        },
      ],
    },
    tripTime: {
      title: t('form.tripTime'),
      titleClass: 'font-bold',
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-wrap gap-4',
      fields: [
        {
          name: 'startDate',
          as: 'BaseInput',
          type: 'datetime-local',
          label: t('form.startDate'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          min: tripDateInputBounds.value.min,
          max: tripDateInputBounds.value.max,
          disabled: isFormDisabled.value,
        },
        {
          name: 'endDate',
          as: 'BaseInput',
          type: 'datetime-local',
          label: t('form.endDate'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          min: tripDateInputBounds.value.min,
          max: tripDateInputBounds.value.max,
          disabled: isFormDisabled.value,
        },
      ],
    },
    ...(isAdminDashboard ? {
      tripPrice: {
        class: 'mb-6',
        wrapperClass: 'mt-4 flex flex-col flex-wrap gap-4',
        fields: [
          {
            name: 'spotPrice',
            as: 'BaseInput',
            type: 'number',
            min: 0,
            label: t('form.tripPrice'),
            labelClass: 'mb-1 inline-block',
            labelRequired: true,
            suffix: t('form.egp'),
            wrapperClass: 'w-full',
            disabled: isFormDisabled.value,
          },
          {
            name: 'dispatcherNotes',
            as: 'BaseTextarea',
            label: t('form.note'),
            labelClass: 'mb-1 inline-block',
            wrapperClass: 'w-full',
            maxlength: 255,
            showCounter: true,
            disabled: isFormDisabled.value,
          },
        ],
      },
    } : {}),
  }))

  const formInitialValues = computed(() => ({
    contactPersonMobile: trip.value.contactPersonMobile,
    contactPersonName: trip.value.contactPersonName,
    endDate: trip.value.endDate,
    leadId: trip.value.leadId,
    spotPrice: trip.value.spotPrice,
    dispatcherNotes: trip.value.dispatcherNotes,
    isHistorical: trip.value.isHistorical,
    startDate: trip.value.startDate,
  }))

  return {
    formInitialValues,
    groups,
    validationSchema,
  }
}
