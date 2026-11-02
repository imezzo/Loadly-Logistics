export enum Platform {
  ADMIN_DASHBOARD = "admin-dashboard",
  APP_DASHBOARD = "app-dashboard",
  COMPANY_DASHBOARD = "company-dashboard",
}

export const isPlatform = (value: string): value is Platform => {
  return Object.values(Platform).includes(value as Platform);
};
