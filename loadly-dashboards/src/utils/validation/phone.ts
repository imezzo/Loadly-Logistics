import { string } from 'yup'
import type { StringSchema } from 'yup'
import { isValidPhoneNumber, validatePhoneNumberLength } from 'libphonenumber-js'

export function createPhoneSchema(t: (key: string) => string, required = false): StringSchema<string | undefined> {
  let schema = string()

  if (required) {
    schema = schema.required()
  } else {
    schema = schema.optional()
  }

  return schema.test('phone', () => t('validation.phone'), (value: string | undefined) => {
    if (!value || value.trim() === '') return true

    return isValidPhoneNumber(value) && !validatePhoneNumberLength(value)
  })
}

