import type { RouteRecordRaw } from 'vue-router'
import { resetPasswordGuard } from './guards/resetPasswordGuard'

export const authRoutes: RouteRecordRaw[] = [
  {
    path: '/auth',
    redirect: '/auth/login',
    component: () => import('@/layouts/AuthLayout.vue'),
    children: [
      {
        path: 'login',
        name: 'Login',
        component: () => import('@/views/global-modules/authentication/login/credentials/LoginCredentials.vue'),
        meta: {
          title: 'Login',
          requiresAuth: false,
        }
      },
      {
        path: 'login/verification-method',
        name: 'LoginVerificationMethod',
        component: () => import('@/views/global-modules/authentication/login/verification-method/VerificationMethod.vue'),
        meta: {
          title: 'Verification Method',
          requiresAuth: false,
          isMfaStep: true,
        }
      },
      {
        path: 'login/verify-code/:method',
        name: 'LoginVerifyCode',
        component: () => import('@/views/global-modules/authentication/login/verify-code/VerifyCode.vue'),
        meta: {
          title: 'Verify Code',
          requiresAuth: false,
          isMfaStep: true,
        }
      },
      {
        path: 'forgot-password',
        name: 'ForgotPassword',
        component: () => import('@/views/global-modules/authentication/forgot-password/ForgotPassword.vue'),
        meta: {
          title: 'Forgot Password',
          requiresAuth: false,
        }
      },
      {
        path: 'reset-password',
        name: 'ResetPassword',
        component: () => import('@/views/global-modules/authentication/reset-password/ResetPassword.vue'),
        meta: {
          title: 'Reset Password',
          requiresAuth: false,
        },
        beforeEnter: resetPasswordGuard,
      }
    ]
  }
]
