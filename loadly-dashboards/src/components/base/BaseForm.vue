<template>
  <Form
    ref="form"
    v-slot="formSlotProps"
    :initial-values="initialValues"
    @submit="$emit('submit', $event)"
  >
    <div v-if="groups">
      <template
        v-for="({ class: groupClass, title, titleClass, description, fields, wrapperClass }, _, index) in groups"
        :key="index"
      >
        <div :class="groupClass">
          <div>
            <h2
              v-if="title"
              :class="titleClass"
              class="mb-1 text-neutral-600"
            >
              {{ title }}
            </h2>

            <p
              v-if="description"
              class="text-neutral-700"
            >
              {{ description }}
            </p>
          </div>

          <div :class="wrapperClass">
            <template
              v-for="field in fields"
              :key="field.name"
            >
              <slot
                :name="`field-before-${field.name}`"
                :field="field"
              />

              <FormField
                :field="field"
                :error-class="errorClass"
                :errors="errors"
                :form-values="formSlotProps.values"
                :class="field.containerClass"
              >
                <template
                  v-if="field.fieldSlot"
                  #[`field-${field.name}`]="slotProps"
                >
                  <slot
                    :name="`field-${field.name}`"
                    v-bind="slotProps"
                  />
                </template>
              </FormField>

              <slot
                :name="`field-after-${field.name}`"
                :field="field"
              />
            </template>
          </div>

          <slot
            :name="`group-after-${title}`"
          />
        </div>

        <slot
          v-if="Number(index) < (Object.keys(groups).length as number - 1)"
          name="separator"
        >
          <hr class="mb-3 border-t border-neutral-500">
        </slot>
      </template>
    </div>

    <div
      v-else
      :class="containerClass"
    >
      <template
        v-for="field in fieldsSchema"
        :key="field.name"
      >
        <slot
          :name="`field-before-${field.name}`"
          :field="field"
        />

        <FormField
          :field="field"
          :error-class="errorClass"
          :errors="errors"
          :form-values="formSlotProps.values"
          :class="field.containerClass"
        >
          <template
            v-if="field.fieldSlot"
            #[`field-${field.name}`]="slotProps"
          >
            <slot
              :name="`field-${field.name}`"
              v-bind="slotProps"
            />
          </template>
        </FormField>

        <slot
          :name="`field-after-${field.name}`"
          :field="field"
        />
      </template>
    </div>

    <slot :form-values="formSlotProps.values" />

    <slot
      name="submit"
      :meta="formSlotProps.meta"
    >
      <button v-bind="submitAttrs">
        <slot name="submit-text">
          {{ $t('form.submit') }}
        </slot>
      </button>
    </slot>
  </Form>
</template>

<script setup lang="ts">
  import { Form } from 'vee-validate'
  import type { Schema, FieldsGroup } from '@/types/form'
  import FormField from './form/FormField.vue'

  const props = defineProps<{
    initialValues?: Record<string, unknown>
    fieldsSchema?: Schema[]
    groups?: Record<string, FieldsGroup>
    submitAttrs?: Record<string, unknown>
    errorClass?: string
    errors?: Record<string, string[]>
    containerClass?: string
  }>()

  const emit = defineEmits<{
    submit: [values: Record<string, unknown>]
  }>()

  const form = useTemplateRef('form')

  watch(() => props.initialValues, (newValues) => {
    if (newValues) {
      form.value?.setValues(newValues, false)
    }
  }, { deep: true })

  async function submitForm(): Promise<void> {
    const instance = form.value
    if (!instance?.validate) return
    const { valid } = await instance.validate()
    if (valid) {
      const values = (typeof instance.getValues === 'function' ? instance.getValues() : {}) as Record<string, unknown>
      emit('submit', values)
    }
  }

  defineExpose({
    form,
    submitForm,
  })
</script>

<style scoped>

</style>
