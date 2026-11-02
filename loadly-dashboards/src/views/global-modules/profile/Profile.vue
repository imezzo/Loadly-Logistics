<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <template v-if="profile">
      <div
        class="
          flex flex-col gap-8
          sm:flex-row sm:items-start sm:gap-10
        "
      >
        <div class="shrink-0">
          <img
            :src="defaultAvatar"
            :alt="$t('a11y.userAvatar')"
            loading="lazy"
            class="size-28 rounded-full object-cover ring-2 ring-neutral-200"
          >
        </div>

        <div class="min-w-0 flex-1 space-y-6">
          <div
            class="
              grid gap-4
              sm:grid-cols-2
            "
          >
            <ProfileField
              v-for="field in profileFields"
              :key="field.label"
              :label="field.label"
              :text="field.text"
              :verified="field.verified"
            />
          </div>

          <ProfilePermissions :permissions="profile.permissions ?? {}" />
        </div>
      </div>
    </template>

    <p
      v-else
      class="py-8 text-center text-neutral-500"
    >
      {{ $t('content.notAvailable') }}
    </p>
  </div>
</template>

<script setup lang="ts">
  import defaultAvatar from '@/assets/images/default-avatar/user.webp'
  import { useEnum } from '@/composables/useEnum'
  import { MfaChannel } from '@/types/request/mfa'
  import { useProfileStore } from '@/stores/admin-dashboard/profile'
  import ProfileField from './components/ProfileField.vue'
  import ProfilePermissions from './components/ProfilePermissions.vue'

  defineOptions({ name: 'ProfilePage' })

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const profileStore = useProfileStore()
  const { profile } = storeToRefs(profileStore)

  const breadcrumbItems = [{ label: t('title.profile') }]

  const statusLabel = computed(() => {
    if (profile.value?.status == null) return t('content.notAvailable')

    return getDisplayName('UserStatus', Number(profile.value.status))
  })

  const mfaChannelLabel = computed(() => {
    if (profile.value?.mfaChannel == null) return t('content.notAvailable')

    const channel = Number(profile.value.mfaChannel)
    if (channel === MfaChannel.NONE) return t('form.none')
    if (channel === MfaChannel.EMAIL) return t('form.email')
    if (channel === MfaChannel.SMS) return t('form.sms')

    return t('content.notAvailable')
  })

  const profileFields = computed(() => {
    const p = profile.value
    const na = t('content.notAvailable')

    return [
      { label: t('form.fullName'), text: p?.fullName ?? na, verified: false },
      { label: t('form.userName'), text: p?.userName ?? na, verified: false },
      { label: t('content.emailAddress'), text: p?.email ?? na, verified: p?.emailVerified ?? false },
      { label: t('content.phoneNumber'), text: p?.phone ?? na, verified: p?.phoneVerified ?? false },
      { label: t('form.role'), text: p?.userType ?? na, verified: false },
      { label: t('form.mfa'), text: mfaChannelLabel.value, verified: false },
      { label: t('form.statusField'), text: statusLabel.value, verified: false },
    ]
  })
</script>

<style scoped>

</style>
