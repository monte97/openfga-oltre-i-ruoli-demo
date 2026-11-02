# Piano di lavoro

Ogni casella e' verificabile con il comando o il file indicato.

- [ ] 1. Struttura del repo, immagine OpenFGA fissata (`docker-compose.yml`, nessun `latest`)
- [ ] 2. `fga/corsi.fga` identico al modello dell'articolo 06
- [ ] 3. `fga/corsi.fga.yaml` e `make test` verde (`fga model test`)
- [ ] 4. `make up` crea store e modello e avvia il backend; `make seed` scrive le tuple nell'ordine della storia
- [ ] 5. `anagrafica.csv` e ListUsers = Anna, Giulia, Mario, Diego, Elena (non Paolo, non Luca)
- [ ] 6. Expand, Read, Check, ReadChanges come nel 06
- [ ] 7. Delete di Diego ed Elena e verifica con Check
- [ ] 8. Modello corretto: write senza scadenza rifiutato, con scadenza accettato (branch `soluzione`)
- [ ] 9. Workflow GitHub Actions con `fga model test`; branch `esempio-pr-rotta` fa fallire il test
- [ ] 10. Backend .NET 8 senza Keycloak: JWT HS256 di sviluppo, `scripts/token.sh`
- [ ] 11. Dual-check: 200 su `GET /courses/43` per Anna e riga di mismatch nel log
- [ ] 12. Cutover con `AUTHZ__SOURCEOFTRUTH=openfga`: stessa richiesta, 403
- [ ] 13. Esecuzione da zero (`make reset`, `make up`, `make seed`, storia completa) e output in `docs/output-reali.md`
- [ ] 14. Controllo segreti
- [ ] 15. README per il lettore
