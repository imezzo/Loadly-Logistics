import { string } from 'yup'
import type { Schema } from 'yup'

const DURATION_REGEX = /^(\d+)\s+(day|days|month|months|year|years)$/i

export function createDurationSchema(t: (key: string) => string): Schema<string> {
  return string()
    .required()
    .matches(DURATION_REGEX, () => t('validation.invalidDuration'))
}

