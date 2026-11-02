import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import { i18n, applyStoredLocale, loadLocaleMessages } from './locales'
import type { AllowedLanguage } from './locales'
import { useApp } from './composables/useApp'
import { reloadAppOnce } from '@/utils/chunkReload'
import { platformComponent } from './utils/platformComponentResolver'
import './assets/css/index.css'

window.addEventListener('vite:preloadError', (event) => {
  event.preventDefault()
  reloadAppOnce()
})

const app = createApp(platformComponent)

app.use(createPinia())
app.use(router)
app.use(i18n)

const initialLocale = (localStorage.getItem('locale') ?? useApp().defaultLanguage) as AllowedLanguage

await loadLocaleMessages(initialLocale)
applyStoredLocale()

app.mount('#app')
