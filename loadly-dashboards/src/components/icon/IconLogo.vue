<template>
  <div class="flex items-center gap-2">
    <svg
      v-if="!isNameOnly"
      width="48"
      height="52"
      viewBox="0 0 48 52"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden="true"
    >
      <path
        d="M24 2.5L42.186 13.25V34.75L24 45.5L5.814 34.75V13.25L24 2.5Z"
        fill="#0F766E"
      />
      <path
        d="M14 28.5L24 34.5L34 28.5"
        stroke="#E8A317"
        stroke-width="2.6"
        stroke-linecap="round"
        stroke-linejoin="round"
      />
      <path
        d="M14 22.5L24 28.5L34 22.5"
        stroke="#5EEAD4"
        stroke-width="2.6"
        stroke-linecap="round"
        stroke-linejoin="round"
      />
      <path
        d="M14 16.5L24 22.5L34 16.5"
        stroke="#F0FDFA"
        stroke-width="2.6"
        stroke-linecap="round"
        stroke-linejoin="round"
      />
    </svg>

    <svg
      v-if="withName"
      width="118"
      height="28"
      viewBox="0 0 118 28"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden="true"
    >
      <text
        x="0"
        y="21"
        fill="#0B1220"
        font-family="Manrope, Cairo, sans-serif"
        font-size="22"
        font-weight="700"
        letter-spacing="-0.4"
      >Loadly</text>
      <circle cx="49.2" cy="8.2" r="2.2" fill="#E8A317" />
    </svg>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    withName?: boolean
    isNameOnly?: boolean
  }>()
</script>
