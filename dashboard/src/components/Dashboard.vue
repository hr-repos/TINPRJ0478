<template>
  <div class="dashboard">
    <h1>Dashboard</h1>
    <div class="controls">
      <button @click="toggleStoplicht('rood')" :class="{ 'on red': stoplichtKleur === 'rood' }">Rood</button>
      <button @click="toggleStoplicht('oranje')" :class="{ 'on orange': stoplichtKleur === 'oranje' }">Oranje</button>
      <button @click="toggleStoplicht('groen')" :class="{ 'on green': stoplichtKleur === 'groen' }">Groen</button>
      <button @click="toggleSlagboom(1)" :class="{ 'on': slagboomStatus1 }">Slagboom 1: {{ slagboomStatus1 ? 'Open' : 'Gesloten' }}</button>
      <button @click="toggleSlagboom(2)" :class="{ 'on': slagboomStatus2 }">Slagboom 2: {{ slagboomStatus2 ? 'Open' : 'Gesloten' }}</button>
      <button @click="resetAlles">Reset</button>
    </div>
    <div class="stoplicht">
      <h2>Stoplicht</h2>
      <div class="stoplicht-container">
        <div class="stoplicht-light red" :class="{ 'on': stoplichtKleur === 'rood' }"></div>
        <div class="stoplicht-light orange" :class="{ 'on': stoplichtKleur === 'oranje' }"></div>
        <div class="stoplicht-light green" :class="{ 'on': stoplichtKleur === 'groen' }"></div>
      </div>
    </div>
    <div class="situatie-overzicht">
      <h2>Situatieoverzicht</h2>
      <ul>
        <li>Stoplichtkleur: {{ stoplichtKleur || 'Uit' }}</li>
        <li>Slagboom 1: {{ slagboomStatus1 ? 'Open' : 'Gesloten' }}</li>
        <li>Slagboom 2: {{ slagboomStatus2 ? 'Open' : 'Gesloten' }}</li>
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
      stoplichtKleur: null,
      slagboomStatus1: false,
      slagboomStatus2: false,
      verkeersintensiteit: 'laag',
    };
  },
  methods: {
    toggleStoplicht(kleur) {
      this.stoplichtKleur = this.stoplichtKleur === kleur ? null : kleur;
      const topic = `vkl/${kleur}`;
      const message = this.stoplichtKleur ? '1' : '0';
      this.publishMessage(topic, message);
    },
    toggleSlagboom(nummer) {
      this[`slagboomStatus${nummer}`] = !this[`slagboomStatus${nummer}`];
      const topic = `asb/${nummer}`;
      const message = this[`slagboomStatus${nummer}`] ? '1' : '0';
      this.publishMessage(topic, message);
    },
    resetAlles() {
      this.stoplichtKleur = null;
      this.slagboomStatus1 = false;
      this.slagboomStatus2 = false;
      this.verkeersintensiteit = 'laag';
      
      const topics = ['vkl/rood', 'vkl/oranje', 'vkl/groen', 'asb/1', 'asb/2'];
      topics.forEach(topic => this.publishMessage(topic, '0'));
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
  width: 400px;
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
  width: 90px;
}

.controls button:hover {
  background-color: #bdc3c7;
}

.controls button.on.red {
  color: #fff;
  background-color: #e74c3c; /* Rood */
}

.controls button.on.orange {
  color: #fff;
  background-color: #f39c12; /* Oranje */
}

.controls button.on.green {
  color: #fff;
  background-color: #2ecc71; /* Groen */
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
</style>
