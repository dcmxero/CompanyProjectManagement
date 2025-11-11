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

## ⚙️ Požiadavky

Pred spustením je potrebné mať nainštalované:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org)
- npm (súčasť Node.js)
- [Angular CLI](https://angular.dev/cli)
  ```bash
  npm install -g @angular/cli
