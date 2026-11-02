export enum MfaChannel {
  NONE = 0,
  EMAIL = 1,
  SMS = 2,
}

export interface SendMfaCodeRequest {
  method: MfaChannel
}

export interface ApiSendMfaCodeRequest {
  channel: MfaChannel
}

export function sendMfaCodeRequestAdapter(request: SendMfaCodeRequest): ApiSendMfaCodeRequest {
  return {
    channel: request.method,
  }
}
