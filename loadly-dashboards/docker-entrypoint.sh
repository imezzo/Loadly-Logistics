#!/bin/sh
set -e
PORT="${PORT:-8080}"
sed -i "s/listen [0-9]\\+/listen ${PORT}/" /etc/nginx/conf.d/default.conf
exec nginx -g "daemon off;"
