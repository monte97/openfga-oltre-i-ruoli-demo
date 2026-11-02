#!/usr/bin/env bash
# Genera un JWT HS256 di sviluppo.
# Uso: ./scripts/token.sh <utente> [ruolo ...]     es. ./scripts/token.sh anna student
# La chiave e' quella di sviluppo dichiarata nel docker-compose.yml: non e' un segreto.
set -euo pipefail

user="${1:?uso: token.sh <utente> [ruolo ...]}"
shift
KEY="${JWT_DEVKEY:-dev-only-hs256-key-not-a-secret-0123456789}"

b64url() { openssl base64 -A | tr '+/' '-_' | tr -d '='; }

roles=""
for r in "$@"; do roles="${roles:+$roles,}\"$r\""; done

header='{"alg":"HS256","typ":"JWT"}'
exp=$(( $(date +%s) + 3600 ))
payload=$(printf '{"sub":"%s","preferred_username":"%s","realm_access":{"roles":[%s]},"exp":%s}' "$user" "$user" "$roles" "$exp")

h=$(printf '%s' "$header" | b64url)
p=$(printf '%s' "$payload" | b64url)
s=$(printf '%s.%s' "$h" "$p" | openssl dgst -sha256 -hmac "$KEY" -binary | b64url)
printf '%s.%s.%s\n' "$h" "$p" "$s"
