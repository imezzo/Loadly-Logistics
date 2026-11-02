import { fileURLToPath, URL } from 'node:url'
import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import tailwindcss from '@tailwindcss/vite'
import Components from 'unplugin-vue-components/vite'
import AutoImport from 'unplugin-auto-import/vite'
import { Platform } from './src/types/platform'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')

  const buildOutputDirs: Record<string, string> = {
    [Platform.ADMIN_DASHBOARD]: 'dist/admin-dashboard',
    [Platform.APP_DASHBOARD]: 'dist/app-dashboard',
    [Platform.COMPANY_DASHBOARD]: 'dist/company-dashboard'
  };

  return {
    plugins: [
      vue(),
      vueDevTools(),
      tailwindcss(),
      Components({
        dirs: ['src/components'],
        extensions: ['vue'],
        deep: true,
        dts: true,
      }),
      AutoImport({
        imports: [
          'vue',
          'vue-router',
          'pinia',
          { 'vue-i18n': ['useI18n'] }
        ],
        eslintrc: {
          enabled: true,
          filepath: './.eslintrc-auto-import.json',
          globalsPropValue: true
        },
        dts: './auto-imports.d.ts',
      }),
    ],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      },
    },
    server: {
      port: parseInt(env.VITE_PORT),
    },
    build: {
      outDir: buildOutputDirs[mode] || 'dist',
      emptyOutDir: true, // only empties that folder, not others
    }
  }
})
