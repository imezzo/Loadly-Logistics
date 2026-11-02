<template>
  <svg
    v-if="filled"
    aria-hidden="true"
    width="24"
    height="24"
    viewBox="0 0 24 24"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <g clip-path="url(#clip0_344_22438)">
      <path
        d="M12 12C13.1 12 14 11.1 14 10C14 8.9 13.1 8 12 8C10.9 8 10 8.9 10 10C10 11.1 10.9 12 12 12ZM12 2C16.2 2 20 5.22 20 10.2C20 13.38 17.55 17.12 12.66 21.43C12.28 21.76 11.71 21.76 11.33 21.43C6.45 17.12 4 13.38 4 10.2C4 5.22 7.8 2 12 2Z"
        fill="currentColor"
      />
    </g>
    <defs>
      <clipPath id="clip0_344_22438">
        <rect
          width="24"
          height="24"
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
  >
    <g clip-path="url(#clip0_800_26487)">
      <path
        d="M10.0002 2.16663C13.2394 2.16679 16.1661 4.64139 16.1663 8.49963C16.1663 9.6953 15.7054 11.0425 14.718 12.5524C13.7313 14.0612 12.2358 15.7046 10.218 17.483C10.1056 17.5781 9.94461 17.5898 9.82056 17.5172L9.76978 17.4811C7.75753 15.7036 6.26681 14.0605 5.28149 12.5524C4.29501 11.0424 3.83325 9.69534 3.83325 8.49963C3.83342 4.64126 6.76092 2.16663 10.0002 2.16663ZM10.0002 6.16663C8.80744 6.16663 7.83325 7.14081 7.83325 8.33362C7.83343 9.52628 8.80754 10.4996 10.0002 10.4996C11.1928 10.4995 12.1661 9.52617 12.1663 8.33362C12.1663 7.14092 11.1929 6.1668 10.0002 6.16663Z"
        stroke="currentColor"
        fill="white"
        stroke-opacity="0.4"
      />
    </g>
    <defs>
      <clipPath id="clip0_800_26487">
        <rect
          width="20"
          height="20"
          fill="white"
        />
      </clipPath>
    </defs>
  </svg>
</template>

<script setup lang="ts">
  defineProps<{
    filled?: boolean
  }>()
</script>

<style scoped>
  svg {
    fill: currentcolor;
  }
</style>

