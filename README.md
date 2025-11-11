# Testovací projekt (.NET + Angular)

Tento projekt demonštruje jednoduchú aplikáciu pre správu projektov – zobrazenie, vytváranie, úpravu a mazanie projektov.  
Backend je postavený na **ASP.NET Core Web API** a ukladá dáta do **XML súboru**.  
Frontend je vytvorený v **Angulari** a komunikuje s API cez HTTP.

---

## 🧩 Použité technológie

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

---

### Konfigurácia projektu
- Projekt používa XML konfiguráciu pre prihlasovanie, logovanie a cesty k dátam.
- Súbor sa nachádza v priečinku Config/config.xml.
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

## 🧠 Poznámky

- Projekt **nepoužíva databázu**, všetky dáta sú uložené v XML súbore.  
- Po vypnutí aplikácie zostávajú dáta zachované.  
```

---

## 👨‍💻 Autor

Vypracované ako testovací príklad pre .NET / Angular (2025).
