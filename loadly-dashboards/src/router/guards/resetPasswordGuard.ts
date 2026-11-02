import { cookies } from '@/utils/cookies'

export function resetPasswordGuard(): string | boolean {
  if (cookies.get('accessToken')) {
    cookies.removeAll()
  }

  return true
}

