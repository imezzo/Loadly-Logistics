<template>
  <BasePopup
    v-model="model"
    content-class="max-w-220! [&>div]:border-none [&>div]:p-0"
  >
    <div class="w-full px-6 py-5">
      <h2
        v-if="view === ActionStatusView.Menu"
        class="mb-6 text-xl font-bold text-neutral-10"
      >
        {{ $t('title.actionStatus') }}
      </h2>

      <TripActionStatusMenu
        v-if="view === ActionStatusView.Menu"
        :trip-id="tripId"
        :can-cancel="canCancel"
        @cancel-trip="view = ActionStatusView.Cancel"
        @change-status="view = ActionStatusView.ChangeStatus"
        @updated="$emit('updated')"
      />

      <CancelTripAction
        v-if="view === ActionStatusView.Cancel"
        :trip-id="tripId"
        show-form
        @discard="view = ActionStatusView.Menu"
        @cancelled="onCancelled"
      />

      <ChangeStatusAction
        v-if="view === ActionStatusView.ChangeStatus"
        :trip-id="tripId"
        show-form
        @discard="view = ActionStatusView.Menu"
        @completed="onCompleted"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  enum ActionStatusView {
    Menu = 'menu',
    Cancel = 'cancel',
    ChangeStatus = 'changeStatus',
  }

  const TripActionStatusMenu = defineAsyncComponent(() => import('./TripActionStatusMenu.vue'))
  const CancelTripAction = defineAsyncComponent(() => import('./cancel-trip/CancelTripAction.vue'))
  const ChangeStatusAction = defineAsyncComponent(
    () => import('./complete-trip/ChangeStatusAction.vue'),
  )

  defineProps<{
    tripId: string | number
    canCancel?: boolean
  }>()

  const emit = defineEmits<{
    cancelled: []
    completed: []
    updated: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const view = ref(ActionStatusView.Menu)

  function onCancelled(): void {
    model.value = false
    emit('cancelled')
  }

  function onCompleted(): void {
    model.value = false
    emit('completed')
  }

  watch(model, (isOpen) => {
    if (isOpen) view.value = ActionStatusView.Menu
  })
</script>

<style scoped>

</style>
