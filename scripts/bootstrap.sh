#!/usr/bin/env bash
# Crea lo store e scrive il modello (corsi.fga), poi salva l'indirizzo e l'id dello store in .env.
set -euo pipefail
cd "$(dirname "$0")/.."

export FGA_API_URL="http://localhost:8180"
out=$(fga store create --name corsi --model fga/corsi.fga)
store_id=$(printf '%s' "$out" | tr -d '\n ' | grep -o '"store":{[^}]*' | grep -o '"id":"[^"]*"' | head -1 | cut -d'"' -f4)
[ -n "$store_id" ] || { echo "store non creato:"; echo "$out"; exit 1; }

printf 'FGA_API_URL=%s\nFGA_STORE_ID=%s\n' "$FGA_API_URL" "$store_id" > .env
echo "store $store_id creato, modello scritto, .env aggiornato"
