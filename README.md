# Oltre i ruoli: la demo

Codice dell'articolo «Oltre i ruoli, con le mani sulla tastiera» sul blog di montelli.dev, che rifà passo per passo la storia dell'articolo [«Oltre i ruoli: un accesso di troppo, trovato in tre passi»](https://montelli.dev/blog/verificare/openfga/06-oltre-i-ruoli/): una piattaforma di corsi, due assistenti che hanno ancora accesso quando non dovrebbero, un modello OpenFGA da correggere e un backend .NET 8 da migrare dai ruoli alle relazioni.

## Cosa serve

- Docker con Compose
- git, per clonare il repo

Niente CLI `fga` da installare, niente Keycloak, Python o .NET: la CLI sta nel container `toolbox`, il backend si compila nel container. Chi ha già la CLI sull'host può usarla sulla porta 8180 (`FGA_API_URL=http://localhost:8180`).

## Avvio

Due terminali: «host» è il terminale della macchina, dove si lanciano `git` e `docker compose`; «toolbox» è la shell dentro il container, dove si lanciano `fga`, `curl` e gli script.

```bash
# host
docker compose up -d --build   # OpenFGA v1.22.1, store, modello, tuple, backend, toolbox
docker compose exec toolbox sh # entra nel toolbox
```

Dentro il toolbox la cartella del repo è `/work`, `fga` parla con `openfga:8080` e `FGA_STORE_ID` è già impostato:

```bash
# toolbox
fga model test --tests fga/corsi.fga.yaml
fga query check user:anna viewer course:42 --context '{"current_time":"2026-11-02T10:00:00Z"}'
curl -s backend:8080/
```

Il servizio `init` crea lo store con il modello e scrive le tuple dello scenario nell'ordine della storia. I cambi di branch (`git checkout`) si fanno sull'host: il repo è montato nel toolbox, quindi il toolbox vede subito i file.

Porte sull'host, facoltative: OpenFGA 8180, backend 5180. `make` è una scorciatoia facoltativa (`make up`, `make shell`, `make test`, `make down`, `make reset`).

## Cosa c'è dentro

| Percorso | Contenuto |
|---|---|
| `fga/corsi.fga` | il modello dell'articolo, prima della correzione |
| `fga/corsi.fga.yaml` | i test del modello (`fga model test --tests fga/corsi.fga.yaml`) |
| `anagrafica.csv` | chi dovrebbe avere accesso al corso 42 secondo HR e fornitore |
| `toolbox/` | il Dockerfile del toolbox: Alpine con `fga`, `curl`, `jq`, `openssl`, `make` |
| `scripts/init.sh` | il servizio `init`: aspetta OpenFGA, crea lo store e scrive le tuple (se lo store c'è già, non fa niente) |
| `scripts/seed.sh` | le tuple, scritte una alla volta nell'ordine della storia |
| `scripts/token.sh` | genera un JWT HS256 di sviluppo, nel toolbox: `./scripts/token.sh anna student` |
| `backend/` | API .NET 8 con `IAuthorizer`: `RbacAuthorizer`, `OpenFgaAuthorizer`, `DualCheckAuthorizer` |
| `.github/workflows/model-test.yml` | `fga model test` a ogni pull request |

Branch:

- `main`: lo stato di partenza, quello dell'articolo prima della correzione.
- `soluzione`: il modello corretto (`assistant: [user with non_expired]`).
- `esempio-pr-rotta`: una modifica al modello che rende Anna `editor` e fa fallire `fga model test`.

## Il backend

Due endpoint: `GET /courses/{id}` (serve la relazione `viewer`) e `PUT /courses/{id}` (serve `editor`). La fonte della decisione si sceglie con la variabile `AUTHZ__SOURCEOFTRUTH`:

- `rbac` (default): decide il vecchio controllo sui ruoli, OpenFGA risponde in parallelo e ogni disaccordo finisce nel log (`docker compose logs backend`).
- `openfga`: decide OpenFGA.

```bash
# toolbox
TOKEN=$(./scripts/token.sh anna student)
curl -i -H "Authorization: Bearer $TOKEN" backend:8080/courses/43
```

```bash
# host
AUTHZ__SOURCEOFTRUTH=openfga docker compose up -d backend
```

```bash
# toolbox
curl -i -H "Authorization: Bearer $TOKEN" backend:8080/courses/43
```

## Date di ReadChanges

OpenFGA registra la data di ogni scrittura nel momento in cui avviene. Il registro che vedi con `fga tuple changes` ha l'ordine dell'articolo e le date del tuo avvio.

## Chiave di sviluppo

I token sono firmati con la chiave HS256 `dev-only-hs256-key-not-a-secret-0123456789`, scritta in chiaro nel `docker-compose.yml` e in `scripts/token.sh`. Serve solo a questa demo, non è un segreto. In produzione il token lo emette l'identity provider e il backend ne verifica la firma con la sua chiave pubblica. Nel repo non ci sono altre credenziali.

## Pulizia

```bash
# host
docker compose down      # ferma lo stack
docker compose down -v   # ferma, cancella lo store id e ricomincia da zero al prossimo up
```
