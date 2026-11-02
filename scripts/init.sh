#!/usr/bin/env bash
# Servizio one-shot del compose: aspetta OpenFGA, crea store e modello, scrive le tuple.
# Se lo store esiste gia' (per esempio dopo un `docker compose up -d backend`) non fa niente.
set -euo pipefail
cd "$(dirname "$0")/.."

until curl -sf "$FGA_API_URL/healthz" >/dev/null; do sleep 1; done

if [ -f /state/env ]; then
  . /state/env
  if fga store get >/dev/null 2>&1; then
    echo "store $FGA_STORE_ID gia' presente: niente da fare"
    exit 0
  fi
fi

./scripts/bootstrap.sh
./scripts/seed.sh
