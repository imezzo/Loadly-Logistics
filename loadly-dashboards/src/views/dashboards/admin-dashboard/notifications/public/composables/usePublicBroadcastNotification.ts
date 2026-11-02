import type { PublicBroadcastNotification } from '@/types/admin-dashboard/response/broadcast-notification'
import { useBroadcastNotificationStore } from '@/stores/admin-dashboard/broadcast-notification'
import { useLoading } from '@/composables/useLoading'
import { formatDate, formatTime } from '@/utils/time'

const IMAGE_EXTENSIONS_REGEX = /\.(?:avif|bmp|gif|jpe?g|png|svg|webp)(?:\?.*)?$/i

export function usePublicBroadcastNotification() {
  const { t, locale } = useI18n()
  const route = useRoute()
  const { setLoading } = useLoading()
  const broadcastNotificationStore = useBroadcastNotificationStore()

  const notification = ref<PublicBroadcastNotification | null>(null)
  const errorMessage = ref<string | null>(null)

  const sentAt = computed(() => {
    if (!notification.value?.sentAt) return ''

    const date = new Date(notification.value.sentAt)
    if (Number.isNaN(date.getTime())) return notification.value.sentAt

    return `${formatDate(date, locale.value)} ${t('content.at')} ${formatTime(date, locale.value)}`
  })

  const imageAttachments = computed(() =>
    notification.value?.attachments.filter(({ url }) => isImageUrl(url)) ?? [])

  const fileAttachments = computed(() =>
    notification.value?.attachments.filter(({ url }) => !isImageUrl(url)) ?? [])

  function isImageUrl(url: string): boolean {
    return IMAGE_EXTENSIONS_REGEX.test(url)
  }

  function getFileName(url: string): string {
    return url.split('/').pop() || url
  }

  async function getNotification(): Promise<void> {
    const code = route.params.code?.toString()
    if (!code) return

    errorMessage.value = null
    setLoading(true)
    const { success, data, message } = await broadcastNotificationStore.getPublic(code)
    setLoading(false)

    if (success) {
      notification.value = data

      return
    }

    errorMessage.value = message || t('error.somethingWentWrong')
  }

  return {
    errorMessage,
    fileAttachments,
    getFileName,
    getNotification,
    imageAttachments,
    notification,
    sentAt,
  }
}
