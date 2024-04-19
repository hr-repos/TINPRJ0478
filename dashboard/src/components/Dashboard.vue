<template>
  <div class="dashboard">
    <h1>Dashboard</h1>
    <div class="controls">
      <!-- Verkeerslicht en slagboom controls voor de eerste weg -->
      <div class="verkeerslicht-controls">
        <h2>Verkeerslichten en slagbomen voor weg 1</h2>
        <!-- Stoplicht 1 controls -->
        <div class="stoplicht-control">
          <h3>Stoplicht 1</h3>
          <button @click="toggleStoplicht('rood', 1)" :class="{ 'on': stoplichtKleur[1] === 'rood', 'red': true }">Rood</button>
          <button @click="toggleStoplicht('oranje', 1)" :class="{ 'on': stoplichtKleur[1] === 'oranje', 'orange': true }">Oranje</button>
          <button @click="toggleStoplicht('groen', 1)" :class="{ 'on': stoplichtKleur[1] === 'groen', 'green': true }">Groen</button>
        </div>
        <!-- Stoplicht 2 controls -->
        <div class="stoplicht-control">
          <h3>Stoplicht 2</h3>
          <button @click="toggleStoplicht('rood', 2)" :class="{ 'on': stoplichtKleur[2] === 'rood', 'red': true }">Rood</button>
          <button @click="toggleStoplicht('oranje', 2)" :class="{ 'on': stoplichtKleur[2] === 'oranje', 'orange': true }">Oranje</button>
          <button @click="toggleStoplicht('groen', 2)" :class="{ 'on': stoplichtKleur[2] === 'groen', 'green': true }">Groen</button>
        </div>
        <!-- Slagbomen controls -->
        <div class="slagboom-controls">
          <h3>Slagbomen voor weg 1</h3>
          <button @click="toggleSlagboom(1)" :class="{ 'on': slagboomStatus[1] }">Slagboom 1: {{ slagboomStatus[1] ? 'Open' : 'Gesloten' }}</button>
          <button @click="toggleSlagboom(2)" :class="{ 'on': slagboomStatus[2] }">Slagboom 2: {{ slagboomStatus[2] ? 'Open' : 'Gesloten' }}</button>
        </div>
      </div>
      <!-- Resetknop -->
      <button @click="resetAlles">Reset</button>
    </div>
    
    <div class="situatie-overzicht">
      <h2>Situatieoverzicht</h2>
      <ul>
        <li>Stoplicht 1 kleur: {{ stoplichtKleur[1] || 'Uit' }}</li>
        <li>Stoplicht 2 kleur: {{ stoplichtKleur[2] || 'Uit' }}</li>
        <li>Slagboom 1: {{ slagboomStatus[1] ? 'Open' : 'Gesloten' }}</li>
        <li>Slagboom 2: {{ slagboomStatus[2] ? 'Open' : 'Gesloten' }}</li>
      </ul>
    </div>

    <!-- Wegen met verkeerslichten en slagbomen -->
    <div class="autoweg-container">
      <!-- Eerste weg met stoplichten en slagbomen -->
      <div class="weg-representatie">
        <div class="stoplichten-en-slagbomen">
          <div class="stoplicht" :class="{ 'on': stoplichtKleur[1] === 'rood', 'red': true }"></div>
          <div class="stoplicht" :class="{ 'on': stoplichtKleur[1] === 'oranje', 'orange': true }"></div>
          <div class="stoplicht" :class="{ 'on': stoplichtKleur[1] === 'groen', 'green': true }"></div>
          <div class="slagboom" :class="{ 'on': slagboomStatus[1] }"></div>
          <div class="stoplicht" :class="{ 'on': stoplichtKleur[2] === 'rood', 'red': true }"></div>
          <div class="stoplicht" :class="{ 'on': stoplichtKleur[2] === 'oranje', 'orange': true }"></div>
          <div class="stoplicht" :class="{ 'on': stoplichtKleur[2] === 'groen', 'green': true }"></div>
          <div class="slagboom" :class="{ 'on': slagboomStatus[2] }"></div>
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
    };
  },
  methods: {
    toggleStoplicht(kleur, nummer) {
  // Als de huidige kleur van het stoplicht gelijk is aan de geselecteerde kleur,
  // zet deze dan op 0 (uit).
  if (this.stoplichtKleur[nummer] === kleur) {
    this.stoplichtKleur[nummer] = null;
    this.publishMessage(`vkl${nummer}/${kleur}`, '0');
  } else {
    // Zet de oude kleur van het relevante stoplicht naar 0
    if (this.stoplichtKleur[nummer] === 'rood') {
      this.publishMessage(`vkl${nummer}/rood`, '0');
    } else if (this.stoplichtKleur[nummer] === 'oranje') {
      this.publishMessage(`vkl${nummer}/oranje`, '0');
    } else if (this.stoplichtKleur[nummer] === 'groen') {
      this.publishMessage(`vkl${nummer}/groen`, '0');
    }

    // Stel de kleur van het stoplicht in op de geselecteerde kleur
    this.stoplichtKleur[nummer] = kleur;
    const topic = `vkl${nummer}/${kleur}`;
    const message = kleur ? '1' : '0';
    this.publishMessage(topic, message);
  }
}


,
    toggleSlagboom(nummer) {
      this.slagboomStatus[nummer] = !this.slagboomStatus[nummer];
      const topic = `asb/${nummer}`;
      const message = this.slagboomStatus[nummer] ? '1' : '0';
      this.publishMessage(topic, message);
    },
    resetAlles() {
      for (let i = 1; i <= 2; i++) {
        this.stoplichtKleur[i] = null; // Reset de kleur van de stoplichten naar null
        this.slagboomStatus[i] = false; // Sluit de slagbomen
        // Stuur resetbericht naar slagbomen van weg 'i'
        this.publishMessage(`asb/${i}`, '0');
        // Stuur berichten om de kleur van de stoplichten op nul te zetten
        this.publishMessage(`vkl${i}/rood`, '0');
        this.publishMessage(`vkl${i}/oranje`, '0');
        this.publishMessage(`vkl${i}/groen`, '0');
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
  right: 0;
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
  width: 50px;
  height: 10px;
  background-color: #555; /* Kleur wanneer de slagboom gesloten is */
  border: 2px solid #333; /* Rand om de slagbomen te benadrukken */
  position: absolute; 
  right: 50%; 
  transform: translateX(-50%); 
}

.slagboom.on {
  background-color: #2ecc71; 
}


</style>
