#!/bin/sh
# Carica l'id dello store scritto dal servizio init, poi lancia la CLI.
[ -z "$FGA_STORE_ID" ] && [ -f /state/env ] && . /state/env
exec /usr/local/bin/fga-bin "$@"
