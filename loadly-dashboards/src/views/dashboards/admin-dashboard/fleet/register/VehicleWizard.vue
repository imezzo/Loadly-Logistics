<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ pageTitle }}
    </h1>

    <VehicleForm
      v-if="isDocumentLoaded"
      ref="vehicleFormRef"
      :initial-form-data
      @submit="onSubmit"
    />
  </div>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import VehicleForm from './components/VehicleForm.vue'
  import { useVehicleWizard } from './composables/useVehicleWizard'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { useRouter } from 'vue-router'

  const { t } = useI18n()
  const router = useRouter()
  const { setLoading } = useLoading()
  const { isViewMode, isUpdateMode, vehicleId } = useVehicleWizard()
  const vehicleStore = useVehicleStore()

  const initialFormData = ref<Record<string, unknown>>({})

  const wizardTitle = computed(() => {
    if (isViewMode.value) return t('title.viewVehicle')
    if (isUpdateMode.value) return t('title.updateVehicle')

    return t('title.registerNewVehicle')
  })

  const breadcrumbItems = computed(() => [
    { label: t('title.fleetVehicle'), to: '/fleet' },
    { label: t('title.fleetManagement'), to: '/fleet' },
    { label: wizardTitle.value },
  ])

  const pageTitle = computed(() => wizardTitle.value)

  const vehicleFormRef = useTemplateRef<InstanceType<typeof VehicleForm>>('vehicleFormRef')
  const isDocumentLoaded = ref(false)

  async function loadVehicle() {
    if (!vehicleId.value) return

    setLoading(true)
    const {success, data} = await vehicleStore.getVehicle(vehicleId.value)
    setLoading(false)

    if (success && data) {
      initialFormData.value = data
    }
  }

  function onSubmit() {
    router.push({ name: 'FleetManagement' })
  }

  onMounted(async () => {
    if (vehicleId.value && (isViewMode.value || isUpdateMode.value)) {
      await loadVehicle()
    }
    isDocumentLoaded.value = true
  })
</script>

<style scoped>

</style>
