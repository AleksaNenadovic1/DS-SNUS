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


# Kubernetes pokretanje

# U glavnom direktorijumu projekta:

kubectl get nodes
- kubectl apply -k bridge/overlays/desktop
- kubectl delete namespace ds-snus
kubectl get pods -n ds-snus

kubectl get svc -n ds-snus 
