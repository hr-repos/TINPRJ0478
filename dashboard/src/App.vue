<template>
  <div id="app">
    <Dashboard/>
  </div>
</template>

<script>
import Dashboard from './components/Dashboard.vue';
import client from "@/assets/mqtt.js";

export default {
  name: 'App',
  components: {
    Dashboard
  },
  async mounted() {
    client.on("message", (topic, message) => {
      // als er een mqtt message is..
    })

    await client.publish('asb', 'true', (error) => {
      if (error) return

      // publish een mqtt bericht..
      console.log(`[MQTT] Published message on topic '${topic}': ${message}`)
    })
  }
};
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
