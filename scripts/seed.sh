#!/usr/bin/env bash
# Scrive le tuple nell'ordine della storia, una scrittura per volta.
# Cosi' ReadChanges le restituisce nello stesso ordine.
set -euo pipefail
cd "$(dirname "$0")/.."
set -a; . ./.env; set +a

w() { fga tuple write "$@" >/dev/null; echo "scritta: $1 $2 $3"; }

# Febbraio: Diego ed Elena diventano assistenti del corso 42, senza scadenza
w user:diego assistant course:42
w user:elena assistant course:42

# Aprile: Luca entra con una scadenza
w user:luca assistant course:42 --condition-name non_expired --condition-context '{"valid_until":"2026-06-30T23:59:59Z"}'

# Acme compra il corso 42 per tutti i suoi dipendenti
w user:anna member org:acme
w user:giulia member org:acme
w user:paolo member org:acme
w org:acme#member student course:42

# Paolo e' sospeso dal corso
w user:paolo blocked course:42

# Mario e' l'autore del corso 42
w user:mario author course:42

# Corso 43 (altro fornitore, non comprato da Acme): serve ai mismatch del backend.
# Mario qui e' solo studente.
w user:marta author course:43
w user:mario student course:43
