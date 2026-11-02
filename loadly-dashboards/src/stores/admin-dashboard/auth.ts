import {
  forgotPasswordRequestAdapter,
  loginRequestAdapter,
  resetPasswordRequestAdapter,
  resetPasswordWithCodeRequestAdapter,
} from '@/types/admin-dashboard/request/auth'
import type {
  ForgotPasswordRequest,
  LoginRequest,
  ResetPasswordRequest,
  ResetPasswordWithCodeRequest,
} from '@/types/admin-dashboard/request/auth'
import type { LoginResponse } from "@/types/admin-dashboard/response/auth";
import { loginResponseAdapter } from "@/types/admin-dashboard/response/auth";
import { api } from "@/utils/api"

export const useAuthStore = defineStore('auth', () => {
  const user = ref<LoginResponse | null>(null)

  async function login(data: LoginRequest) {
    return api({
      url: '/auth/login',
      method: 'POST',
      data: loginRequestAdapter(data),
      responseAdapter: loginResponseAdapter,
    })
  }

  async function logout() {
    return api({
      url: '/auth/logout',
      method: 'POST',
      data: {
        revokeAll: false
      },
    })
  }

  async function forgotPassword(data: ForgotPasswordRequest) {
    return api({
      url: '/auth/forget-password',
      method: 'POST',
      data: forgotPasswordRequestAdapter(data),
    })
  }

  async function refreshToken(refreshToken: string) {
    return api({
      url: '/auth/refresh-token',
      method: 'POST',
      data: { refreshToken },
    })
  }

  async function resetPassword(data: ResetPasswordRequest) {
    return api({
      url: '/auth/reset-password',
      method: 'POST',
      data: resetPasswordRequestAdapter(data),
    })
  }

  async function resetPasswordWithCode(data: ResetPasswordWithCodeRequest) {
    return api({
      url: '/auth/reset-password-with-code',
      method: 'POST',
      data: resetPasswordWithCodeRequestAdapter(data),
    })
  }

  return {
    forgotPassword,
    login,
    logout,
    refreshToken,
    resetPassword,
    resetPasswordWithCode,
    user,
  }
})
