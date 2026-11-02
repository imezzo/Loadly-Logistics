<template>
  <span
    class="inline-flex items-baseline"
    aria-live="polite"
    :aria-label="formattedValue"
  >
    <template
      v-for="(part, index) in digitParts"
      :key="index"
    >
      <span
        v-if="!part.isDigit"
        class="inline-block"
      >
        {{ part.char }}
      </span>

      <span
        v-else
        class="flip-digit relative inline-block h-[1em] w-[1ch] overflow-hidden"
      >
        <Transition name="flip">
          <span
            :key="part.char"
            class="absolute inset-0 flex items-center justify-center"
          >
            {{ part.char }}
          </span>
        </Transition>
      </span>
    </template>
  </span>
</template>

<script setup lang="ts">
  const props = defineProps<{
    formatOptions?: Intl.NumberFormatOptions
    locale?: string
    value: number
  }>()

  const formattedValue = computed(() =>
    props.value.toLocaleString(props.locale, props.formatOptions ?? {}),
  )

  const digitParts = computed(() =>
    formattedValue.value.split('').map((char) => ({
      char,
      isDigit: /\d/.test(char),
    })),
  )
</script>

<style scoped>
  .flip-enter-active,
  .flip-leave-active {
    transition:
      transform 0.45s cubic-bezier(0.22, 1, 0.36, 1),
      opacity 0.45s cubic-bezier(0.22, 1, 0.36, 1);
  }

  .flip-enter-from {
    transform: translateY(110%);
    opacity: 0;
  }

  .flip-leave-to {
    transform: translateY(-110%);
    opacity: 0;
  }

  .flip-leave-active {
    position: absolute;
    inset: 0;
  }

  @media (prefers-reduced-motion: reduce) {
    .flip-enter-active,
    .flip-leave-active {
      transition: none;
    }
  }
</style>
