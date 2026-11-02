import type { FieldsGroup } from '@/types/form'
import { object, string } from 'yup'
import { useEnum } from '@/composables/useEnum'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { TripType, waypointType } from '@/types/admin-dashboard/response/trip/trip'
import { useTripWizard } from './useTripWizard'

export function useTripPointForm(setPointAddress?: (url: string) => void | Promise<void>) {
  const { t } = useI18n()
  const { isFormDisabledWithCompletedStatus } = useTripWizard()
  const { enums } = useEnum()
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const stopTypeOptions = computed(() => {
    const options = enums.value?.TripWaypointStopType ?? []

    if (Number(trip.value.type) !== TripType.OneWay) return options

    return options.filter(option => Number(option.value) !== Number(waypointType.depot))
  })

  const initialValues = ref({
    stopType: '',
    googleMapLink: '',
    latitude: '',
    longitude: '',
    addressName: '',
    taskDetails: '',
  })

  const validationSchema = computed(() => object({
    stopType: string().required(),
    googleMapLink: string().required(),
    latitude: string().optional(),
    longitude: string().optional(),
    addressName: string().required(),
    taskDetails: string().optional(),
  }))

  const groups = computed<Record<string, FieldsGroup>>(() => ({
    pointTypeAndLink: {
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-col gap-4',
      fields: [
        {
          name: 'stopType',
          as: 'BaseRadioGroup',
          label: t('form.chooseThePoint'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          options: stopTypeOptions.value,
          wrapperClass: 'flex flex-row gap-6',
          disabled: isFormDisabledWithCompletedStatus.value,
        },
        {
          name: 'googleMapLink',
          as: 'BaseInput',
          type: 'text',
          label: t('form.googleMapLink'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          wrapperClass: 'w-full',
          placeholder: t('form.address'),
          disabled: isFormDisabledWithCompletedStatus.value,
          onInput: (e: InputEvent) => {
            const url = (e.target as HTMLInputElement)?.value?.trim?.()
            if (url) setPointAddress?.(url)
          },
        },
      ],
    },
    latLong: {
      title: t('form.latAndLong'),
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-wrap gap-4',
      fields: [
        {
          name: 'latitude',
          as: 'BaseInput',
          type: 'text',
          label: t('form.lat'),
          labelClass: 'mb-1 inline-block',
          containerClass: 'w-full md:w-[calc(50%-0.5rem)]',
          wrapperClass: 'w-full',
          disabled: true,
        },
        {
          name: 'longitude',
          as: 'BaseInput',
          type: 'text',
          label: t('form.long'),
          labelClass: 'mb-1 inline-block',
          containerClass: 'w-full md:w-[calc(50%-0.5rem)]',
          wrapperClass: 'w-full',
          disabled: true,
        },
      ],
    },
    addressAndNote: {
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-col gap-4',
      fields: [
        {
          name: 'addressName',
          as: 'BaseTextarea',
          label: t('form.address'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          wrapperClass: 'w-full',
          disabled: isFormDisabledWithCompletedStatus.value,
        },
        {
          name: 'taskDetails',
          as: 'BaseTextarea',
          label: t('form.addNoteInPickups'),
          labelClass: 'mb-1 inline-block',
          wrapperClass: 'w-full',
          disabled: isFormDisabledWithCompletedStatus.value,
        },
      ],
    },
  }))

  return {
    initialValues,
    groups,
    validationSchema,
  }
}
