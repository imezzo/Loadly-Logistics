<template>
  <svg
    v-if="isOpen"
    aria-hidden="true"
    width="20"
    height="20"
    viewBox="0 0 20 20"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <g clip-path="url(#clip0_1016_2708)">
      <path
        d="M18.325 6.66671C18.325 6.06671 18.0166 5.54171 17.5416 5.25004L9.99996 0.833374L2.45829 5.25004C1.98329 5.54171 1.66663 6.06671 1.66663 6.66671V15C1.66663 15.9167 2.41663 16.6667 3.33329 16.6667H16.6666C17.5833 16.6667 18.3333 15.9167 18.3333 15L18.325 6.66671ZM16.6583 6.66671V6.67504L9.99996 10.8334L3.33329 6.66671L9.99996 2.76671L16.6583 6.66671ZM3.33329 15V8.61671L9.99996 12.8L16.6583 8.64171L16.6666 15H3.33329Z"
      />
    </g>
    <defs>
      <clipPath>
        <rect
          width="20"
          height="20"
          fill="white"
        />
      </clipPath>
    </defs>
  </svg>

  <svg
    v-else
    width="20"
    height="20"
    viewBox="0 0 20 20"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
    class="fill-transparent!"
  >
    <path
      d="M6.24854 8.12427L8.68744 9.62793C9.49222 10.1241 10.508 10.1241 11.3128 9.62793L13.7517 8.12427"
      stroke="black"
      stroke-opacity="0.2"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <rect
      x="2.49707"
      y="4.16431"
      width="15.0062"
      height="11.6715"
      rx="3"
      stroke="#1B1B1B"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>
</template>

<script setup lang="ts">
  withDefaults(defineProps<{
    isOpen?: boolean
  }>(), {
    isOpen: true,
  })
</script>

<style scoped>
  svg {
    fill: var(--color-neutral-10);

    path {
      fill: inherit;
    }
  }
</style>
