<template>
  <article
    class="
      flex flex-col rounded-2xl border border-neutral-500 bg-white p-4 transition-colors
      hover:shadow-xs
    "
  >
    <div class="mb-4 flex">
      <img
        src="@/assets/images/default-avatar/office.webp"
        :alt="company.name"
        class="size-15.5 shrink-0 rounded-md border border-neutral-300 object-contain"
      >

      <div class="ms-4 me-auto w-[60%] gap-2 truncate">
        <h3 class="mb-2 w-max font-bold text-neutral-10">
          {{ company.name }}
        </h3>

        <span class="text-content-secondary">{{ company.id }}</span>

        <span
          v-if="!company.name && !company.id"
          class="text-content-secondary"
        >
          {{ $t('content.noData') }}
        </span>
      </div>

      <RouterLink
        v-if="hasEditPermission"
        :to="{ name: 'UpdateCompany', params: { id: company.id } }"
        class="
          me-2 flex size-8 items-center justify-center rounded-full bg-black/3 transition-colors
          hover:bg-info-500/10
        "
      >
        <IconCog class="size-4" />
      </RouterLink>

      <button
        v-if="hasDeletePermission"
        class="
          flex size-8 items-center justify-center rounded-full bg-black/3 transition-colors
          hover:bg-error-600/10
        "
        @click="isDeletePopupOpen = true"
      >
        <IconTrash class="size-4" />
      </button>
    </div>

    <template v-if="company.contactPerson">
      <div
        v-if="company.website"
        class="flex items-center gap-2"
      >
        <IconClick class="size-4 shrink-0 text-primary" />

        <BaseLink
          :to="company.website"
          rel="noopener noreferrer"
          class="
            truncate text-primary
            hover:underline
          "
          @click.stop
        >
          {{ company.website }}
        </BaseLink>
      </div>

      <a
        :href="`tel:${company.contactPerson.phone}`"
        class="
          flex items-center gap-2 text-primary
          hover:underline
        "
        @click.stop
      >
        <IconGroup class="size-4 shrink-0 text-primary" />

        <span class="truncate">
          {{ $t('content.contactPerson') }} ({{ company.contactPerson.phone }})
        </span>
      </a>

      <div class="mt-4 w-fit rounded-full border border-gray-50 px-3 py-2">
        {{ $t('content.creationDate') }} {{ company.createdAt }}
      </div>
    </template>

    <DeleteCompanyPopup
      v-model="isDeletePopupOpen"
      :company-name="company.name || company.id || ''"
      :is-loading="isDeleting"
      @confirm="onDelete"
    />

    <PopupError
      v-model="isErrorPopupOpen"
      :message="errorMessage"
    />
  </article>
</template>

<script setup lang="ts">
import type { Company } from '@/types/admin-dashboard/response/company'
import { usePermission } from '@/composables/admin-dashboard/usePermission'
import { Permission, Company as CompanyPermission } from '@/types/response/permission-group'
import { useCompanyStore } from '@/stores/admin-dashboard/company'
import { useLoading } from '@/composables/useLoading'
import DeleteCompanyPopup from './DeleteCompanyPopup.vue'
import { useList } from '../composables/useList'

const props = defineProps<{
  company: Company
}>()

const companyStore = useCompanyStore()
const { setLoading } = useLoading()
const { fetchCompanies } = useList()

const isDeletePopupOpen = ref(false)
const isDeleting = ref(false)
const isErrorPopupOpen = ref(false)
const errorMessage = ref<string>('')

const {
  hasPermission: hasEditPermission
} = usePermission(Permission.Company, CompanyPermission.Edit)
const {
  hasPermission: hasDeletePermission
} = usePermission(Permission.Company, CompanyPermission.Delete)

async function onDelete(): Promise<void> {
  if (!props.company.id) return

  isDeleting.value = true
  setLoading(true)

  const { success, message } = await companyStore.deleteCompany(props.company.id)

  isDeleting.value = false
  setLoading(false)

  if (success) {
    isDeletePopupOpen.value = false
    fetchCompanies()

    return
  }

  isDeletePopupOpen.value = false
  errorMessage.value = message
  isErrorPopupOpen.value = true
}
</script>

<style scoped>

</style>
