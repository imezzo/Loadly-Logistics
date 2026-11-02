import { setLocale } from 'yup';

export function useVeeValidate() {

  const { t, te, locale } = useI18n();

  const fieldLabel = (path: string) => te(`form.${path}`) ? t(`form.${path}`, path) : path
        .replace(/([A-Z])/g, ' $1')
        .replace(/^./, str => str.toUpperCase())
        .trim()

  const changeLocale = () => {
    setLocale({
      mixed: {
        required: ({ path }) => t("validation.required", { field: fieldLabel(path) }),
        oneOf: ({ values }) => t("validation.oneOf", { values }),
        notOneOf: ({ values }) => t("validation.notOneOf", { values }),
        defined: () => t("validation.defined"),
      },
      string: {
        min: ({ min }) => t("validation.min", { min }),
        max: ({ max }) => t("validation.max", { max }),
        email: () => t("validation.email"),
        url: () => t("validation.url"),
        length: ({ length }) => t("validation.length", { length }),
        matches: () => t("validation.regex"),
      },
      number: {
        min: ({ min }) => t("validation.minNum", { min }),
        max: ({ max }) => t("validation.maxNum", { max }),
        integer: () => t("validation.integer"),
        positive: () => t("validation.positive"),
        negative: () => t("validation.negative"),
      },
      date: {
        min: ({ min }) => t("validation.minDate", { min }),
        max: ({ max }) => t("validation.maxDate", { max }),
      },
      array: {
        min: ({ min }) => t("validation.arrayMin", { min }),
        max: ({ max }) => t("validation.arrayMax", { max }),
      },
      object: {
        noUnknown: () => t("validation.noUnknown"),
      },
    })
  }

  changeLocale()

  watch(locale, () => changeLocale())

  return { changeLocale };
}
