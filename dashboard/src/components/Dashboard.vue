<template>
  <div class="dashboard">
    <h1>Dashboard</h1>
    <div class="controls">
      <!-- Verkeerslichten knoppen -->
      <button @click="toggleVerkeerslicht('rood')" :class="{ 'on': verkeerslichtRood }">Rood</button>
      <button @click="toggleVerkeerslicht('oranje')" :class="{ 'on': verkeerslichtOranje }">Oranje</button>
      <button @click="toggleVerkeerslicht('groen')" :class="{ 'on': verkeerslichtGroen }">Groen</button>
      <!-- Slagbomen knoppen -->
      <button @click="toggleSlagboom(1)" :class="{ 'on': slagboomStatus1 }">Slagboom 1: {{ slagboomStatus1 ? 'Open' : 'Gesloten' }}</button>
      <button @click="toggleSlagboom(2)" :class="{ 'on': slagboomStatus2 }">Slagboom 2: {{ slagboomStatus2 ? 'Open' : 'Gesloten' }}</button>
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
        // Zorg ervoor dat de andere lichten uit zijn
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
      // Stuur een MQTT bericht met de nieuwe status
      this.client.publish(`verkeerslichten/${kleur}`, this[`verkeerslicht${kleur.charAt(0).toUpperCase() + kleur.slice(1)}`] ? '1' : '0');
    },
    toggleSlagboom(nummer) {
      this[`slagboomStatus${nummer}`] = !this[`slagboomStatus${nummer}`];
      this.client.publish(`slagboom/status${nummer}`, this[`slagboomStatus${nummer}`] ? '1' : '0');
    },
    connectMQTT() {
      this.client = mqtt.connect(this.mqttHost);
      this.client.on('connect', () => {
        this.client.subscribe(['verkeerslichten/rood', 'verkeerslichten/oranje', 'verkeerslichten/groen', 'slagboom/status1', 'slagboom/status2']);
      }); 
      this.client.on('message', (topic, message) => {
        // Logica om de statussen te updaten gebaseerd op de inkomende MQTT berichten
      });
      this.client.on('error', (error) => {
        console.error('MQTT-verbinding fout:', error);
      });
    }
  }
};
</script>

<style scoped>
.dashboard {
  max-width: 800px;
  margin: 20px auto;
  padding: 20px;
  border-radius: 15px;
  box-shadow: 0 2px 15px rgba(0, 0, 0, 0.1);
  background-color: #fff;
  text-align: center;
}

.controls {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  margin-bottom: 20px;
}

.controls button {
  margin: 10px;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: black;
}

.controls button:hover {
  opacity: 0.8;
}

.on {
  background-color: #4CAF50; /* Green */
}

.off {
  background-color: #f44336; /* Red */
}

/* Custom colors for traffic lights */
.verkeerslichten button {
  width: 100px; /* Ensure equal width for all traffic light buttons */
}

.rood.on {
  background-color: #ff4136; /* Red */
}

.oranje.on {
  background-color: #ff851b; /* Orange */
}

.groen.on {
  background-color: #2ecc40; /* Green */
}

.sidebar {
  position: fixed;
  top: 0;
  right: -350px;
  width: 300px;
  height: 100%;
  background-color: #f4f4f4;
  transition: right 0.3s ease;
  padding: 20px;
}

.sidebar.open {
  right: 0;
}

.sidebar h2 {
  margin-top: 0;
}

.sidebar ul {
  list-style-type: none;
  padding: 0;
}

.sidebar li {
  margin-bottom: 10px;
  font-size: 16px;
}

@media (max-width: 768px) {
  .dashboard {
    margin: 20px;
  }

  .controls {
    flex-direction: column;
  }

  .sidebar {
    position: static;
    width: auto;
    height: auto;
    padding-top: 20px;
  }

  .sidebar.open {
    right: initial;
  }
}
</style>

