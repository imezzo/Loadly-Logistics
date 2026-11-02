<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="model"
        class="fixed inset-0 z-51 flex items-center justify-center bg-black/50 backdrop-blur-sm"
        role="dialog"
        :aria-modal="true"
        @click="onBackdropClick"
      >
        <div
          class="
            relative flex w-full max-w-175 flex-col items-center justify-center rounded-4xl border
            border-gray-600 bg-white px-4 py-5 shadow-[0px_4px_32px_-4px_rgba(111,108,143,0.12)]
          "
          :class="contentClass"
          @click.stop
        >
          <div
            class="
              relative flex size-full items-start justify-center rounded-3xl border border-gray-600
              py-5
            "
          >
            <button
              v-if="showCloseButton"
              class="
                absolute end-4 top-4 flex size-8 items-center justify-center rounded-full
                border-[0.5px] border-gray-600 bg-white bg-linear-to-b to-gray-600
                shadow-[0px_0.94px_0.94px_0px_#96A1AC1F_inset]
              "
              @click="onBackdropClick"
            >
              <IconCross />
            </button>

            <slot />
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
  const props = withDefaults(defineProps<{
    persistent?: boolean
    showCloseButton?: boolean
    contentClass?: string
  }>(), {
    persistent: false,
    showCloseButton: true,
    contentClass: '',
  })

  const model = defineModel<boolean>('modelValue', { required: true })

  function onBackdropClick() {
    if (!props.persistent) {
      model.value = false
    }
  }
</script>

<style scoped>

</style>
