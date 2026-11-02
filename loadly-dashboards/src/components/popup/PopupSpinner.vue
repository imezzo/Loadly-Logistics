<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <span
        v-if="isLoading"
        class="fixed inset-0 z-9999 flex size-full items-center justify-center bg-theme"
      >
        <span>
          <IconLogoTruck class="truck" />
          <IconLogoHand class="hand" />
          <IconLogo
            class="logo"
            is-name-only
            with-name
          />
        </span>
      </span>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'

  const { isLoading } = useLoading()
</script>

<style scoped>
  .truck {
    position: relative;
    z-index: 2;
    transform: translateX(-100px);
    animation: truck-move 2s infinite;
  }

  .hand {
    transform: translateY(0);
    animation: hand-move 2s infinite;
  }

  .logo {
    transform: translateY(-90px);
    opacity: 0;
    animation: logo-move 2s infinite;
  }

  @keyframes logo-move {
    0% {
      opacity: 0;
    }

    50% {
      opacity: 1;
    }

    100% {
      opacity: 0;
    }
  }

  @keyframes truck-move {
    0% {
      transform: translateX(-100px);
    }

    50% {
      transform: translateX(10px);
    }

    100% {
      transform: translateX(-100px);
    }
  }

  @keyframes hand-move {
    0% {
      transform: translateY(0);
    }

    50% {
      transform: translateY(-100px);
    }

    100% {
      transform: translateY(0);
    }
  }
</style>
