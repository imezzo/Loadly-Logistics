# syntax=docker/dockerfile:1.7
FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
# Railway maps service variables into Docker ARGs when declared.
ARG VITE_API_BASE_URL
ARG VITE_BRAND_NAME=Loadly
ARG VITE_DEFAULT_LANGUAGE=en
ARG VITE_API_BASE_MODE=admin
ARG VITE_PRODUCTION=true
ARG VITE_GOOGLE_MAPS_API_KEY=
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL \
    VITE_BRAND_NAME=$VITE_BRAND_NAME \
    VITE_DEFAULT_LANGUAGE=$VITE_DEFAULT_LANGUAGE \
    VITE_API_BASE_MODE=$VITE_API_BASE_MODE \
    VITE_PRODUCTION=$VITE_PRODUCTION \
    VITE_GOOGLE_MAPS_API_KEY=$VITE_GOOGLE_MAPS_API_KEY \
    VITE_PORT=80
RUN test -n "$VITE_API_BASE_URL" || (echo "VITE_API_BASE_URL required at build" && exit 1)
RUN npm run build:admin

FROM nginx:1.27-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh
COPY --from=build /app/dist/admin-dashboard /usr/share/nginx/html
ENV PORT=8080
EXPOSE 8080
CMD ["/docker-entrypoint.sh"]
