import type {
  AxiosError,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios"
import { useNetworkStore } from "@/stores/admin-dashboard/network"
import { onUnauthorizedError } from "./unauthorized"

const networkStore = useNetworkStore()
export const requestInterceptor = (config: InternalAxiosRequestConfig) => {
  networkStore.clearNetworkError()

  if (config.data instanceof FormData) {
    delete config.headers['Content-Type']
  }

  return config
}

export const requestErrorInterceptor = (error: AxiosError) => {
  return Promise.reject(error)
}

export const responseInterceptor = (response: AxiosResponse) => {
  return {
    ...response,
    data: response.data,
  }
}

export const responseErrorInterceptor = async (error: AxiosError) => {
  if (!error.response && error.request) {
    networkStore.setNetworkError(true, error.message)

    return Promise.reject(error)
  }

  const status = error.response?.status
  switch (status) {
    case 401: {
      const responseURL = error.request.responseURL || ''
      const hasRefreshToken = responseURL.includes('/auth/') && !responseURL.includes('/auth/profile')
      const retryResponse = await onUnauthorizedError(hasRefreshToken, error.config)

      if (retryResponse) {
        return retryResponse
      }
      break
    }
    case 404:
      console.warn("Resource not found")
      break
    default:
      break
  }

  return Promise.reject(error)
}
