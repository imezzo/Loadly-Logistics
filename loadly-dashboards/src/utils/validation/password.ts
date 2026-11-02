import { string } from 'yup'

export function createPasswordSchema(t: (key: string) => string) {
  return string()
    .required()
    .min(8, t('validation.charactersMinimum'))
    .matches(/[A-Z]/, t('validation.atLeastOneCapital'))
    .matches(/[!@#$%^&*(),.?":{}|<>]/, t('validation.oneSymbolAndOneNumber'))
    .matches(/\d/, t('validation.oneSymbolAndOneNumber'))
}
