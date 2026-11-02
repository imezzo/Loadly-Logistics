<template>
  <svg
    v-if="visible"
    aria-hidden="true"
    width="17"
    height="14"
    viewBox="0 0 17 14"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      d="M10.0184 4.81674C10.9942 5.79258 10.9942 7.37674 10.0184 8.35424C9.04257 9.33008 7.4584 9.33008 6.4809 8.35424C5.50507 7.37841 5.50507 5.79424 6.4809 4.81674C7.4584 3.83924 9.04173 3.83924 10.0184 4.81674"
      stroke="currentColor"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      fill-rule="evenodd"
      clip-rule="evenodd"
      d="M0.75 6.58333C0.75 6.03417 0.876667 5.49083 1.12167 4.99V4.99C2.38417 2.40917 5.17417 0.75 8.25 0.75C11.3258 0.75 14.1158 2.40917 15.3783 4.99V4.99C15.6233 5.49083 15.75 6.03417 15.75 6.58333C15.75 7.1325 15.6233 7.67583 15.3783 8.17667V8.17667C14.1158 10.7575 11.3258 12.4167 8.25 12.4167C5.17417 12.4167 2.38417 10.7575 1.12167 8.17667V8.17667C0.876667 7.67583 0.75 7.1325 0.75 6.58333Z"
      stroke="currentColor"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>


  <svg
    v-else
    aria-hidden="true"
    width="20"
    height="20"
    viewBox="0 0 20 20"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      d="M12.1319 11.2983C11.599 12.1969 10.5638 12.6698 9.53555 12.4844C8.50735 12.2989 7.70259 11.4942 7.51715 10.466C7.3317 9.43778 7.80462 8.40259 8.70326 7.86963"
      stroke="currentColor"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      d="M14.9973 14.1635C13.56 15.2537 11.8042 15.8413 10.0002 15.8358C7.01093 15.889 4.24897 14.2461 2.86888 11.594C2.37313 10.5891 2.37313 9.41085 2.86888 8.406C3.55909 7.03152 4.65934 5.90547 6.01746 5.18359"
      stroke="currentColor"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      d="M17.0229 11.7789C17.0564 11.7155 17.0999 11.6587 17.1315 11.5941C17.6272 10.5892 17.6272 9.41095 17.1315 8.4061C15.7514 5.75395 12.9894 4.11111 10.0001 4.16434C9.81273 4.16434 9.63057 4.18935 9.44556 4.20154"
      stroke="currentColor"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <path
      d="M17.5031 16.6696L3.33057 2.49707"
      stroke="currentColor"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>
</template>

<script setup lang="ts">
  defineProps<{visible: boolean}>()
</script>

<style scoped>

</style>
