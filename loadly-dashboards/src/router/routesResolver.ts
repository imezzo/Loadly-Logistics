import { isPlatform, Platform } from "@/types/platform";
import type { RouteRecordRaw } from "vue-router";

const mode = import.meta.env.MODE as Platform;

if (!isPlatform(mode)) {
  throw new Error(`Invalid platform mode: ${mode}`)
}

let routes: readonly RouteRecordRaw[];

// ? Note: Keeping this configuration static is important to ensure that only the necessary routes are included in the bundle because if it's dynamic, it will include all routes in the bundle.
if (mode === 'app-dashboard') {
  routes = (await import('./dashboards/app-dashboard/index.ts')).routes;
} else if (mode === 'company-dashboard') {
  routes = (await import('./dashboards/company-dashboard/index.ts')).routes;
} else {
  routes = (await import('./dashboards/admin-dashboard/index.ts')).routes;
}

export { routes };
