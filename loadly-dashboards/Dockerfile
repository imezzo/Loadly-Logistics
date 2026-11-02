# syntax=docker/dockerfile:1.7
FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
# Build-time API URL injected by Railway as build arg / env
ARG VITE_API_BASE_URL=https://api.example.com/api
ARG VITE_BRAND_NAME=Loadly
ARG VITE_DEFAULT_LANGUAGE=en
ARG VITE_API_BASE_MODE=admin
ARG VITE_PRODUCTION=true
ARG VITE_PORT=80
ARG VITE_GOOGLE_MAPS_API_KEY=
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL \
    VITE_BRAND_NAME=$VITE_BRAND_NAME \
    VITE_DEFAULT_LANGUAGE=$VITE_DEFAULT_LANGUAGE \
    VITE_API_BASE_MODE=$VITE_API_BASE_MODE \
    VITE_PRODUCTION=$VITE_PRODUCTION \
    VITE_PORT=$VITE_PORT \
    VITE_GOOGLE_MAPS_API_KEY=$VITE_GOOGLE_MAPS_API_KEY
RUN npm run build:admin

FROM nginx:1.27-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/admin-dashboard /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
