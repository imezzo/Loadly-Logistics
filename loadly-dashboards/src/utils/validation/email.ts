import type { Schema } from 'yup'
import { string } from 'yup'

export const EMAIL_REGEX = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/

export function createEmailSchema(t: (key: string) => string, required = true): Schema<string | undefined> {
  let schema = string()

  if (required) {
    schema = schema.required()
  } else {
    schema = schema.optional()
  }

  return schema.matches(EMAIL_REGEX, { excludeEmptyString: true, message: () => t('validation.email') })
}
