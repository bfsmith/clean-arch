.PHONY: help docker-up docker-down docker-restart

# Default target
help:
	@echo "Available commands:"
	@echo "  make docker-up      - Start Docker services (Keycloak, Aspire)"
	@echo "  make docker-down    - Stop Docker services"
	@echo "  make docker-restart - Restart Docker services"
	@echo ""
	@echo "For backend commands, run 'make' from the backend directory"

# Docker commands
docker-up:
	docker compose up -d

docker-down:
	docker compose down

docker-restart: docker-down docker-up

