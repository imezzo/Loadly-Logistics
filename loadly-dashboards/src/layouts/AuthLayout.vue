<template>
  <main class="flex h-screen min-h-screen w-screen min-w-screen overflow-hidden">
    <section
      class="
        flex w-full flex-col items-center justify-start overflow-y-auto px-10 py-5
        sm:px-20
        lg:w-1/2 lg:justify-center lg:py-0
        [@media(height<--theme(--breakpoint-2sm))]:justify-start
        [@media(height<--theme(--breakpoint-2sm))]:py-8
      "
    >
      <RouterView />
    </section>

    <aside
      v-if="!isMobile"
      :class="[
        `
          relative hidden size-full flex-col items-center justify-center overflow-hidden bg-auth-bg
          bg-cover
          lg:flex lg:w-1/2
          en:rounded-bl-[6.25rem]
          ar:rounded-br-[6.25rem]
        `,
      ]"
    />
  </main>
</template>

<script setup lang="ts">
  import { useMediaQuery } from '@vueuse/core'

  const isMobile = useMediaQuery('(max-width: 64rem)')
</script>

<style scoped>

</style>

