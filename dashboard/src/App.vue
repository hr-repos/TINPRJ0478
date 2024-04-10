<template>
  <div id="app">
    <Dashboard/>
<!--    <button @click="publishMessage">Publish Message</button>-->
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
  methods: {
    publishMessage() {
      const topic = 'example/topic';
      const message = 'Hello, MQTT!';
      client.publish(topic, message, (error) => {
        if (error) {
          console.error(`[MQTT] Publish error on topic '${topic}':`, error);
          return;
        }
        console.log(`[MQTT] Published message on topic '${topic}': ${message}`);
      });
    }
  }
};
</script>

<style>
/* Your styles here */
</style>
