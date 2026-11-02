#!/usr/bin/env bash
# Crea lo store col modello (corsi.fga) e salva l'id in /state/env, letto da toolbox e backend.
set -euo pipefail
cd "$(dirname "$0")/.."

out=$(fga store create --name corsi --model fga/corsi.fga)
store_id=$(printf '%s' "$out" | jq -r '.store.id')
[ -n "$store_id" ] && [ "$store_id" != null ] || { echo "store non creato:"; echo "$out"; exit 1; }

printf 'export FGA_STORE_ID=%s\n' "$store_id" > /state/env
echo "store $store_id creato, modello scritto"
