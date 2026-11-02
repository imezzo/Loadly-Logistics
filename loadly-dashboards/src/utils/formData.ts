export function appendToFormData(
  formData: FormData,
  key: string,
  value: string | undefined
): void {
  if (value) formData.append(key, value)
}
