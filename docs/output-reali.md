# Output reali

Ogni blocco e' l'output di un'esecuzione vera, da zero, il 2026-11-02 (`docker compose down -v`, poi `docker compose up -d --build`; OpenFGA v1.22.1, fga v0.7.19 nel toolbox). Tutti i comandi `fga` e `curl` sono lanciati nel toolbox, tranne dove e' indicato `# host`. Le date di ReadChanges sono quelle del seed di questa esecuzione.

## Test del modello (toolbox)

```bash
$ fga model test --tests fga/corsi.fga.yaml
# Test Summary #
Tests 6/6 passing
Checks 12/12 passing
```

## Chi puo' vederlo, adesso (toolbox)

```bash
$ fga query list-users --object course:42 --relation viewer --user-filter user --context "{\"current_time\":\"$NOW\"}"
{"users":[{"object":{"id":"diego","type":"user"}},{"object":{"id":"elena","type":"user"}},{"object":{"id":"mario","type":"user"}},{"object":{"id":"anna","type":"user"}},{"object":{"id":"giulia","type":"user"}}]}
```

```bash
$ fga query list-users --object course:42 --relation viewer --user-filter user
Error: failed to list users due to POST validation error for ListUsers POST with body {"code":"validation_error","message":"failed to evaluate relationship condition: 'non_expired' - tuple 'course:42#assistant@user:luca' is missing context parameters '[current_time]'"}
 with error code validation_error error message: failed to evaluate relationship condition: 'non_expired' - tuple 'course:42#assistant@user:luca' is missing context parameters '[current_time]'
```

## Anagrafica (toolbox)

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

## Da dove arriva l'accesso (toolbox)

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
      "timestamp":"2026-11-02T16:25:49.97022125Z"
    },
    {
      "key": {
        "object":"course:42",
        "relation":"assistant",
        "user":"user:elena"
      },
      "timestamp":"2026-11-02T16:25:49.984516791Z"
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
      "timestamp":"2026-11-02T16:25:50.004316375Z"
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

## Da quando (toolbox)

```bash
$ fga tuple changes --type course
{"changes":[{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T16:25:49.97022125Z","tuple_key":{"object":"course:42","relation":"assistant","user":"user:diego"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T16:25:49.984516791Z","tuple_key":{"object":"course:42","relation":"assistant","user":"user:elena"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T16:25:50.004316375Z","tuple_key":{"condition":{"context":{"valid_until":"2026-06-30T23:59:59Z"},"name":"non_expired"},"object":"course:42","relation":"assistant","user":"user:luca"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T16:25:50.0851825Z","tuple_key":{"object":"course:42","relation":"student","user":"org:acme#member"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T16:25:50.119239208Z","tuple_key":{"object":"course:42","relation":"blocked","user":"user:paolo"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T16:25:50.136455291Z","tuple_key":{"object":"course:42","relation":"author","user":"user:mario"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T16:25:50.148437458Z","tuple_key":{"object":"course:43","relation":"author","user":"user:marta"}},{"operation":"TUPLE_OPERATION_WRITE","timestamp":"2026-11-02T16:25:50.162809708Z","tuple_key":{"object":"course:43","relation":"student","user":"user:mario"}}],"continuation_token":"MDFNNkVIOU1USlZNMTUxUjJUWFBXNDdXMTN8Y291cnNl"}
```

## Chiudere e verificare (toolbox)

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
{"users":[{"object":{"id":"giulia","type":"user"}},{"object":{"id":"anna","type":"user"}},{"object":{"id":"mario","type":"user"}}]}
```

## Correggere il modello

```bash
# host
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
# toolbox
$ fga model write --file fga/corsi.fga
{"authorization_model_id":"01M6EH9Y1M23MFXR4R4TK11KM9"}
```

```bash
# toolbox
$ fga tuple write user:sara assistant course:42
Error: failed to write tuple: POST validation error for Write POST with body {"code":"validation_error","message":"Invalid tuple 'course:42#assistant@user:sara'. Reason: condition is missing"}
 with error code validation_error error message: Invalid tuple 'course:42#assistant@user:sara'. Reason: condition is missing
```

```bash
# toolbox
$ fga tuple write user:sara assistant course:42 --condition-name non_expired --condition-context '{"valid_until":"2026-12-31T23:59:59Z"}'
{"successful":[{"condition":{"context":{"valid_until":"2026-12-31T23:59:59Z"},"name":"non_expired"},"object":"course:42","relation":"assistant","user":"user:sara"}]}
```

```bash
# toolbox
$ fga model test --tests fga/corsi.fga.yaml
# Test Summary #
Tests 6/6 passing
Checks 12/12 passing
```

## La PR che rompe il modello

```bash
# host
$ git restore --staged --worktree fga/corsi.fga
$ git checkout -q esempio-pr-rotta
```

```bash
# toolbox
$ fga model test --tests fga/corsi.fga.yaml
(FAILING) Anna vede il corso 42 e non lo modifica: Checks (2/3 passing)
ⅹ Check(user=user:anna,relation=editor,object=course:42, context:&map[current_time:2026-11-02T10:00:00Z]): expected=false, got=true
---
# Test Summary #
Tests 5/6 passing
Checks 11/12 passing
exit=1
```

```bash
# host
$ git checkout -q main
```

## Il backend: shadow, con RBAC che decide (toolbox)

```bash
$ curl -s backend:8080/; echo
elearning-api: demo RBAC->ReBAC, fonte di verita': rbac
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" backend:8080/courses/42
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" backend:8080/courses/43
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -X PUT -H "Authorization: Bearer $(./scripts/token.sh mario author)" backend:8080/courses/42
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -X PUT -H "Authorization: Bearer $(./scripts/token.sh mario author)" backend:8080/courses/43
200
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh giulia)" backend:8080/courses/42
403
```

```bash
$ curl -s -o /dev/null -w '%{http_code}\n' backend:8080/courses/42
401
```

```bash
# host
warn: ElearningApi.Authz.DualCheckAuthorizer[0]
      dual-check mismatch: user:anna viewer course:43 -> rbac=True openfga=False
warn: ElearningApi.Authz.DualCheckAuthorizer[0]
      dual-check mismatch: user:mario editor course:43 -> rbac=True openfga=False
warn: ElearningApi.Authz.DualCheckAuthorizer[0]
      dual-check mismatch: user:giulia viewer course:42 -> rbac=False openfga=True
```

## Cutover

```bash
# host
$ AUTHZ__SOURCEOFTRUTH=openfga docker compose up -d backend
```

```bash
# toolbox
$ curl -s backend:8080/; echo
elearning-api: demo RBAC->ReBAC, fonte di verita': openfga
```

```bash
# toolbox
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" backend:8080/courses/42
200
```

```bash
# toolbox
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" backend:8080/courses/43
403
```

```bash
# toolbox
$ curl -s -o /dev/null -w '%{http_code}\n' -X PUT -H "Authorization: Bearer $(./scripts/token.sh mario author)" backend:8080/courses/43
403
```

```bash
# toolbox
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh giulia)" backend:8080/courses/42
200
```

```bash
# toolbox
$ curl -s -o /dev/null -w '%{http_code}\n' -H "Authorization: Bearer $(./scripts/token.sh anna student)" backend:8080/courses/42
200
```

## Scostamenti rispetto all'articolo 06 (comportamento reale di OpenFGA v1.22.1)

- `expand viewer` restituisce un solo livello: `student`, `editor` e `blocked` compaiono come usersets calcolati, non espansi. Per vedere Luca, Diego ed Elena serve un expand separato su `assistant`; per i membri di Acme, `org:acme#member` resta uno userset (Anna, Giulia e Paolo si leggono con `fga tuple read` sul tipo `org`).
- `list-users` senza `current_time` nel contesto non include Luca: restituisce un errore di validazione per tutta la richiesta.
- L'ordine degli utenti in `list-users` cambia da una chiamata all'altra.
- `fga tuple changes --type course` restituisce tutte le tuple del tipo `course` (anche `student`, `blocked`, `author` e il corso 43), non solo gli assistenti.
