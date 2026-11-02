<template>
  <div class="flex w-full max-w-96 flex-col items-center text-center">
    <FormHeader :sub-header="$t('form.verifyOTP')">
      <template #description>
        <i18n-t
          keypath="form.verificationCodeSentTo"
          tag="p"
          class="mt-2 text-sm text-content-secondary"
        >
          <template #method>
            {{ verificationMethodLabel }}
          </template>
        </i18n-t>

        <span class="font-bold">
          {{ verificationDescription }}
        </span>
      </template>
    </FormHeader>

    <BaseAlert
      v-if="errorMessage"
      :model-value="!!errorMessage"
      :message="errorMessage"
      variant="error"
      class="mb-4"
    />

    <BaseInputOtp
      v-model="otp"
      :length="inputOTPLength"
      class="mb-4 w-full"
    />

    <div class="mb-8 flex w-full justify-between font-medium">
      <button
        type="button"
        class="
          cursor-pointer text-neutral-20
          hover:text-primary
          disabled:text-gray-400
        "
        :disabled="timer > 0 || isResending"
        @click="resendOtp"
      >
        {{ isResending ? $t('loader.sending') : $t('form.haventReceivedOTP') }}
      </button>

      <span class="text-neutral-20">{{ formattedTimer }}</span>
    </div>

    <button
      type="button"
      class="
        mb-4 h-13.5 w-full rounded-2xl bg-primary text-center leading-6 font-medium tracking-tight
        text-white
        hover:bg-primary/90
        disabled:bg-gray-500 disabled:text-black/20
      "
      :disabled="otp.length < inputOTPLength"
      @click="$emit('verify', otp)"
    >
      {{ $t('button.continue') }}
    </button>

    <button
      type="button"
      class="btn-secondary-outline flex h-13.5 w-full items-center justify-center gap-2"
      @click="$emit('back')"
    >
      <IconArrow class="rotate-180 fill-neutral-20!" />
      {{ $t('button.back') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import { formatTimer } from '@/utils/time'
  import { useIntervalFn } from '@vueuse/core'
  import { MfaChannel } from '@/types/request/mfa'
  import { useAuth } from '@/composables/admin-dashboard/useAuth'
  import { useMfaStore } from '@/stores/admin-dashboard/mfa'
  import FormHeader from '../../../components/FormHeader.vue'

  const { user } = useAuth()
  const { t } = useI18n()

  const props = defineProps<{
    method: MfaChannel
    errorMessage: string | null
  }>()

  defineEmits<{
    verify: [code: string]
    back: []
  }>()

  const mfaStore = useMfaStore()

  const otp = ref('')
  const inputOTPLength = ref(6)
  const timer = ref(30)
  const isResending = ref(false)

  const formattedTimer = computed(() => formatTimer(timer.value))

  const verificationMethodLabel = computed(() => (
    props.method === MfaChannel.EMAIL
      ? t('form.email').toLowerCase()
      : t('form.phone')
  ))

  const verificationDescription = computed(() => {
    if (props.method === MfaChannel.EMAIL) {
      return user.value?.user?.email ?? t('form.emailMasked')
    }

    return t('form.phoneMasked', { number: user.value?.user?.phone?.slice(-4) })
  })

  const { pause, resume } = useIntervalFn(() => {
    if (timer.value > 0) {
      timer.value--
    } else {
      pause()
    }
  }, 1000, { immediate: false })

  function startTimer() {
    timer.value = 30
    resume()
  }

  async function resendOtp() {
    if (isResending.value) return

    isResending.value = true

    const { success } = await mfaStore.sendMfaCode({ method: props.method })
    isResending.value = false

    if (success) startTimer()
  }

  onMounted(startTimer)
</script>

<style scoped>

</style>
