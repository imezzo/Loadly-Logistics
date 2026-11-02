<template>
  <main class="min-h-screen bg-neutral-100">
    <article
      class="
        mx-auto max-w-4xl px-4 py-6
        sm:px-2 sm:py-8
      "
    >
      <BaseAlert
        v-if="errorMessage"
        :message="errorMessage"
        variant="error"
      />

      <template v-else-if="notification">
        <header class="mb-8 border-b border-neutral-200 pb-4">
          <h1
            class="
              text-2xl font-bold wrap-break-word text-neutral-10
              sm:text-3xl
            "
            dir="auto"
          >
            {{ notification.subject }}
          </h1>

          <p
            v-if="sentAt"
            class="mt-2 flex items-center gap-2 text-sm text-content-secondary"
          >
            <IconCalendar class="size-4 shrink-0" />
            <span>{{ $t('content.notificationDate') }}:</span>
            <time :datetime="notification.sentAt">{{ sentAt }}</time>
          </p>
        </header>

        <section>
          <p
            class="leading-relaxed wrap-break-word whitespace-pre-line text-neutral-10"
            dir="auto"
          >
            {{ notification.body }}
          </p>
        </section>

        <section
          v-if="notification.attachments.length"
          class="mt-4"
        >
          <h2 class="mb-4 text-lg font-semibold text-neutral-10">
            {{ $t('content.attachments') }}
          </h2>

          <div
            v-if="imageAttachments.length"
            class="flex flex-col gap-4"
          >
            <a
              v-for="(attachment, index) in imageAttachments"
              :key="attachment.id"
              :href="attachment.url"
              target="_blank"
              rel="noopener noreferrer"
              class="
                block overflow-hidden rounded-lg border border-neutral-200 bg-white
                transition-opacity
                hover:opacity-90
              "
            >
              <img
                :src="attachment.url"
                :alt="`${notification.subject} - ${index + 1}`"
                class="h-auto w-full object-contain"
                loading="lazy"
              >
            </a>
          </div>

          <ul
            v-if="fileAttachments.length"
            class="mt-4 flex flex-col gap-2"
          >
            <li
              v-for="attachment in fileAttachments"
              :key="attachment.id"
            >
              <a
                :href="attachment.url"
                target="_blank"
                rel="noopener noreferrer"
                class="
                  flex items-center gap-2 rounded-lg border border-neutral-200 bg-white px-4 py-3
                  text-sm text-neutral-10
                  hover:opacity-80
                "
              >
                <IconDownload class="size-4 shrink-0" />
                <span class="truncate">{{ getFileName(attachment.url) }}</span>
              </a>
            </li>
          </ul>
        </section>
      </template>
    </article>
  </main>
</template>

<script setup lang="ts">
  import { usePublicBroadcastNotification } from './composables/usePublicBroadcastNotification'

  const {
    errorMessage,
    fileAttachments,
    getFileName,
    getNotification,
    imageAttachments,
    notification,
    sentAt,
  } = usePublicBroadcastNotification()

  onMounted(getNotification)
</script>

<style scoped>

</style>
