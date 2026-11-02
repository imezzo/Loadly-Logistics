<template>
  <button
    class="flex cursor-pointer items-center gap-3 rounded-full bg-neutral-30 py-1.5 ps-4 pe-7"
    v-bind="$attrs"
  >
    <img
      :src="avatarSrc"
      :alt="avatarAlt"
      loading="lazy"
      class="size-10 rounded-full object-cover"
    >

    <div class="flex flex-col items-start">
      <span class="leading-tight font-semibold text-black">
        {{ name }}
      </span>

      <span class="text-sm leading-tight text-black/60">
        {{ role }}
      </span>
    </div>
  </button>
</template>

<script setup lang="ts">
  import defaultAvatar from '@/assets/images/default-avatar/user.webp'

  withDefaults(defineProps<{
    name: string
    role: string
    avatarSrc?: string
    avatarAlt?: string
  }>(), {
    avatarSrc: defaultAvatar,
    avatarAlt: 'User avatar',
  })
</script>

<style scoped>

</style>
