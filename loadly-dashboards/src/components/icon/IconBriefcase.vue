<template>
  <svg
    aria-hidden="true"
    width="18"
    height="18"
    viewBox="0 0 18 18"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      d="M9 11.25V12.375"
      stroke="#999999"
      stroke-width="1.125"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      opacity="0.4"
      class="stroke-neutral-700"
      d="M2.24902 11.25C2.36101 13.4494 2.47831 14.6037 3.29761 15.3335C4.18608 16.125 5.57194 16.125 8.34366 16.125H9.65443C12.4261 16.125 13.812 16.125 14.7004 15.3335C15.5197 14.6037 15.637 13.4494 15.749 11.25"
      stroke-width="1.125"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      class="stroke-neutral-700"
      d="M2.13465 7.83232C3.40913 10.2558 6.28367 11.25 8.99927 11.25C11.7149 11.25 14.5894 10.2558 15.8639 7.83232C16.4723 6.67542 16.0116 4.5 14.5133 4.5H3.48527C1.98694 4.5 1.52629 6.67542 2.13465 7.83232Z"
      stroke-opacity="0.4"
      stroke-width="1.125"
    />
    <path
      opacity="0.4"
      class="stroke-neutral-700"
      d="M11.9993 4.5L11.9331 4.2682C11.6031 3.11317 11.4381 2.53565 11.0452 2.20532C10.6524 1.875 10.1306 1.875 9.08696 1.875H8.91153C7.86798 1.875 7.34617 1.875 6.95334 2.20532C6.56051 2.53565 6.3955 3.11317 6.06549 4.2682L5.99927 4.5"
      stroke-width="1.125"
    />
  </svg>
</template>

<script setup lang="ts">
  const props = withDefaults(defineProps<{
    color?: string
  }>(), {
    color: 'var(--color-neutral-700)',
  })
</script>

<style scoped>
  svg {
    stroke: v-bind('props.color');

    path {
      stroke: inherit;

      &.stroke-neutral-700 {
        stroke: inherit;
      }
    }
  }
</style>

