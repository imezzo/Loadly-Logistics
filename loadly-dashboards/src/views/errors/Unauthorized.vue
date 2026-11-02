<template>
  <div class="flex min-h-screen items-center justify-center bg-neutral-500 px-4">
    <div class="w-full max-w-md text-center">
      <div class="mb-6 flex justify-center">
        <div class="flex size-20 items-center justify-center rounded-full bg-error-50">
          <IconLock class="size-10 text-error-600" />
        </div>
      </div>

      <h1 class="mb-4 text-3xl font-bold text-neutral-800">
        {{ $t('error.unauthorized.title') }}
      </h1>

      <p class="mb-2 text-lg text-neutral-600">
        {{ $t('error.unauthorized.message') }}
      </p>

      <p class="mb-4 text-neutral-700">
        {{ $t('error.unauthorized.contactSupport') }}
      </p>

      <RouterLink
        to="/"
        class="btn-primary mx-auto"
      >
        {{ $t('button.goToDashboard') }}
      </RouterLink>
    </div>
  </div>
</template>

<script setup lang="ts">
  defineOptions({
    name: 'UnauthorizedPage',
  })
</script>

<style scoped>

</style>

