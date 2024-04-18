<template>
  <div class="dashboard">
    <h1>Dashboard</h1>
    <div class="controls">
      <!-- Verkeerslicht controls voor elke weg -->
      <div v-for="i in 2" :key="'stoplicht' + i">
        <h2>Verkeerslicht {{ i }}</h2>
        <button @click="toggleStoplicht('rood', i)" :class="{ 'on red': stoplichtKleur[i] === 'rood' }">Rood</button>
        <button @click="toggleStoplicht('oranje', i)" :class="{ 'on orange': stoplichtKleur[i] === 'oranje' }">Oranje</button>
        <button @click="toggleStoplicht('groen', i)" :class="{ 'on green': stoplichtKleur[i] === 'groen' }">Groen</button>
        
        <!-- Verkeerslichten -->
        <div class="stoplicht">
          <div class="stoplicht-container">
            <div class="stoplicht-light red" :class="{ 'on': stoplichtKleur[i] === 'rood' }"></div>
            <div class="stoplicht-light orange" :class="{ 'on': stoplichtKleur[i] === 'oranje' }"></div>
            <div class="stoplicht-light green" :class="{ 'on': stoplichtKleur[i] === 'groen' }"></div>
          </div>
        </div>
        
        <!-- Slagboom control -->
        <button @click="toggleSlagboom(i)" :class="{ 'on': slagboomStatus[i] }">Slagboom {{ i }}: {{ slagboomStatus[i] ? 'Open' : 'Gesloten' }}</button>
      </div>
      <!-- Resetknop -->
      <h2>----------</h2>
      <button @click="resetAlles">Reset</button>
    </div>
    <div class="situatie-overzicht">
      <h2>Situatieoverzicht</h2>
      <ul>
        <li v-for="i in 2" :key="'situatie' + i">Stoplichtkleur {{ i }}: {{ stoplichtKleur[i] || 'Uit' }}</li>
        <li v-for="i in 2" :key="'slagboom' + i">Afsluitboom {{ i }}: {{ slagboomStatus[i] ? 'Open' : 'Gesloten' }}</li>
        <li>Verkeersintensiteit: {{ verkeersintensiteit }}</li>
      </ul>
    </div>
  </div>
</template>

<script>
import client from "@/assets/mqtt.js"; // Zorg dat dit pad klopt voor jouw projectstructuur

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
      this.stoplichtKleur[nummer] = this.stoplichtKleur[nummer] === kleur ? null : kleur;
      const topic = `vkl${nummer}/${kleur}`;
      const message = this.stoplichtKleur[nummer] ? '1' : '0';
      this.publishMessage(topic, message);
    },
    toggleSlagboom(nummer) {
      this.slagboomStatus[nummer] = !this.slagboomStatus[nummer];
      const topic = `asb/${nummer}`;
      const message = this.slagboomStatus[nummer] ? '1' : '0';
      this.publishMessage(topic, message);
    },
    resetAlles() {
      this.stoplichtKleur = {};
      this.slagboomStatus = {};
      this.verkeersintensiteit = 'laag';
      for (let i = 1; i <= 2; i++) {
        const topics = [`vkl${i}/rood`, `vkl${i}/oranje`, `vkl${i}/groen`, `asb/${i}`];
        topics.forEach(topic => this.publishMessage(topic, '0'));
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
  border-radius: 12px;
  box-shadow: 0px 8px 24px rgba(0, 0, 0, 0.1);
  width: 500px;
  padding: 25px;
  box-sizing: border-box;
  color: #2c3e50;
}
.controls {
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

/* Autoweg toevoeging */
.autoweg {
  margin-top: 20px;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.auto {
  width: 60px;
  height: 30px;
  background-color: #34495e; /* bijvoorbeeld */
  margin-bottom: 10px;
}
</style>
