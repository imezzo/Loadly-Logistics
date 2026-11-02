# syntax=docker/dockerfile:1.7
FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
# Railway injects service variables as build-time env.
ENV VITE_BRAND_NAME=Loadly \
    VITE_DEFAULT_LANGUAGE=en \
    VITE_API_BASE_MODE=admin \
    VITE_PRODUCTION=true \
    VITE_PORT=80
RUN test -n "$VITE_API_BASE_URL" || (echo "VITE_API_BASE_URL required at build" && exit 1)
RUN npm run build:admin

FROM nginx:1.27-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/admin-dashboard /usr/share/nginx/html
ENV PORT=8080
EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]
