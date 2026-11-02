import { string } from 'yup'
import type { Schema } from 'yup'

export function createArabicTextSchema(t: (key: string) => string, maxLength?: number): Schema<string> {
  return string()
    .required()
    .matches(/^[\u0600-\u06FF\s\d\p{P}\p{S}]+$/u, () => t('validation.arabicOnly'))
    .max(maxLength ?? 100)
}

