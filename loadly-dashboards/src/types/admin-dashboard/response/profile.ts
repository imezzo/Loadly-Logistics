import type { MfaChannel } from "../../request/mfa"

export interface Profile {
  createdAt: string
  email: string
  emailVerified: boolean
  fullName: string
  id: number
  mfaChannel: MfaChannel
  permissions: Record<string, string[]>
  phone: string
  phoneVerified: boolean
  status: number
  type: number
  updatedAt: string
  userName: string
  userType: string
}

export interface ApiProfile {
  createdAt: string
  email: string
  emailVerified: boolean
  fullName: string
  id: number
  mfaChannel: MfaChannel
  permissions: Record<string, string[]>
  phone: string
  phoneVerified: boolean
  status: number
  type: number
  updatedAt: string
  userName: string
  userType: string
}

export function profileResponseAdapter(response: ApiProfile): Profile {
  return {
    createdAt: response.createdAt,
    email: response.email,
    emailVerified: response.emailVerified,
    fullName: response.fullName,
    id: response.id,
    mfaChannel: response.mfaChannel,
    permissions: response.permissions,
    phone: response.phone,
    phoneVerified: response.phoneVerified,
    status: response.status,
    type: response.type,
    updatedAt: response.updatedAt,
    userName: response.userName,
    userType: response.userType,
  }
}

