import type { MfaChannel } from '../../request/mfa'

export interface LoginRequest {
  email?: string
  username?: string
  phone?: string
  password: string
}

export interface ForgotPasswordRequest {
  identifier: string
  channel: MfaChannel
}

export interface ResetPasswordRequest {
  token?: string
  password: string
  confirmPassword: string
}

export interface ResetPasswordWithCodeRequest {
  identifier: string
  code: string
  newPassword: string
  confirmPassword: string
}

export interface ApiResetPasswordWithCodeRequest {
  identifier: string
  code: string
  newPassword: string
  confirmPassword: string
}



export interface ApiLoginRequest {
  login: string
  password: string
}

export interface ApiForgotPasswordRequest {
  identifier: string
  channel: MfaChannel
}

export interface ApiResetPasswordRequest {
  token?: string
  newPassword: string
  confirmPassword: string
}



export function loginRequestAdapter(request: LoginRequest): ApiLoginRequest {
  return {
    login: request.email ?? request.username ?? request.phone ?? '',
    password: request.password,
  }
}

export function forgotPasswordRequestAdapter(request: ForgotPasswordRequest): ApiForgotPasswordRequest {
  return {
    identifier: request.identifier,
    channel: request.channel,
  }
}

export function resetPasswordRequestAdapter(request: ResetPasswordRequest): ApiResetPasswordRequest {
  const payload: ApiResetPasswordRequest = {
    newPassword: request.password,
    confirmPassword: request.confirmPassword,
  }
  if (request.token) payload.token = request.token

  return payload
}

export function resetPasswordWithCodeRequestAdapter(
  request: ResetPasswordWithCodeRequest
): ApiResetPasswordWithCodeRequest {
  return {
    identifier: request.identifier,
    code: request.code,
    newPassword: request.newPassword,
    confirmPassword: request.confirmPassword,
  }
}
