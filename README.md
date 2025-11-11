# Testovací projekt (.NET + Angular)

Tento projekt demonštruje jednoduchú aplikáciu pre správu projektov – zobrazenie, vytváranie, úpravu a mazanie projektov.  
Backend je postavený na **ASP.NET Core Web API** a ukladá dáta do **XML súboru**.  
Frontend je vytvorený v **Angulari** a komunikuje s API cez HTTP.

---

## 💡 Dôvody výberu technológií

Pre implementáciu backendu som si zvolil **.NET (C#)**, pretože s touto technológiou mám dlhoročné skúsenosti a dobre ju poznám z praxe.  
ASP.NET Core poskytuje stabilné, výkonné a prehľadné prostredie pre tvorbu REST API, takže bol prirodzenou voľbou pre serverovú časť projektu.

Naopak, pre frontend som si zvolil **Angular**, s ktorým som už dlhšie nepracoval.  
Projekt som preto využil ako príležitosť na **obnovenie znalostí Angularu**, hlavne v oblasti **Reactive Forms**, komunikácie s API a práce so **standalone komponentmi**.

Kombinácia .NET a Angular umožňuje jasné oddelenie backendu a frontendu, moderný vývojový proces a typovo bezpečné riešenie na oboch stranách aplikácie.

---

## 🏗️ Architektúra riešenia

Projekt je rozdelený na dve časti:

- **WebApi (C# / ASP.NET Core)** – serverová časť poskytujúca REST API, spracováva CRUD operácie a zapisuje dáta do XML.
- **WebClient (Angular)** – klientská aplikácia komunikujúca s API cez HTTP, zobrazujúca projekty a formuláre pre ich úpravu.

Komunikácia medzi klientom a serverom prebieha cez HTTP vo formáte JSON.

---

## 🧩 Použité technológie

### 🧰 Vývojové prostredie a nástroje

- **Visual Studio 2022** – vývoj backendu, ladenie API, správa závislostí a konfigurácií.  
- **Visual Studio Code** – vývoj frontendu, úprava HTML, CSS a TypeScript kódu.  
- **Angular CLI** – generovanie komponentov, buildovanie a spúšťanie Angular aplikácie.  
- **Swagger UI (Swashbuckle)** – interaktívne testovanie a automatická dokumentácia API.  
- **Serilog** – logovanie udalostí na strane servera pre jednoduchšie ladenie a sledovanie chýb.

### Backend (.NET)
- **ASP.NET Core Web API** – REST rozhranie pre operácie s projektmi  
- **XML súbor** – perzistencia dát namiesto databázy  
- **System.Xml.Linq (LINQ to XML)** – čítanie a zápis XML  
- **Swagger (Swashbuckle)** – dokumentácia a testovanie API  

### Frontend (Angular)
- **Angular** (standalone komponenty, Reactive Forms)  
- **TypeScript**  
- **RxJS** – reakčné programovanie  
- **HttpClient** – komunikácia s backendom  

### Konfigurácia projektu
- Projekt používa XML konfiguráciu pre prihlasovanie, logovanie a cesty k dátam.
- Súbor sa nachádza v priečinku Config/config.xml.
- Prihlasovacie údaje (username: admin, password: Admin123) sú uložené v tomto súbore.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <auth>
    <username>admin</username>
    <passwordHash>Admin123</passwordHash>
  </auth>
  <storage>
    <projectsPath>../data/projects.xml</projectsPath>
    <encoding>windows-1250</encoding>
  </storage>
  <logging>
    <logPath>../logs/app-.log</logPath>
    <fileSizeLimit>10</fileSizeLimit>
    <retainedFiles>5</retainedFiles>
    <minimumLevel>Information</minimumLevel>
  </logging>
</configuration>
```
---

## ⚙️ Požiadavky

Pred spustením je potrebné mať nainštalované:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org)
- npm (súčasť Node.js)
- [Angular CLI](https://angular.dev/cli)

Inštalácia Angular CLI:
```bash
npm install -g @angular/cli
```

---

## 🚀 Spustenie projektu

> 📝 **Poznámka:**  
> Všetky príkazy nižšie predpokladajú, že sa nachádzate v koreňovom priečinku projektu,  
> kde sa nachádza súbor `*.sln`.

### 1️⃣ Backend – API
```bash
cd WebApi
dotnet restore
dotnet run
```

Client sa spustí na:  
➡️ `http://localhost:4200/login`

Swagger UI (na testovanie API):  
➡️ `https://localhost:5001/swagger/index.html`

Projekt pri štarte načíta XML súbor (napr. `projects.xml`), ktorý obsahuje všetky projekty.  
Zmeny (vytvorenie, úprava, zmazanie) sa okamžite zapisujú späť do XML.

---

### 2️⃣ Frontend – Angular
```bash
cd WebClient
npm install
npm start
```

Frontend sa spustí na:  
➡️ `http://localhost:4200`

---

## 📄 Štruktúra XML súboru

Dáta o projektoch sa ukladajú do súboru `projects.xml` v nasledujúcej štruktúre:

```xml
<?xml version="1.0" encoding="windows-1250"?>
<projects>
	<project id="prj1">
		<name>Informačný systém firmy ABC</name>
		<abbreviation>IS-ABC</abbreviation>
		<customer>ABC, s. r. o.</customer>
	</project>
	<project id="prj2">
		<name>Importný modul ISIS</name>
		<abbreviation>Import-ISIS</abbreviation>
		<customer>Homer Simpson</customer>
	</project>
	<project id="prj3">
		<name>Portácia IS-VAK na Oracle</name>
		<abbreviation>OracleVAK</abbreviation>
		<customer>VAK, s. p.</customer>
	</project>
	<project id="prj4">
		<name>Elektronický obchod pre Telecom</name>
		<abbreviation>EComTelecom</abbreviation>
		<customer>Český Telecom, a. s.</customer>
	</project>
	<project id="prj5">
		<name>Rozpoznávanie čiarového kódu pre Delvitu</name>
		<abbreviation>CK-Delvita</abbreviation>
		<customer>Delvita, a. s.</customer>
	</project>
</projects>
```

---

## ✅ Validácia vstupov

Validácia údajov sa vykonáva **na strane frontendu** pomocou Angular Reactive Forms.  
Formulár kontroluje, či sú všetky povinné polia vyplnené (`Validators.required`) a zobrazuje používateľovi chybové hlášky priamo v rozhraní.

---

## 🧾 Logovanie

Aplikácia využíva knižnicu **Serilog** na zaznamenávanie udalostí.  
Logy sa zapisujú do súborov v priečinku `logs` podľa konfigurácie v `config.xml`.

Logujú sa:
- spustenie aplikácie  
- úspešné / neúspešné požiadavky  
- chyby pri čítaní alebo zápise XML

---

## 🧠 Poznámky

- Projekt **nepoužíva databázu**, všetky dáta sú uložené v XML súbore.  
- Po vypnutí aplikácie zostávajú dáta zachované.  

---

## 🔧 Možnosti rozšírenia

Do budúcnosti je možné aplikáciu rozšíriť bez väčších zásahov do architektúry:

### 💾 Podpora pre databázu (SQL Server, SQLite)
Aktuálne sa projekty ukladajú do XML súboru.  
Službu `ProjectsService` by bolo možné jednoducho nahradiť implementáciou, ktorá využíva **Entity Framework Core**.  
Stačí:
- vytvoriť databázový kontext (`DbContext`) s entitou `Project`,
- nakonfigurovať pripojenie v `appsettings.json`,
- upraviť dependency injection, aby aplikácia používala novú implementáciu služby (napr. `IProjectsRepository`),
- spustiť migrácie (`dotnet ef migrations add Init` a `dotnet ef database update`).

Tým by sa perzistencia presunula z XML do databázy bez zmeny API vrstvy ani Angular frontendu.

### 🔐 Autentifikácia pomocou JWT tokenov

Aplikácia využíva **JWT autentifikáciu** pre prihlásenie používateľa.

Prihlasovacie údaje sa načítavajú z konfiguračného XML súboru (`Config/config.xml`).  
Po úspešnom prihlásení API vygeneruje **JWT token** (pomocou knižnice `System.IdentityModel.Tokens.Jwt`),  
ktorý klient (Angular) uloží do `localStorage` a následne ho automaticky pridáva do hlavičky každého HTTP volania:
- Token je následne overovaný middleware-om v `Program.cs`, čím je zabezpečený prístup k chráneným API endpointom.

### 👥 Správa používateľov a rolí

Na autentifikáciu sa momentálne používa jeden účet definovaný v XML súbore.  
Jednoduchým rozšírením by bolo pridať:
- správu používateľov v osobitnom XML alebo databáze,
- definovanie rolí (napr. **admin**, **user**),
- a kontrolu prístupu pomocou `[Authorize(Roles = "...")]` atribútov v controlleroch.


---

## 👨‍💻 Autor

Vypracované ako testovací príklad pre .NET / Angular (2025).
