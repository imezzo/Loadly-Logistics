<template>
  <section class="rounded-xl border border-neutral-200 bg-white p-6">
    <h2 class="mb-4 text-lg font-semibold text-neutral-800">
      {{ $t('content.pickupPointAndDropoffPoint') }}
    </h2>

    <CardPreview :title="$t('content.tripInformation')">
      <template #content>
        <CardPreviewLabel
          :label="$t('form.companyName')"
          :content="companyName || '—'"
        />
        <CardPreviewLabel
          :label="$t('form.contractRefNumber')"
          :content="contractRefLabel || '—'"
        />
        <CardPreviewLabel
          :label="$t('content.timeTrip')"
          :content="timeTripLabel || '—'"
        />
        <CardPreviewLabel
          :label="$t('form.contactName')"
          :content="trip.contactPersonName || '—'"
        />
        <CardPreviewLabel
          :label="$t('content.phoneNumber')"
          :content="trip.contactPersonMobile || '—'"
        />
        <CardPreviewLabel
          :label="$t('form.specificInstructions')"
          :content="specificInstructions || '—'"
        />
      </template>
    </CardPreview>
  </section>
</template>

<script setup lang="ts">
  import CardPreview from '@/components/card/CardPreview.vue'
  import CardPreviewLabel from '@/components/card/CardPreviewLabel.vue'
  import { useLookups } from '@/composables/useLookups'
  import { useTripStore } from '@/stores/admin-dashboard/trip'

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)
  const { companies, contractReferenceNumbers } = useLookups()

  const companyName = computed(() => {
    const id = trip.value.companyId

    return companies.value?.items?.find((i) => i.value === id)?.label ?? ''
  })

  const contractRefLabel = computed(() => {
    const id = trip.value.contractId

    return contractReferenceNumbers.value?.items?.find((i) => i.value === id)?.label ?? ''
  })

  const timeTripLabel = computed(() => {
    const start = trip.value.startDate
    const end = trip.value.endDate

    if (!start && !end) return ''

    if (start && end) {
      return `${formatDate(start)} - ${formatDate(end)}`
    }

    return formatDate(start || end || '')
  })

  const specificInstructions = computed(() => '')

  function formatDate(value: string): string {
    if (!value) return ''

    const d = new Date(value)

    if (Number.isNaN(d.getTime())) return value

    return new Intl.DateTimeFormat(undefined, {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(d)
  }
</script>

<style scoped>

</style>
