<template>
  <button
    type="button"
    class="btn-primary-outline"
  >
    <slot>
      {{ $t('button.export') }}

      <IconExport />
    </slot>
  </button>
</template>

<script setup lang="ts">

</script>

<style scoped>

</style>
