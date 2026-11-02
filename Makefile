# Scorciatoie facoltative per chi ha make sull'host: con solo Docker bastano i comandi `docker compose`.
.PHONY: up shell test down reset logs

up:     ## Avvia tutto: OpenFGA, store, modello, tuple, backend, toolbox
	docker compose up -d --build

shell:  ## Entra nel toolbox
	docker compose exec toolbox sh

test:   ## Test del modello (non serve lo stack)
	docker compose run --rm --no-deps toolbox fga model test --tests fga/corsi.fga.yaml

down:   ## Ferma lo stack
	docker compose down

reset:  ## Ferma lo stack e cancella i dati
	docker compose down -v --remove-orphans

logs:   ## Segue i log del backend
	docker compose logs -f backend
