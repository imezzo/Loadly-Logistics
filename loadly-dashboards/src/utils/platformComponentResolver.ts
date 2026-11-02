import { Platform, isPlatform } from '@/types/platform';

const mode = import.meta.env.MODE as Platform;

if (!isPlatform(mode)) {
  throw new Error(`Invalid platform mode: ${mode}`)
}

const platformComponent = defineAsyncComponent(() => import(`@/views/dashboards/${mode}/App.vue`));

export { platformComponent };
