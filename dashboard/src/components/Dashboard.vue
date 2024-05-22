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
        <summary><h2>Afsluitbomen</h2></summary>
        <button @click="toggleSlagboom(1)" :class="{ 'on': slagboomStatus[1] }">Afsluitboom 1: {{ slagboomStatus[1] ? 'Gesloten' : 'Open' }}</button>
        <button @click="toggleSlagboom(2)" :class="{ 'on': slagboomStatus[2] }">Afsluitboom 2: {{ slagboomStatus[2] ? 'Gesloten' : 'Open' }}</button>
      </details>

      <button @click="resetAlles">Reset</button>
      <button @click="startSimulatie">Demo</button>
    </div>



    <!-- <div class="situatie-overzicht">
      <summary><h2>Situatieoverzicht</h2></summary>
      <ul>
        <li>Stoplicht 1 kleur: {{ stoplichtKleur[1] || 'Uit' }}</li>
        <li>Stoplicht 2 kleur: {{ stoplichtKleur[2] || 'Uit' }}</li>
        <li>Slagboom 1: {{ slagboomStatus[1] ? 'Open' : 'Gesloten' }}</li>
        <li>Slagboom 2: {{ slagboomStatus[2] ? 'Open' : 'Gesloten' }}</li>
      </ul>
    </div> -->

    
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
      verkeersintensiteit: 'laag',
      simulatieInterval: null,
      knipperInterval: null,
      knipperStatus: { 1: false, 2: false },
    };
  },
  methods: {
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
      // Controleer de huidige status om de juiste tijdelijke status te bepalen
      const temporaryStatus = this.slagboomStatus[nummer] ? '4' : '3';
      const topicStart = `asb/${nummer}/verander`;
      this.publishMessage(topicStart, temporaryStatus);

      // Status van de slagboom wijzigen
      this.slagboomStatus[nummer] = !this.slagboomStatus[nummer];

      // Wacht 10 seconden voordat de definitieve status wordt gepubliceerd
      setTimeout(() => {
        const topicFinal = `asb/${nummer}/verander`;
        const messageFinal = this.slagboomStatus[nummer] ? '0' : '1';
        this.publishMessage(topicFinal, messageFinal);
      }, 10000); // 10 seconden
    },
    resetAlles() {
      for (let i = 1; i <= 2; i++) {
        this.stoplichtKleur[i] = null;
        this.slagboomStatus[i] = false;
        this.publishMessage(`asb/${i}/verander`, '2');
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

</style>
