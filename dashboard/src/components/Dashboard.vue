<template>
  <div class="dashboard">
    <h1>Dashboard</h1>
    <div class="controls">
      <!-- Verkeerslichten knoppen -->
      <button @click="toggleVerkeerslicht('rood')" :class="{ 'on red': verkeerslichtRood }">Rood</button>
      <button @click="toggleVerkeerslicht('oranje')" :class="{ 'on orange': verkeerslichtOranje }">Oranje</button>
      <button @click="toggleVerkeerslicht('groen')" :class="{ 'on green': verkeerslichtGroen }">Groen</button>
      <!-- Slagbomen knoppen -->
      <button @click="toggleSlagboom(1)" :class="{ 'on slagboomOpen': slagboomStatus1, 'on slagboomClosed': !slagboomStatus1 }">Slagboom 1: {{ slagboomStatus1 ? 'Open' : 'Gesloten' }}</button>
      <button @click="toggleSlagboom(2)" :class="{ 'on slagboomOpen': slagboomStatus2, 'on slagboomClosed': !slagboomStatus2 }">Slagboom 2: {{ slagboomStatus2 ? 'Open' : 'Gesloten' }}</button>
    </div>
    <div class="sidebar" :class="{ 'open': sidebarOpen }">
      <h2>Status Overzicht</h2>
      <ul>
        <li>Verkeerslicht Rood: {{ verkeerslichtRood ? 'Aan' : 'Uit' }}</li>
        <li>Verkeerslicht Oranje: {{ verkeerslichtOranje ? 'Aan' : 'Uit' }}</li>
        <li>Verkeerslicht Groen: {{ verkeerslichtGroen ? 'Aan' : 'Uit' }}</li>
        <li>Slagboom 1: {{ slagboomStatus1 ? 'Open' : 'Gesloten' }}</li>
        <li>Slagboom 2: {{ slagboomStatus2 ? 'Open' : 'Gesloten' }}</li>
      </ul>
      <!-- Ultrasone sensor status -->
      <div class="sensor-status">
        <h2>Ultrasone Sensor Status</h2>
        <p>{{ ultrasoneSensorWaarde ? 'Object gedetecteerd' : 'Geen object gedetecteerd' }}</p>
      </div>
    </div>
  </div>
</template>

<script>
import mqtt from 'mqtt';
import config from '@/config'; // Zorg ervoor dat dit pad overeenkomt met de locatie van je config-bestand.

export default {
  name: 'Dashboard',
  data() {
    return {
      verkeerslichtRood: false,
      verkeerslichtOranje: false,
      verkeerslichtGroen: false,
      slagboomStatus1: false,
      slagboomStatus2: false,
      ultrasoneSensorWaarde: null, // Nieuwe data-eigenschap voor de sensorwaarde
      client: null,
      sidebarOpen: false,
      mqttHost: config.mqttHost
    };
  },
  created() {
    this.connectMQTT();
  },
  beforeDestroy() {
    if (this.client) {
      this.client.end();
    }
  },
  methods: {
    toggleVerkeerslicht(kleur) {
      // Update de juiste verkeerslicht status gebaseerd op 'kleur'
      if (kleur === 'rood') {
        this.verkeerslichtRood = !this.verkeerslichtRood;
        this.verkeerslichtOranje = false;
        this.verkeerslichtGroen = false;
      } else if (kleur === 'oranje') {
        this.verkeerslichtOranje = !this.verkeerslichtOranje;
        this.verkeerslichtRood = false;
        this.verkeerslichtGroen = false;
      } else if (kleur === 'groen') {
        this.verkeerslichtGroen = !this.verkeerslichtGroen;
        this.verkeerslichtRood = false;
        this.verkeerslichtOranje = false; 
      }
      this.client.publish(`verkeerslichten/${kleur}`, this[`verkeerslicht${kleur.charAt(0).toUpperCase() + kleur.slice(1)}`] ? '1' : '0');
    },
    toggleSlagboom(nummer) {
      this[`slagboomStatus${nummer}`] = !this[`slagboomStatus${nummer}`];
      this.client.publish(`slagboom/status${nummer}`, this[`slagboomStatus${nummer}`] ? '1' : '0');
    },
    connectMQTT() {
      this.client = mqtt.connect(this.mqttHost);
      this.client.on('connect', () => {
        this.client.subscribe([
          'verkeerslichten/rood', 
          'verkeerslichten/oranje', 
          'verkeerslichten/groen', 
          'slagboom/status1', 
          'slagboom/status2',
          'ultrasoneSensor/topic' // Abonneer op het nieuwe topic van de ultrasone sensor
        ]);
      }); 
      this.client.on('message', (topic, message) => {
        if (topic.startsWith('verkeerslichten/')) {
          let kleur = topic.split('/')[1];
          this[`verkeerslicht${kleur.charAt(0).toUpperCase() + kleur.slice(1)}`] = message.toString() === '1';
        } else if (topic.startsWith('slagboom/status')) {
          let nummer = topic.split('status')[1];
          this[`slagboomStatus${nummer}`] = message.toString() === '1';
        } else if (topic === 'ultrasoneSensor/topic') {
          this.ultrasoneSensorWaarde = message.toString(); // Update de sensorwaarde
        }
      });
      this.client.on('error', (error) => {
        console.error('MQTT-verbinding fout:', error);
      });
    }
  }
};
</script>

<style scoped>
body {
  background-color: #34495e; 
  font-family: 'Arial', sans-serif;
  color: #ffffff;
  height: 100vh;
  margin: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

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

.controls button.on.green {
  color: #fff;
  background-color: #2ecc71; /* Groen */
}

.controls button.on.red {
  color: #fff;
  background-color: #e74c3c; /* Rood */
}

.controls button.on.orange {
  color: #fff;
  background-color: #f39c12; /* Oranje */
}

.controls button.on.slagboomOpen {
  color: #fff;
  background-color: #2ecc71; /* Groen voor open */
}

.sidebar {
  background-color: #2c3e50; 
  border-radius: 8px;
  padding: 20px;
  color: #ecf0f1; 
  margin-top: 20px;
}

.sidebar h2 {
  border-bottom: 1px solid #ecf0f1; 
  padding-bottom: 10px;
  margin-bottom: 20px;
  font-size: 18px;
}

.sidebar ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.sidebar li {
  margin-bottom: 12px;
  font-size: 16px;
  line-height: 1.6;
}

.sidebar li:before {
  content: '•';
  color: #ecf0f1;
  font-weight: bold;
  display: inline-block; 
  width: 1em;
  margin-left: -1em; 
}

.sensor-status {
  margin-top: 25px;
  padding: 15px;
  background-color: #34495e;
  border-radius: 8px;
  text-align: center;
  font-size: 16px;
}

@media (max-width: 768px) {
  .dashboard {
    width: 90%; 
    margin: 20px;
  }
}

</style>