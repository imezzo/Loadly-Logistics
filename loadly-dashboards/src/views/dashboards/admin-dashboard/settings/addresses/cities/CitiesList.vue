<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <div
      class="
        mb-4 flex flex-col gap-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <h1 class="text-xl font-bold text-neutral-800">
        {{ $t('title.cities') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addCity') }}

        <IconPlus />
      </button>
    </div>

    <CitiesTable
      :cities="cities"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editCity"
      @delete="deleteCity"
    />

    <EditCityPopup
      v-model="isEditPopupOpen"
      :city="selectedCity"
      @refresh="fetchCities"
    />

    <DeleteCityPopup
      v-model="isDeletePopupOpen"
      :city-name="cityToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateCityPopup
      v-model="isCreatePopupOpen"
      @refresh="fetchCities"
    />
  </div>
</template>

<script setup lang="ts">
  import type { City } from '@/types/admin-dashboard/response/city'
  import { Address, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useCityStore } from '@/stores/admin-dashboard/city'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import CitiesTable from './components/CitiesTable.vue'
  import EditCityPopup from './components/EditCityPopup.vue'
  import DeleteCityPopup from './components/DeleteCityPopup.vue'
  import CreateCityPopup from './components/CreateCityPopup.vue'
  import { useLookups } from '@/composables/useLookups'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.Address, Address.CreateCity)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.cities') },
  ]

  const {
    fetchCities,
    cities,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()
  const { getGovernments } = useLookups()

  const cityStore = useCityStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedCity = ref<City | null>(null)
  const cityToDelete = ref<City | null>(null)

  function editCity(city: City) {
    selectedCity.value = city
    isEditPopupOpen.value = true
  }

  function deleteCity(city: City) {
    cityToDelete.value = city
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!cityToDelete.value) return

    setDeleting(true)
    const { success } = await cityStore.deleteCity(cityToDelete.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      cityToDelete.value = null
      fetchCities()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedCity.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      cityToDelete.value = null
    }
  })

  onMounted(() => {
    getGovernments()
    fetchCities()
  })
</script>

<style scoped>

</style>

