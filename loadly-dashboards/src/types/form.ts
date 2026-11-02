export interface Schema {
  as: string
  fieldSlot?: boolean
  group?: string
  label?: string
  labelClass?: string
  labelInfo?: boolean
  labelRequired?: boolean
  leftIcon?: string
  name: string
  wrapperClass?: string
  [key: string]: unknown
}

export interface FieldsGroup {
  class?: string
  description?: string
  descriptionClass?: string
  fields: Schema[]
  title?: string
  titleClass?: string
  wrapperClass?: string
}
