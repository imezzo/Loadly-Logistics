import router from "@/router"
import axios from "axios"
import type {
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios"
import { useAuth } from "@/composables/admin-dashboard/useAuth"
import { cookies } from "@/utils/cookies"

const { getRefreshToken } = useAuth()

async function refreshTokenAndRetryRequest(
  originalRequest: InternalAxiosRequestConfig
): Promise<AxiosResponse> {
  try {
    await getRefreshToken()

    const accessToken = cookies.get('accessToken')
    if (accessToken) {
      originalRequest.headers.Authorization = `Bearer ${accessToken}`
    }

    return axios(originalRequest)
  } catch (error) {
    router.push({name: 'Login'})

    return Promise.reject(error)
  }
}

export async function onUnauthorizedError(
  hasRefreshToken: boolean,
  originalRequest: InternalAxiosRequestConfig | undefined
): Promise<AxiosResponse | undefined> {
  if (hasRefreshToken) {
    router.push({name: 'Login'})

    return undefined
  }

  if (!originalRequest) {
    await getRefreshToken()

    return undefined
  }

  return refreshTokenAndRetryRequest(originalRequest)
}

