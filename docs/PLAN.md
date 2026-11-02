# Piano di lavoro

Ogni casella e' verificabile con il comando o il file indicato.

- [x] 1. Struttura del repo, immagine OpenFGA fissata (`docker-compose.yml`, nessun `latest`)
- [x] 2. `fga/corsi.fga` identico al modello dell'articolo 06
- [x] 3. `fga/corsi.fga.yaml` e `make test` verde (`fga model test`)
- [x] 4. `make up` crea store e modello e avvia il backend; `make seed` scrive le tuple nell'ordine della storia
- [x] 5. `anagrafica.csv` e ListUsers = Anna, Giulia, Mario, Diego, Elena (non Paolo, non Luca)
- [x] 6. Expand, Read, Check, ReadChanges come nel 06
- [x] 7. Delete di Diego ed Elena e verifica con Check
- [x] 8. Modello corretto: write senza scadenza rifiutato, con scadenza accettato (branch `soluzione`)
- [x] 9. Workflow GitHub Actions con `fga model test`; branch `esempio-pr-rotta` fa fallire il test
- [x] 10. Backend .NET 8 senza Keycloak: JWT HS256 di sviluppo, `scripts/token.sh`
- [x] 11. Dual-check: 200 su `GET /courses/43` per Anna e riga di mismatch nel log
- [x] 12. Cutover con `AUTHZ__SOURCEOFTRUTH=openfga`: stessa richiesta, 403
- [x] 13. Esecuzione da zero (`make reset`, `make up`, `make seed`, storia completa) e output in `docs/output-reali.md`
- [x] 14. Controllo segreti
- [x] 15. README per il lettore
