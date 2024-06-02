# Dashboard

## Overzicht

Dit project is een verkeersbeheer dashboard geïmplementeerd met behulp van Vue.js. Het stelt de gebruiker in staat om verkeerslichten en slagbomen te bedienen. Het dashboard biedt knoppen om de status van de verkeerslichten en slagbomen te schakelen.

## Installatie

### Vereisten

Zorg ervoor dat je de volgende software hebt geïnstalleerd:

- [Node.js](https://nodejs.org/)
- [npm](https://www.npmjs.com/)

### Vue CLI Installeren

Voordat je het project kunt opzetten, moet je de Vue CLI (Command Line Interface) installeren. Dit is een handig hulpmiddel om Vue.js-projecten te maken en te beheren.

1. Installeer de Vue CLI via npm:
    ```bash
    npm install -g @vue/cli
    ```

2. Controleer of de Vue CLI correct is geïnstalleerd door de versie te controleren:
    ```bash
    vue --version
    ```

### Project Installatie

1. Clone de repository of download het zip-bestand en pak het uit.

2. Navigeer naar de projectdirectory:
    ```bash
    cd naam van de projectmap
    ```

3. Installeer de npm:
    ```bash
    npm install
    ```

4. Start de ontwikkelingsserver:
    ```bash
    npm run serve
    ```

## Functies

- **Verkeerslichten Controle:**
  - Schakel rode, gele en groene lichten voor twee verkeerslichten.
  - Activeer knipperend geel licht.
- **Slagbomen Controle:**
  - Schakel de open/dicht status van twee slagbomen.
  - Forceer slagbomen om te sluiten.
  - Reset slagbomen.
- **Simulatie:**
  - Start een demonstratie van het systeem.
- **Meldingen:**
  - Toon foutmeldingen wanneer slagbomen problemen ondervinden.

## Componenten

### Dashboard.vue

De belangrijkste Vue-component die de gebruikersinterface biedt voor het bedienen van de verkeerslichten en slagbomen.

#### Methods

- `processIncomingMessage(topic, message)`: Verwerkt inkomende MQTT-berichten en werkt de dashboardstatus bij.
- `removeNotification(nummer)`: Verwijdert meldingen voor een specifieke slagboom.
- `toggleStoplicht(kleur, nummer)`: Schakelt de status van een verkeerslicht.
- `toggleSlagboom(nummer)`: Schakelt de status van een slagboom.
- `geforceerdSluiten(nummer)`: Forceert een slagboom om te sluiten.
- `resetSlagboom(nummer)`: Reset de status van een slagboom.
- `resetAlles()`: Reset alle verkeerslichten en slagbomen.
- `knipperGeel(nummer)`: Activeert/deactiveert knipperend geel licht voor een verkeerslicht.
- `isKnipperActief(nummer)`: Controleert of knipperen actief is voor een verkeerslicht.
- `startSimulatie()`: Start een demonstratie van het systeem.
- `stopSimulatie()`: Stopt de simulatie.
- `simulateSlagboomError(nummer)`: Simuleert een foutmelding voor een slagboom.
- `simulateSlagboomOpen(nummer)`: Simuleert het openen van een slagboom om de foutmelding te verwijderen.
- `publishMessage(topic, message)`: Publiceert een MQTT-bericht.

## MQTT Instellingen

## Installatie van MQTT Explorer

MQTT Explorer is een handige tool om je MQTT-berichten te monitoren en debuggen. Het biedt een grafische interface waarmee je eenvoudig kunt zien welke berichten naar welke topics worden gepubliceerd en welke berichten je client ontvangt.

### MQTT Explorer Installeren

1. Ga naar de [MQTT Explorer GitHub-pagina] [directe download](https://mqtt-explorer.com/).

2. Installeer de applicatie door het gedownloade bestand te openen en de instructies te volgen.

### MQTT Explorer Gebruiken

1. Start MQTT Explorer na de installatie.

2. Maak een nieuwe verbinding door te klikken op de knop `Add new connection`.

3. Voer de URL van je MQTT-broker in (bijvoorbeeld `ws://10.3.141.1:8801`).

4. Klik op `Connect` om verbinding te maken met de broker.

5. Zodra de verbinding tot stand is gebracht, kun je de verschillende topics zien waarop berichten worden gepubliceerd en ontvangen. Je kunt ook zelf berichten publiceren naar topics door op de knop `Publish` te klikken en de benodigde informatie in te vullen.

### mqtt.js

controleer of de `mqtt.js` bestand  in de `src/assets` directory met de volgende inhoud bevat:

```javascript
import mqtt from 'mqtt'

//'ws://mq.nl.eu.org:8884'
//const client = mqtt.connect(`ws://${window.location.hostname}:8801`, {
const client = mqtt.connect(`ws://10.3.141.1:8801`, {
    clean: true,
    connectTimeout: 4000,
    reconnectPeriod: 5000,
    keepalive: 60,
    resubscribe: true
})

client.on('close', () => {
    console.log('[MQTT] Connection closed')
})

client.on('error', (error) => {
    console.error('[MQTT] Connection failed', error)
})

client.on("connect", () => {
    console.log('[MQTT] Connection established')
})

export default client
```


### Build

```sh
npm run build
```

This produces a `dist` folder, which needs to be moved to the Raspberry Pi. Then you can do `docker compose up -d --build`.
