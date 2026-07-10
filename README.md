# DS-SNUS

## Distribuirani sistem za senzore 

## Članovi tima:

## SV28/2023 Marko Đorđević

## SV78/2023 Aleksa Nenadović

## SV82/2024 Luka Berić

# Uputstvo za pokretanje sistema

## Preduslovi

# Potrebno je instalirati:

- .NET 9 SDK
- Docker Desktop
- Docker Compose
- Minikube i kubectl (za Kubernetes)


# Docker Compose pokretanje

# U glavnom direktorijumu projekta:

docker compose build --no-cache

docker compose up --build

kubectl get nodes

kubectl get pods -n ds-snus

kubectl get services -n ds-snus
