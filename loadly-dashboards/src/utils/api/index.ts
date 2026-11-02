import axios from "axios";
import type { AxiosError, AxiosRequestConfig } from "axios";
import { useAuth } from "@/composables/admin-dashboard/useAuth";
import type { ApiResponse } from "@/types/admin-dashboard/response/response";
import { cookies } from "@/utils/cookies"
import { i18n } from "@/locales";
import { transformApiRequest, transformApiResponse } from "@/utils/api/transformers";
import type { RequestAdapter, ResponseAdapter } from "@/utils/api/transformers";
import { useApp } from "@/composables/useApp";

interface ApiConfig extends AxiosRequestConfig {
  requestAdapter?: RequestAdapter;
  responseAdapter?: ResponseAdapter;
}

export const api = async (config: ApiConfig) => {
  const { isLoggedIn, isTempLoggedIn } = useAuth()
  const { modeBaseUrl: baseURL } = useApp()

  let Authorization = '';
  if (isLoggedIn.value && cookies.get('accessToken')) {
    Authorization = `Bearer ${cookies.get('accessToken')}`;
  } else if (isTempLoggedIn.value && cookies.get('tempAccessToken')) {
    Authorization = `Bearer ${cookies.get('tempAccessToken')}`;
  }

  const instance = axios.create({
    baseURL,
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
      Authorization,
      lang: i18n.global.locale.value as string,
    },
    transformRequest: [
      (data, headers) =>
        transformApiRequest(data, headers, config.requestAdapter),
    ],
    transformResponse: config.responseType === "blob"
      ? [(data) => data]
      : [(data: unknown) => transformApiResponse(data, config.responseAdapter)],
  })

  const interceptors = await import("./interceptors")

  if (interceptors) {
    instance.interceptors.request.use(
      interceptors.requestInterceptor,
      interceptors.requestErrorInterceptor
    )
    instance.interceptors.response.use(
      interceptors.responseInterceptor,
      interceptors.responseErrorInterceptor
    )
  }

  try {
    const response = await instance.request(config)

    if (config.responseType === 'blob') {
      const blob = response.data as Blob

      return { success: true, message: "", data: null, blob }
    }

    const { success, message, data, errors } = response.data

    return { success, message, data, errors }
  } catch (error) {
    const response = (error as AxiosError).response?.data as ApiResponse<null>

    return {
      success: response?.success ?? false,
      message: response?.message ?? '',
      data: { data: response?.data ?? null },
      errors: response?.errors,
    }
  }
}
