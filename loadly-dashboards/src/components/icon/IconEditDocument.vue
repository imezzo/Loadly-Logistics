<template>
  <svg
    aria-hidden="true"
    width="20"
    height="20"
    viewBox="0 0 20 20"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      d="M7.49915 17.5031H4.16443C3.24357 17.5031 2.49707 16.7566 2.49707 15.8357V4.16419C2.49707 3.24333 3.24357 2.49683 4.16443 2.49683H14.1686C15.0895 2.49683 15.836 3.24333 15.836 4.16419V6.66523"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      fill-rule="evenodd"
      clip-rule="evenodd"
      d="M10 17.5031L12.1876 17.233C12.3415 17.214 12.4847 17.1442 12.5944 17.0346L17.895 11.7341C18.482 11.1495 18.4843 10.1997 17.9 9.61234L17.895 9.60817V9.60817C17.3104 9.02108 16.3606 9.01884 15.7732 9.60317V9.60817L10.5211 14.8604C10.4142 14.9665 10.3451 15.1047 10.3243 15.2539L10 17.5031Z"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      fill-rule="evenodd"
      clip-rule="evenodd"
      d="M6.66532 14.1684V14.1684C5.97467 14.1684 5.41479 13.6085 5.41479 12.9179V12.9179C5.41479 12.2272 5.97467 11.6674 6.66532 11.6674V11.6674C7.35596 11.6674 7.91584 12.2272 7.91584 12.9179V12.9179C7.91584 13.6085 7.35596 14.1684 6.66532 14.1684Z"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      d="M5.41479 5.83164H12.0842"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      d="M5.41479 8.74948H8.74952"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>
</template>

<script setup lang="ts">
  const props = withDefaults(defineProps<{
    color?: string
  }>(), {
    color: '#BABCC2',
  })
</script>

<style scoped>
  svg {
    stroke: v-bind('props.color');

    path {
      stroke: inherit;
    }
  }
</style>

