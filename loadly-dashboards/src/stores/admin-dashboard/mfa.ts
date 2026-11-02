import { sendMfaCodeRequestAdapter, } from "@/types/request/mfa"
import type { SendMfaCodeRequest } from "@/types/request/mfa"
import { api } from "@/utils/api"

export const useMfaStore = defineStore('mfa', () => {

  async function sendMfaCode(data: SendMfaCodeRequest) {
    return api({
      url: '/auth/send-mfa-code',
      method: 'POST',
      data: sendMfaCodeRequestAdapter(data),
    })
  }

  async function verifyMfaCode(code: string) {
    return api({
      url: '/auth/verify-mfa',
      method: 'POST',
      data: { code },
    })
  }

  return {
    sendMfaCode,
    verifyMfaCode,
  }
})
