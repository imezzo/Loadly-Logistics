<template>
  <section class="mt-6">
    <CardPreview :title="$t('content.contactPersonHeading')">
      <div
        class="
          flex flex-wrap items-baseline gap-x-10 gap-y-4 rounded-[0.625rem] border border-gray-50
          bg-neutral-30 p-3
          sm:gap-x-12 sm:p-4
        "
      >
        <CardPreviewLabel
          :label="$t('form.contactPersonName')"
          :content="contactPersonName"
        />

        <CardPreviewLabel
          :label="$t('content.mobileNumber')"
          :content="contactPersonMobile"
        />
      </div>
    </CardPreview>
  </section>
</template>

<script setup lang="ts">
  import CardPreview from '@/components/card/CardPreview.vue'
  import { useTripStore } from '@/stores/admin-dashboard/trip'

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const contactPersonName = computed(() => trip.value.contactPersonName?.trim() || '-')
  const contactPersonMobile = computed(() => trip.value.contactPersonMobile?.trim() || '-')
</script>

<style scoped>

</style>
