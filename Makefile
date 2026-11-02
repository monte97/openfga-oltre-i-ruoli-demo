.PHONY: up seed test down reset logs

up:     ## Avvia OpenFGA, crea lo store col modello, avvia il backend
	docker compose up -d openfga
	@until curl -sf http://localhost:8180/healthz >/dev/null; do sleep 1; done
	./scripts/bootstrap.sh
	docker compose up -d --build backend

seed:   ## Scrive le tuple dello scenario, nell'ordine della storia
	./scripts/seed.sh

test:   ## Test del modello (non serve lo stack)
	fga model test --tests fga/corsi.fga.yaml

down:   ## Ferma lo stack
	docker compose down

reset:  ## Ferma lo stack, cancella i dati e .env
	docker compose down -v --remove-orphans
	rm -f .env

logs:   ## Segue i log del backend
	docker compose logs -f backend
