# Oltre i ruoli: la demo

Codice dell'articolo «Oltre i ruoli, con le mani sulla tastiera» sul blog di montelli.dev, che rifà passo per passo la storia dell'articolo [«Oltre i ruoli: un accesso di troppo, trovato in tre passi»](https://montelli.dev/blog/verificare/openfga/06-oltre-i-ruoli/): una piattaforma di corsi, due assistenti che hanno ancora accesso quando non dovrebbero, un modello OpenFGA da correggere e un backend .NET 8 da migrare dai ruoli alle relazioni.

## Cosa serve

- Docker con Compose
- la CLI [`fga`](https://github.com/openfga/cli) (testata con la v0.7.19)
- `make`, `curl` e `openssl` (ci sono già su macOS e Linux)

Niente Keycloak, niente Python, niente .NET installato: il backend si compila nel container.

## Avvio

```bash
make up      # OpenFGA v1.22.1, store e modello, backend sulla 5180
make seed    # le tuple dello scenario, nell'ordine della storia
make test    # fga model test, non serve lo stack
```

`make up` scrive in `.env` l'indirizzo e l'id dello store. Per usare la CLI `fga` negli esempi dell'articolo:

```bash
set -a; . ./.env; set +a
```

Porte sull'host: OpenFGA 8180, backend 5180.

## Cosa c'è dentro

| Percorso | Contenuto |
|---|---|
| `fga/corsi.fga` | il modello dell'articolo, prima della correzione |
| `fga/corsi.fga.yaml` | i test del modello (`make test`) |
| `anagrafica.csv` | chi dovrebbe avere accesso al corso 42 secondo HR e fornitore |
| `scripts/seed.sh` | le tuple, scritte una alla volta nell'ordine della storia |
| `scripts/token.sh` | genera un JWT HS256 di sviluppo: `./scripts/token.sh anna student` |
| `backend/` | API .NET 8 con `IAuthorizer`: `RbacAuthorizer`, `OpenFgaAuthorizer`, `DualCheckAuthorizer` |
| `.github/workflows/model-test.yml` | `fga model test` a ogni pull request |

Branch:

- `main`: lo stato di partenza, quello dell'articolo prima della correzione.
- `soluzione`: il modello corretto (`assistant: [user with non_expired]`).
- `esempio-pr-rotta`: una modifica al modello che rende Anna `editor` e fa fallire `make test`.

## Il backend

Due endpoint: `GET /courses/{id}` (serve la relazione `viewer`) e `PUT /courses/{id}` (serve `editor`). La fonte della decisione si sceglie con la variabile `AUTHZ__SOURCEOFTRUTH`:

- `rbac` (default): decide il vecchio controllo sui ruoli, OpenFGA risponde in parallelo e ogni disaccordo finisce nel log (`docker compose logs backend`).
- `openfga`: decide OpenFGA.

```bash
TOKEN=$(./scripts/token.sh anna student)
curl -i -H "Authorization: Bearer $TOKEN" localhost:5180/courses/43

AUTHZ__SOURCEOFTRUTH=openfga docker compose up -d backend
curl -i -H "Authorization: Bearer $TOKEN" localhost:5180/courses/43
```

## Date di ReadChanges

OpenFGA registra la data di ogni scrittura nel momento in cui avviene. Il registro che vedi con `fga tuple changes` ha l'ordine dell'articolo e le date del tuo seed.

## Chiave di sviluppo

I token sono firmati con la chiave HS256 `dev-only-hs256-key-not-a-secret-0123456789`, scritta in chiaro nel `docker-compose.yml` e in `scripts/token.sh`. Serve solo a questa demo, non è un segreto. In produzione il token lo emette l'identity provider e il backend ne verifica la firma con la sua chiave pubblica. Nel repo non ci sono altre credenziali.

## Pulizia

```bash
make down    # ferma lo stack
make reset   # ferma, cancella i dati e .env
```
