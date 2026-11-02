<template>
  <div
    v-if="companies.length"
    class="
      grid grid-cols-1 gap-4
      lg:grid-cols-2
    "
  >
    <CompanyCard
      v-for="company in companies"
      :key="company.id"
      :company="company"
    />
  </div>

  <p
    v-else
    class="py-12 text-center text-content-secondary"
  >
    {{ $t('content.noCompaniesFound') }}
  </p>

  <BasePagination
    :pagination="pagination"
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />
</template>

<script setup lang="ts">
  import type { Company } from '@/types/admin-dashboard/response/company'
  import type { Pagination } from '@/types/response/pagination'
  import CompanyCard from './CompanyCard.vue'
  import { useList } from '../composables/useList'

  defineProps<{
    companies: Company[]
    pagination: Pagination
  }>()

  const {
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = useList()
</script>

<style scoped>

</style>

