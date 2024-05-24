<template>
  <div class="dashboard">
    <h1>Dashboard</h1>
    <div class="controls">
      <details>
        <summary><h2>Verkeerslicht 1</h2></summary>
        <button @click="toggleStoplicht('rood', 1)" :class="{ 'on': stoplichtKleur[1] === 'rood', 'red': true }">Rood</button>
        <button @click="toggleStoplicht('geel', 1)" :class="{ 'on': stoplichtKleur[1] === 'geel', 'orange': true }">Geel</button>
        <button @click="knipperGeel(1)" :class="{ 'knipper-actief': isKnipperActief(1) }">Knipper Geel</button>
        <button @click="toggleStoplicht('groen', 1)" :class="{ 'on': stoplichtKleur[1] === 'groen', 'green': true }">Groen</button>
      </details>
      
      <details>
        <summary><h2>Verkeerslicht 2</h2></summary>
        <button @click="toggleStoplicht('rood', 2)" :class="{ 'on': stoplichtKleur[2] === 'rood', 'red': true }">Rood</button>
        <button @click="toggleStoplicht('geel', 2)" :class="{ 'on': stoplichtKleur[2] === 'geel', 'orange': true }">Geel</button>
        <button @click="knipperGeel(2)" :class="{ 'knipper-actief': isKnipperActief(2) }">Knipper Geel</button>
        <button @click="toggleStoplicht('groen', 2)" :class="{ 'on': stoplichtKleur[2] === 'groen', 'green': true }">Groen</button>
      </details>

      <details>
        <summary><h2>Afsluitbomen .</h2></summary>
        <button @click="toggleSlagboom(1)" :disabled="foutStatus[1] && !geforceerdGesloten[1]" :class="{ 'on': slagboomStatus[1] }">Afsluitboom 1: {{ slagboomStatus[1] ? 'Gesloten' : 'Open' }}</button>
        <button @click="toggleSlagboom(2)" :disabled="foutStatus[2] && !geforceerdGesloten[2]" :class="{ 'on': slagboomStatus[2] }">Afsluitboom 2: {{ slagboomStatus[2] ? 'Gesloten' : 'Open' }}</button>
        <button @click="geforceerdSluiten(1)">Geforceerd Sluiten Slagboom 1</button>
        <button @click="geforceerdSluiten(2)">Geforceerd Sluiten Slagboom 2</button>
        <button @click="resetSlagboom(1)">Reset Slagboom 1</button>
        <button @click="resetSlagboom(2)">Reset Slagboom 2</button>
      </details>

      <button @click="resetAlles">Reset alles</button>
      <button @click="startSimulatie">Demo</button>
    </div>

    <div class="situatie-overzicht">
      <!-- Stoplichten -->
      <details>
        <summary><h2>Verkeerslichten overzicht</h2></summary>
        <ul>
          <li>Verkeerslicht 1 status: {{ stoplichtKleur[1] || 'Gedoofd' }}</li>
          <li>Verkeerslicht 2 status: {{ stoplichtKleur[2] || 'Gedoofd' }}</li>
        </ul>
      </details>
    <!-- Slagbomen -->
      <details>
        <summary><h2>Afsluitbomen overzicht</h2></summary>
        <ul>
          <li>Afsluitboom 1 status: {{ slagboomStatus[1] ? 'Gesloten' : 'Open' }}</li>
          <li>Afsluitboom 2 status: {{ slagboomStatus[2] ? 'Gesloten' : 'Open' }}</li>
        </ul>
      </details>
    </div>
    <div class="autoweg-container">
      <div class="weg-representatie">
        <div class="stoplichten-en-slagbomen">
          <!-- Set 1 met nummering in witte kleur en slagboom bovenaan -->
          <div style="display: flex; align-items: center;">
            <div style="padding-right: 10px; font-size: 10px; color: white;">VKL 1</div>
            <div>
              <div class="slagboom slagboom-boven" :class="{ 'on2': slagboomStatus[1] }"></div>
              <div class="stoplicht" :class="{ 'on': stoplichtKleur[1] === 'rood', 'red': true }"></div>
              <div class="stoplicht" :class="{ 'on': stoplichtKleur[1] === 'geel', 'orange': true }"></div>
              <div class="stoplicht" :class="{ 'on': stoplichtKleur[1] === 'groen', 'green': true }"></div>
            </div>
            <div style="padding-left: 80px; font-size: 10px; color: white;">ASB 1</div>
          </div>
        
          <!-- Set 2 met nummering in witte kleur en slagboom onderaan gedraaid -90 graden -->
          <div style="display: flex; align-items: center;">
            <div style="padding-right: 10px; font-size: 10px; color: white;">VKL 2</div>
            <div>
              <div class="stoplicht" :class="{ 'on': stoplichtKleur[2] === 'rood', 'red': true }"></div>
              <div class="stoplicht" :class="{ 'on': stoplichtKleur[2] === 'geel', 'orange': true }"></div>
              <div class="stoplicht" :class="{ 'on': stoplichtKleur[2] === 'groen', 'green': true }"></div>
              <div class="slagboom slagboom-onder" :class="{ 'on': slagboomStatus[2] }"></div>
            </div>
            <div style="padding-left: 80px; font-size: 10px; color: white;">ASB 2</div>
          </div>
        </div>
      </div>
      <!-- Tweede weg zonder stoplichten en slagbomen -->
      <div class="weg-representatie"></div>
    </div>

    <!-- Meldingen -->
    <div v-if="notifications.length" class="notifications">
      <ul>
        <li v-for="(notification, index) in notifications" :key="index">{{ notification }}</li>
      </ul>
    </div>
  </div>
</template>



<script>
import client from "@/assets/mqtt.js";

export default {
  name: 'Dashboard',
  data() {
    return {
      stoplichtKleur: {},
      slagboomStatus: {},
      foutStatus: { 1: false, 2: false }, // Status om bij te houden of er een foutmelding actief is
      geforceerdGesloten: { 1: false, 2: false }, // Status om bij te houden of de slagboom geforceerd gesloten is
      verkeersintensiteit: 'laag',
      simulatieInterval: null,
      knipperInterval: null,
      knipperStatus: { 1: false, 2: false },
      notifications: [] // Toegevoegd voor meldingen
    };
  },
  created() {
    client.on('message', this.processIncomingMessage);
    // Abonneer op de relevante topics
    client.subscribe('asb/+/terugkoppeling', { qos: 1 });
  },
  methods: {
    processIncomingMessage(topic, message) {
      console.log(`Ontvangen bericht op topic: ${topic}: ${message.toString()}`);
      const msg = message.toString(); // Converteer het bericht naar een string
      const [prefix, nummer, suffix] = topic.split('/'); // Split het topic in delen

      // Controleer of het bericht van de slagboom terugkoppeling komt
      if (prefix === 'asb' && (nummer === '1' || nummer === '2') && suffix === 'terugkoppeling') {
        if (msg === '5') {
          const notification = `Normaal sluiten van slagboom ${nummer} mislukt (object gedetecteerd)`;
          if (!this.notifications.includes(notification)) {
            this.notifications.push(notification);
          }
          this.foutStatus[nummer] = true; // Zet foutstatus voor de slagboom
          this.geforceerdGesloten[nummer] = false; // Reset geforceerde sluiting status
        } else if (msg === '0' || msg === '1') {
          if (!this.foutStatus[nummer]) {
            this.removeNotification(nummer);
          }
        }
      }
    },

    removeNotification(nummer) {
      const notification = `Normaal sluiten van slagboom ${nummer} mislukt (object gedetecteerd)`;
      const index = this.notifications.indexOf(notification);
      if (index !== -1) {
        this.notifications.splice(index, 1);
      }
    },

    toggleStoplicht(kleur, nummer) {
      let message = '0'; // Standaard waarde voor 'uit'

      // Schakel de knipperfunctie uit als deze actief is
      if (this.knipperStatus[nummer]) {
        this.knipperGeel(nummer); // Deactiveer het knipperen
      }

      // Schakel de huidige kleur uit
      if (this.stoplichtKleur[nummer] !== null) {
        if (this.stoplichtKleur[nummer] === 'knipper') {
          this.publishMessage(`vkl/${nummer}/verander`, '0'); // Stuur bericht om knipperen uit te zetten
        } else {
          this.publishMessage(`vkl/${nummer}/verander`, '0'); // Stuur bericht om huidige kleur uit te zetten
        }
      }

      // Stel de nieuwe kleur in of zet deze uit als het dezelfde kleur is
      if (this.stoplichtKleur[nummer] === kleur) {
        this.stoplichtKleur[nummer] = null;
      } else {
        this.stoplichtKleur[nummer] = kleur;

        if (kleur === 'rood') {
          message = '1';
        } else if (kleur === 'geel') {
          message = '2';
        } else if (kleur === 'groen') {
          message = '3';
        }
      }

      const topic = `vkl/${nummer}/verander`;
      this.publishMessage(topic, message);
    },

    toggleSlagboom(nummer) {
      if (this.foutStatus[nummer] && !this.geforceerdGesloten[nummer]) {
        console.log(`Slagboom ${nummer} kan niet bediend worden vanwege foutmelding`);
        return; // Stop als er een foutmelding actief is en de slagboom niet geforceerd gesloten is
      }

      const message = this.slagboomStatus[nummer] ? '0' : '1';
      const topic = `asb/${nummer}/verander`;
      this.publishMessage(topic, message);
      this.slagboomStatus[nummer] = !this.slagboomStatus[nummer];
    },

    geforceerdSluiten(nummer) {
      const topic = `asb/${nummer}/verander`;
      const message = '2'; // Bericht voor geforceerd sluiten
      this.publishMessage(topic, message);
      this.slagboomStatus[nummer] = true; // Zet de status van de slagboom op gesloten
      this.geforceerdGesloten[nummer] = true; // Markeer als geforceerd gesloten
    },

    resetSlagboom(nummer) {
      this.foutStatus[nummer] = false; // Reset de foutstatus voor de specifieke slagboom
      this.geforceerdGesloten[nummer] = false; // Reset de geforceerde sluiting status
      this.removeNotification(nummer); // Verwijder eventuele meldingen
      this.slagboomStatus[nummer] = false; // Zet de slagboom terug naar de open status

      // Publish the MQTT message to reset the barrier
      const topic = `asb/${nummer}/verander`;
      const message = '0'; // '0' indicates reset/open command
      this.publishMessage(topic, message);
    },

    resetAlles() {
      for (let i = 1; i <= 2; i++) {
        this.stoplichtKleur[i] = null;
        this.slagboomStatus[i] = false;
        this.foutStatus[i] = false; // Reset de foutstatus
        this.geforceerdGesloten[i] = false; // Reset geforceerde sluiting status
        this.removeNotification(i); // Verwijder eventuele meldingen
        this.publishMessage(`asb/${i}/verander`, '3'); // Reset noodstop
        this.publishMessage(`asb/${i}/verander`, '0'); // Voeg deze regel toe om '0' te publiceren
        this.publishMessage(`vkl/${i}/verander`, '0');
        this.publishMessage(`vkl/${i}/verander`, '0');
        this.publishMessage(`vkl/${i}/verander`, '0');
      }
      if (this.knipperInterval) {
        clearInterval(this.knipperInterval);
        this.knipperInterval = null;
        this.knipperStatus = { 1: false, 2: false };
      }
    },

    knipperGeel(nummer) {
      if (this.knipperStatus[nummer]) {
        // Deactiveer de knipperfunctie als deze actief is
        clearInterval(this.knipperInterval);
        this.knipperInterval = null;
        this.knipperStatus[nummer] = false;
        this.stoplichtKleur[nummer] = null; // Zet de kleur terug naar 'uit'
        this.publishMessage(`vkl/${nummer}/verander`, '0'); // Bericht voor knipperen uit
      } else {
        // Activeer de knipperfunctie als deze niet actief is
        if (this.stoplichtKleur[nummer] !== null) {
          this.publishMessage(`vkl/${nummer}/verander`, '0'); // Zet huidige kleur uit
        }
        this.knipperStatus[nummer] = true;
        this.stoplichtKleur[nummer] = 'knipper';
        this.publishMessage(`vkl/${nummer}/verander`, '4'); // Bericht voor knipperen aan

        // Houd bij of het stoplicht aan of uit is tijdens het knipperen
        this.knipperInterval = setInterval(() => {
          if (this.stoplichtKleur[nummer] === 'geel') {
            this.stoplichtKleur[nummer] = null;
          } else {
            this.stoplichtKleur[nummer] = 'geel';
          }
        }, 1000); // Herhaal het knipperen elke 1000 ms (1 keer per seconde)
      }
    },

    isKnipperActief(nummer) {
      return this.knipperStatus[nummer];
    },

    startSimulatie() {
      this.resetAlles(); // Reset alle lichten en slagbomen voor de simulatie begint
      let steps = [
        { action: () => this.toggleStoplicht('rood', 1), delay: 2000 },
        { action: () => this.toggleSlagboom(1), delay: 10000 },
        { action: () => this.toggleSlagboom(1), delay: 10000 },
        { action: () => this.toggleStoplicht('groen', 1), delay: 2000 },
        { action: () => this.toggleStoplicht('geel', 1), delay: 500 },
        { action: () => this.toggleStoplicht('geel', 1), delay: 500 },
        { action: () => this.toggleStoplicht('geel', 1), delay: 500 },
        { action: () => this.toggleStoplicht('geel', 1), delay: 500 },
        { action: () => this.toggleStoplicht('geel', 1), delay: 3000 },
        { action: () => this.toggleStoplicht('rood', 1), delay: 2000 },
        { action: () => this.toggleSlagboom(1), delay: 10000 },
        { action: () => this.simulateSlagboomError(1), delay: 5000 }, // Simuleer foutmelding voor slagboom 1
        { action: () => this.simulateSlagboomOpen(1), delay: 5000 } // Simuleer het openen van slagboom 1 om de foutmelding te verwijderen
      ];

      let index = 0;
      const executeStep = () => {
        let step = steps[index];
        step.action();
        index++;
        if (index < steps.length) {
          setTimeout(executeStep, step.delay);
        } else {
          index = 0; // Reset voor de volgende cyclus
          setTimeout(executeStep, 30000); // Herstart na de volledige cyclus
        }
      };

      executeStep(); // Start de eerste stap
    },

    stopSimulatie() {
      clearInterval(this.simulatieInterval);
      if (this.knipperInterval) {
        clearInterval(this.knipperInterval);
        this.knipperInterval = null;
        this.knipperStatus = { 1: false, 2: false };
      }
    },

    simulateSlagboomError(nummer) {
      console.log(`Normaal sluiten van slagboom ${nummer} mislukt (object gedetecteerd)`);
      this.notifications.push(`Normaal sluiten van slagboom ${nummer} mislukt (object gedetecteerd)`);
      this.foutStatus[nummer] = true; // Zet foutstatus voor de slagboom
    },

    simulateSlagboomOpen(nummer) {
      console.log(`Simulatie: Slagboom ${nummer} is geopend`);
      this.slagboomStatus[nummer] = false;
      this.removeNotification(nummer);
      this.foutStatus[nummer] = false; // Reset de foutstatus
    },

    publishMessage(topic, message) {
      client.publish(topic, message, {qos: 1}, (error) => {
        if (error) {
          console.error(`[MQTT] Publish error on topic '${topic}':`, error);
        } else {
          console.log(`[MQTT] Published message on topic '${topic}': ${message}`);
        }
      });
    },
  }
};
</script>


<style scoped>
.dashboard {
  background-color: #fff;
  border-radius: 50px;
  box-shadow: 0px 8px 24px rgba(0, 0, 0, 0.1);
  width: 700px;
  padding: 25px;
  box-sizing: border-box;
  color: #2c3e50;
}
.controls {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: row;
  margin-bottom: 30px;
}
.controls button {
  background-color: #ecf0f1;
  border: none;
  border-radius: 6px;
  padding: 10px 15px;
  margin: 5px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  color: #7f8c8d;
  font-size: 14px;
}
.controls button:hover {
  background-color: #bdc3c7;
}
.controls button.on.red {
  color: #fff;
  background-color: #e74c3c;
}
.controls button.on.orange {
  color: #fff;
  background-color: #f39c12;
}
.controls button.on.green {
  color: #fff;
  background-color: #2ecc71;
}
.stoplicht {
  margin-top: 20px;
}
.stoplicht h2 {
  text-align: center;
}
.stoplicht-container {
  display: flex;
  justify-content: center;
}
.stoplicht-light {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  margin: 5px;
}
.red {
  background-color: red;
}
.orange {
  background-color: orange;
}
.green {
  background-color: green;
}
.on {
  border: 2px solid #000;
}
.situatie-overzicht {
  padding: 15px;
  margin-top: 20px;
  background-color: #ecf0f1;
  border-radius: 6px;
}
.situatie-overzicht h2 {
  text-align: center;
  margin-bottom: 15px;
}
.situatie-overzicht ul {
  list-style: none;
  padding: 0;
}
.situatie-overzicht li {
  margin-bottom: 10px;
  font-size: 14px;
}

/* Algemene container voor de weg */
.weg-representatie {
  background-color: #333; /* Donkergrijs voor het asfalt */
  height: 150px; /* De hoogte van de weg */
  width: 100%; /* Volledige breedte */
  margin-bottom: 20px; /* Ruimte tussen de wegen */
  position: relative;
}

/* Wegmarkeringen toevoegen */
.weg-representatie::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  width: calc(130% - 200px); 
  height: 4px;
  background: repeating-linear-gradient(
    to right,
    #fff,
    #fff 10px,
    #333 10px,
    #333 20px /* Breedte van de stippellijnen */
  );
  transform: translateY(-50%);
}

/* Verkeerslichten en slagbomen boven de weg */
.stoplichten-boven {
  position: absolute;
  width: 100%;
  top: 40%; /* Iets lager voor visuele balans */
  display: flex;
  justify-content: space-around;
  align-items: center;
}

/* Verkeerslichten en slagbomen onder de weg */
.stoplichten-onder {
  position: absolute;
  width: 100%;
  bottom: 40%; /* Iets hoger voor visuele balans */
  display: flex;
  justify-content: space-around;
  align-items: center;
}


/* Stijlen voor de verkeerslichten */
.stoplicht {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  margin: 0 5px;
  background-color: #555; /* Kleur wanneer het licht uit staat */
  border: 2px solid #333; /* Rand om de verkeerslichten te benadrukken */
}

.red.on { background-color: red; }
.orange.on { background-color: orange; }
.green.on { background-color: green; }

/* Stijlen voor de slagbomen */
.slagboom {
  width: 70px;
  height: 10px;
  background: linear-gradient(to right, white 25%, red 25%, red 50%, white 50%, white 75%, red 75%);
  border: 2px solid #333; /* Rand om de slagbomen te benadrukken */
  position: absolute;
  right: 50%;
  transform: translateX(-50%); /* Startpositie, gecentreerd */
  transition: transform 10s ease;
  transform-origin: left;
}

.slagboom-boven {
  transform: translateX(-50%) rotate(0deg); 
}

.slagboom-onder {
  transform: translateX(-50%) rotate(0deg); 
}

.slagboom.on {
  transform: translateX(-50%) rotate(-90deg); 
}

.slagboom.on2 {
  transform: translateX(-50%) rotate(90deg); 
}

.knipper-actief {
  background-color: yellow;
  color: black;
}

summary {
  cursor: pointer;
  font-weight: bold;
  padding: 20px;
  background-color: #f0f0f0; 
  border-radius: 5px; 
  transition: background-color 0.3s ease;
}

summary:hover {
  background-color: #e0e0e0; 
}

.dashboard {
  position: relative; /* Zorg ervoor dat het dashboard het referentiepunt is voor de absolute positionering */
  background-color: #fff;
  border-radius: 50px;
  box-shadow: 0px 8px 24px rgba(0, 0, 0, 0.1);
  width: 700px;
  padding: 25px;
  box-sizing: border-box;
  color: #2c3e50;
}
.dashboard {
  position: relative; /* Zorg ervoor dat het dashboard het referentiepunt is voor de absolute positionering */
  background-color: #fff;
  border-radius: 50px;
  box-shadow: 0px 8px 24px rgba(0, 0, 0, 0.1);
  width: 700px;
  padding: 25px;
  box-sizing: border-box;
  color: #2c3e50;
}

.notifications {
  background-color: #f8d7da;
  color: #721c24;
  border: 2px solid #f5c6cb;
  padding: 20px;
  border-radius: 10px;
  text-align: center;
  font-size: 1.5em;
  font-weight: bold;
  position: absolute; /* Maak de positie absoluut */
  top: 70%; /* Plaats iets naar beneden van het midden */
  left: 150%; /* Plaats in het midden van het dashboard */
  transform: translate(-50%, -50%); /* Corrigeer voor de breedte en hoogte */
  width: 80%; /* Maak de breedte van de melding 80% van de dashboard breedte */
  z-index: 1000; /* Zorg ervoor dat het boven de andere elementen komt */
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% {
    box-shadow: 0 0 5px #f5c6cb;
  }
  50% {
    box-shadow: 0 0 20px #f5c6cb;
  }
  100% {
    box-shadow: 0 0 5px #f5c6cb;
  }
}



</style>
