<template>
  <div class="flex w-full max-w-108 flex-col">
    <FormHeader :sub-header="$t('form.verificationCode')">
      <template #description>
        <i18n-t
          keypath="form.verificationDescription"
          tag="span"
        >
          <span class="font-bold">{{ $t('form.mfa') }}</span>
        </i18n-t>
      </template>
    </FormHeader>

    <template v-if="isCompanyDashboard">
      <VerificationOption
        :label="$t('form.email')"
        :description="$t('form.emailVerification')"
        :is-active="selectedMethod === MfaChannel.EMAIL"
        @click="selectMethod(MfaChannel.EMAIL)"
      >
        <template #icon>
          <IconEmail class="size-5" />
        </template>
      </VerificationOption>

      <hr class="my-2 border-gray-50">
    </template>

    <VerificationOption
      :label="$t('form.phoneNumber')"
      :description="$t('form.phoneNumberVerification')"
      :is-active="selectedMethod === MfaChannel.SMS"
      @click="selectMethod(MfaChannel.SMS)"
    >
      <template #icon>
        <IconChat class="size-5" />
      </template>
    </VerificationOption>

    <button
      class="
        mt-8 flex h-13.5 w-full items-center justify-center gap-2 rounded-2xl bg-primary leading-6
        font-medium tracking-tight text-white
        hover:bg-primary/90
        disabled:bg-gray-500 disabled:text-black/20
      "
      :disabled="!selectedMethod"
      @click="$emit('continue', selectedMethod)"
    >
      {{ $t('button.continueVerification') }}
      <IconArrow :class="{'fill-black/20!': !selectedMethod}" />
    </button>
  </div>
</template>

<script setup lang="ts">
  import { MfaChannel } from '@/types/request/mfa'
  import FormHeader from '../../../components/FormHeader.vue'
  import VerificationOption from './VerificationOption.vue'
  import { useApp } from '@/composables/useApp.ts'

  type VerificationMethod = MfaChannel | null

  defineEmits<{
    continue: [method: VerificationMethod],
  }>()

  const { isCompanyDashboard } = useApp()

  const selectedMethod = ref<VerificationMethod>(null)

  function selectMethod(method: VerificationMethod) {
    selectedMethod.value = method
  }
</script>

<style scoped>

</style>
