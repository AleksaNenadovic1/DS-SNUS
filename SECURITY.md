## AES enkripcija

Senzorski podaci se pre slanja šifruju AES algoritmom kako bi se zaštitila poverljivost podataka.


## ECDSA digitalni potpis

Svaka poruka se potpisuje digitalnim potpisom radi provere:

- autentičnosti izvora
- integriteta podataka


## Zaštita od Replay napada

Svaka poruka sadrži jedinstveni MessageId i vremensku oznaku.

Već obrađene poruke se odbacuju.


## Validacija podataka

Pre upisa u bazu proveravaju se:

- identitet senzora
- format poruke
- vremenska oznaka
- vrednost merenja


## Zaštita baze

PostgreSQL baza koristi korisničko ime i lozinku, a pristup je omogućen samo servisima kojima je potreban.


## Izolacija servisa

Docker i Kubernetes omogućavaju odvojeno izvršavanje servisa i kontrolisanu komunikaciju između njih.


## CQRS arhitektura

Odvajanje obrade podataka od API sloja omogućava bolju skalabilnost, održavanje i kontrolu pristupa.