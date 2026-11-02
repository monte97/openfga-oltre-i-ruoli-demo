# Output reali

Ogni blocco e' l'output di un'esecuzione vera, da zero, il 2026-11-02 (OpenFGA v1.22.1, fga version v`0.7.19` (commit: `cd511b8b9bb697066f053eac3446a73e81741a54`, date: `2026-07-13T18:59:54Z`)). Le date di ReadChanges sono quelle del seed di questa esecuzione.

## Avvio da zero

```bash
$ make reset 2>&1 | tail -3
 Network openfga-oltre-i-ruoli_default Removing 
 Network openfga-oltre-i-ruoli_default Removed 
rm -f .env
```

```bash
$ make up 2>&1 | grep -v -E '^#|^$' | tail -5
 Container openfga-oltre-i-ruoli-openfga-1 Running 
 Container openfga-oltre-i-ruoli-backend-1 Creating 
 Container openfga-oltre-i-ruoli-backend-1 Created 
 Container openfga-oltre-i-ruoli-backend-1 Starting 
 Container openfga-oltre-i-ruoli-backend-1 Started 
```

```bash
$ make seed
./scripts/seed.sh
scritta: user:diego assistant course:42
scritta: user:elena assistant course:42
scritta: user:luca assistant course:42
scritta: user:anna member org:acme
scritta: user:giulia member org:acme
scritta: user:paolo member org:acme
scritta: org:acme#member student course:42
scritta: user:paolo blocked course:42
scritta: user:mario author course:42
scritta: user:marta author course:43
scritta: user:mario student course:43
```

```bash
$ make test
fga model test --tests fga/corsi.fga.yaml
# Test Summary #
Tests 6/6 passing
Checks 12/12 passing
```

## Anagrafica

```bash
$ cat anagrafica.csv
utente,chi_e,fonte,stato
anna,dipendente Acme,HR Acme,attiva
giulia,dipendente Acme,HR Acme,attiva
paolo,dipendente Acme,HR Acme,sospeso dal corso 42
mario,autore del corso 42,fornitore,attivo
diego,assistente del corso 42,fornitore,contratto finito a giugno
elena,assistente del corso 42,fornitore,uscita a maggio
luca,assistente del corso 42,fornitore,fino al 30 giugno
```

## Chi puo' vederlo, adesso

```bash
$ fga query list-users --object course:42 --relation viewer --user-filter user --context "{\"current_time\":\"$NOW\"}"
{"users":[{"object":{"id":"elena","type":"user"}},{"object":{"id":"diego","type":"user"}},{"object":{"id":"mario","type":"user"}},{"object":{"id":"anna","type":"user"}},{"object":{"id":"giulia","type":"user"}}]}
```

```bash
$ fga query list-users --object course:42 --relation viewer --user-filter user
Error: failed to list users due to POST validation error for ListUsers POST with body {"code":"validation_error","message":"failed to evaluate relationship condition: 'non_expired' - tuple 'course:42#assistant@user:luca' is missing context parameters '[current_time]'"}
 with error code validation_error error message: failed to evaluate relationship condition: 'non_expired' - tuple 'course:42#assistant@user:luca' is missing context parameters '[current_time]'
```

## Da dove arriva l'accesso

```bash
$ fga query expand viewer course:42
{"tree":{"root":{"difference":{"base":{"name":"course:42#viewer","union":{"nodes":[{"leaf":{"computed":{"userset":"course:42#student"}},"name":"course:42#viewer"},{"leaf":{"computed":{"userset":"course:42#editor"}},"name":"course:42#viewer"}]}},"subtract":{"leaf":{"computed":{"userset":"course:42#blocked"}},"name":"course:42#viewer"}},"name":"course:42#viewer"}}}
```

```bash
$ fga query expand editor course:42
{"tree":{"root":{"name":"course:42#editor","union":{"nodes":[{"leaf":{"computed":{"userset":"course:42#author"}},"name":"course:42#editor"},{"leaf":{"computed":{"userset":"course:42#assistant"}},"name":"course:42#editor"}]}}}}
```

```bash
$ fga query expand assistant course:42
{"tree":{"root":{"leaf":{"users":{"users":["user:diego","user:elena","user:luca"]}},"name":"course:42#assistant"}}}
```

```bash
$ fga query expand student course:42
{"tree":{"root":{"leaf":{"users":{"users":["org:acme#member"]}},"name":"course:42#student"}}}
```

```bash
$ fga tuple read --object course:42 --relation assistant
{
  "continuation_token":"",
  "tuples": [
    {
      "key": {
        "object":"course:42",
        "relation":"assistant",
        "user":"user:diego"
      },
      "timestamp":"2026-11-02T15:24:58.348100671Z"
    },
    {
      "key": {
        "object":"course:42",
        "relation":"assistant",
        "user":"user:elena"
      },
      "timestamp":"2026-11-02T15:24:58.366257962Z"
    },
    {
      "key": {
        "condition": {
          "context": {
            "valid_until":"2026-06-30T23:59:59Z"
          },
          "name":"non_expired"
        },
        "object":"course:42",
        "relation":"assistant",
        "user":"user:luca"
      },
      "timestamp":"2026-11-02T15:24:58.382823462Z"
    }
  ]
}
```

```bash
$ fga query check user:luca viewer course:42 --context "{\"current_time\":\"$NOW\"}"
{"allowed":false,"resolution":""}
```

```bash
$ fga query check user:diego viewer course:42 --context "{\"current_time\":\"$NOW\"}"
{"allowed":true,"resolution":""}
```

```bash
$ fga query check user:elena viewer course:42 --context "{\"current_time\":\"$NOW\"}"
{"allowed":true,"resolution":""}
```

## Da quando

```bash
$ fga tuple changes --type course
{"changes":[{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T15:24:58.348100671Z","tuple_key":{"object":"course:42","relation":"assistant","user":"user:diego"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T15:24:58.366257962Z","tuple_key":{"object":"course:42","relation":"assistant","user":"user:elena"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T15:24:58.382823462Z","tuple_key":{"condition":{"context":{"valid_until":"2026-06-30T23:59:59Z"},"name":"non_expired"},"object":"course:42","relation":"assistant","user":"user:luca"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T15:24:58.445118171Z","tuple_key":{"object":"course:42","relation":"student","user":"org:acme#member"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T15:24:58.461844463Z","tuple_key":{"object":"course:42","relation":"blocked","user":"user:paolo"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T15:24:58.476169754Z","tuple_key":{"object":"course:42","relation":"author","user":"user:mario"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T15:24:58.490966338Z","tuple_key":{"object":"course:43","relation":"author","user":"user:marta"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T15:24:58.505219004Z","tuple_key":{"object":"course:43","relation":"student","user":"user:mario"}}],"continuation_token":"MDFNNkVEVDZSOUE3RVdZVjg0MU5QQ1JHMlB8Y291cnNl"}
```

## Chiudere e verificare

```bash
$ fga tuple delete user:diego assistant course:42
{}
```

```bash
$ fga tuple delete user:elena assistant course:42
{}
```

```bash
$ fga query check user:diego viewer course:42 --context "{\"current_time\":\"$NOW\"}"
{"allowed":false,"resolution":""}
```

```bash
$ fga query check user:elena viewer course:42 --context "{\"current_time\":\"$NOW\"}"
{"allowed":false,"resolution":""}
```

```bash
$ fga query list-users --object course:42 --relation viewer --user-filter user --context "{\"current_time\":\"$NOW\"}"
{"users":[{"object":{"id":"giulia","type":"user"}},{"object":{"id":"mario","type":"user"}},{"object":{"id":"anna","type":"user"}}]}
```

## Correggere il modello

```bash
$ git checkout soluzione -- fga/corsi.fga && git diff --cached -U1 fga/corsi.fga | tail -8
index 7ae9424..fbcd3cc 100644
--- a/fga/corsi.fga
+++ b/fga/corsi.fga
@@ -13,3 +13,3 @@ type course
     define author: [user]
-    define assistant: [user, user with non_expired]
+    define assistant: [user with non_expired]
     define student: [user, org#member]
```

```bash
$ fga model write --file fga/corsi.fga
{"authorization_model_id":"01M6EDT7451VKH9B8RRRWGPV00"}
```

```bash
$ fga tuple write user:sara assistant course:42
Error: failed to write tuple: POST validation error for Write POST with body {"code":"validation_error","message":"Invalid tuple 'course:42#assistant@user:sara'. Reason: condition is missing"}
 with error code validation_error error message: Invalid tuple 'course:42#assistant@user:sara'. Reason: condition is missing
```

```bash
$ fga tuple write user:sara assistant course:42 --condition-name non_expired --condition-context '{"valid_until":"2026-12-31T23:59:59Z"}'
{"successful":[{"condition":{"context":{"valid_until":"2026-12-31T23:59:59Z"},"name":"non_expired"},"object":"course:42","relation":"assistant","user":"user:sara"}]}
```

```bash
$ make test
fga model test --tests fga/corsi.fga.yaml
# Test Summary #
Tests 6/6 passing
Checks 12/12 passing
```

```bash
$ git reset -q HEAD fga/corsi.fga && git checkout -q main -- fga/corsi.fga; git status --short
?? docs/output-reali.md
```

## La PR che rompe il modello

```bash
$ git checkout -q esempio-pr-rotta && make test; git checkout -q main
fga model test --tests fga/corsi.fga.yaml
(FAILING) Anna vede il corso 42 e non lo modifica: Checks (2/3 passing)
ⅹ Check(user=user:anna,relation=editor,object=course:42, context:&map[current_time:2026-11-02T10:00:00Z]): expected=false, got=true
---
# Test Summary #
Tests 5/6 passing
Checks 11/12 passing
make: *** [test] Error 1
```

## Il backend: shadow, con RBAC che decide

```bash
$ curl -s localhost:5180/; echo
elearning-api: demo RBAC->ReBAC, fonte di verita': rbac
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" localhost:5180/courses/42
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" localhost:5180/courses/43
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -X PUT -H "Authorization: Bearer $(./scripts/token.sh mario author)" localhost:5180/courses/42
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -X PUT -H "Authorization: Bearer $(./scripts/token.sh mario author)" localhost:5180/courses/43
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh giulia)" localhost:5180/courses/42
403
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' localhost:5180/courses/42
401
```

```bash
$ docker compose logs backend --no-log-prefix | grep -B1 mismatch | grep -v -- '^--'
warn: ElearningApi.Authz.DualCheckAuthorizer[0]
      dual-check mismatch: user:anna viewer course:43 -> rbac=True openfga=False
warn: ElearningApi.Authz.DualCheckAuthorizer[0]
      dual-check mismatch: user:mario editor course:43 -> rbac=True openfga=False
warn: ElearningApi.Authz.DualCheckAuthorizer[0]
      dual-check mismatch: user:giulia viewer course:42 -> rbac=False openfga=True
```

## Cutover

```bash
$ AUTHZ__SOURCEOFTRUTH=openfga docker compose up -d backend 2>&1 | tail -2
 Container openfga-oltre-i-ruoli-backend-1 Starting 
 Container openfga-oltre-i-ruoli-backend-1 Started 
```

```bash
$ curl -s localhost:5180/; echo
elearning-api: demo RBAC->ReBAC, fonte di verita': openfga
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" localhost:5180/courses/42
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" localhost:5180/courses/43
403
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -X PUT -H "Authorization: Bearer $(./scripts/token.sh mario author)" localhost:5180/courses/43
403
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh giulia)" localhost:5180/courses/42
200
```

## Scostamenti rispetto all'articolo 06 (comportamento reale di OpenFGA v1.22.1)

- `expand viewer` restituisce un solo livello: `student`, `editor` e `blocked` compaiono come usersets calcolati, non espansi. Per vedere Luca, Diego ed Elena serve un expand separato su `assistant`; per i membri di Acme, `org:acme#member` resta uno userset (Anna, Giulia e Paolo si leggono con `fga tuple read` sul tipo `org`).
- `list-users` senza `current_time` nel contesto non include Luca: restituisce un errore di validazione per tutta la richiesta.
- L'ordine degli utenti in `list-users` cambia da una chiamata all'altra.
- `fga tuple changes --type course` restituisce tutte le tuple del tipo `course` (anche `student`, `blocked`, `author` e il corso 43), non solo gli assistenti.
