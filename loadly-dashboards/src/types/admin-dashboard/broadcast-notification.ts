export enum BroadcastNotificationChannel {
  EMAIL = 1,
  SMS = 2,
  IN_APP = 3,
}

export const API_CHANNEL_MAP: Record<string, BroadcastNotificationChannel> = {
  EMAIL: BroadcastNotificationChannel.EMAIL,
  SMS: BroadcastNotificationChannel.SMS,
  IN_APP: BroadcastNotificationChannel.IN_APP,
}

export const BroadcastNotificationModelType = {
  DISPATCHER: 'admin',
  COMPANY: 'company',
  DRIVER: 'driver',
} as const
