<template>
  <div>
    <label
      v-if="field.label"
      :for="field.name"
      :class="field.labelClass || 'mb-1 flex items-center font-medium text-neutral-10'"
    >
      {{ field.label }}
      <span
        v-if="field.labelRequired"
        class="ms-1 text-error-600"
      >*</span>

      <IconInfo
        v-if="field.labelInfo"
        variant="filled"
        class="size-5 text-neutral-40"
      />
    </label>

    <Field
      v-slot="{ value, handleChange, errorMessage }"
      :name="field.name"
    >
      <component
        :is="resolveComponent(field.as)"
        :field-id="field.name"
        :model-value="value"
        :wrapper-class="[
          errorMessage ? 'input-error' : '',
          field.wrapperClass,
        ].flat().join(' ')"
        v-bind="field"
        @update:model-value="handleChange"
      >
        <template
          v-if="field.leftIcon"
          #left-icon
        >
          <component :is="resolveComponent(field.leftIcon as string)" />
        </template>
      </component>

      <p
        v-if="errorMessage || errors?.[field.name]?.[0]"
        :class="errorClass"
      >
        {{ errorMessage || errors?.[field.name]?.[0] }}
      </p>

      <slot
        v-if="field.fieldSlot"
        :name="`field-${field.name}`"
        :value="value"
        :form-values="formValues"
      />
    </Field>
  </div>
</template>

<script setup lang="ts">
  import { Field } from 'vee-validate'
  import type { Schema } from '@/types/form'

  withDefaults(defineProps<{
    field: Schema
    errorClass?: string
    errors?: Record<string, string[]>
    formValues?: Record<string, unknown>
  }>(), {
    errorClass: 'mt-1 text-error-600',
    errors: undefined,
    formValues: undefined,
  })

  const resolvedComponents = new Map<string, Component>()

  function resolveComponent(name: string): Component | string {
    if (name === name.toLowerCase()) return name

    const existing = resolvedComponents.get(name)

    if (existing) return existing

    const component = defineAsyncComponent(() => {
      if (name.startsWith('Icon')) {
        return import(`@/components/icon/${name}.vue`)
      }

      return import(`@/components/base/${name}.vue`)
    })

    resolvedComponents.set(name, component)

    return component
  }
</script>

