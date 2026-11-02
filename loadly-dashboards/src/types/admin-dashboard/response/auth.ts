import type { MfaChannel } from "../../request/mfa"

export enum LoginMethod {
  PHONE = 'phone',
  USERNAME = 'username',
}

export interface User {
  email: string
  fullName: string
  id: number
  mfaChannel: MfaChannel
  permissions: Record<string, string[]>
  phone: string
  userName: string
  userType: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  user: User
}

export interface ApiUser {
  email: string
  fullName: string
  id: number
  mfaChannel: MfaChannel
  permissions: Record<string, string[]>
  phone: string
  userName: string
  userType: string
}

export interface ApiLoginResponse {
  accessToken: string
  expiresIn: number
  refreshToken: string
  user: ApiUser
}

export function loginResponseAdapter(response: ApiLoginResponse): LoginResponse {
  return {
    accessToken: response.accessToken,
    refreshToken: response.refreshToken,
    user: {
      email: response.user.email,
      fullName: response.user.fullName,
      id: response.user.id,
      mfaChannel: response.user.mfaChannel,
      permissions: response.user.permissions,
      phone: response.user.phone,
      userName: response.user.userName,
      userType: response.user.userType,
    },
  }
}
