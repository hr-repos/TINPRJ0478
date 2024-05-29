import mqtt from 'mqtt'

//'ws://mq.nl.eu.org:8884'
const client = mqtt.connect(`ws://${window.location.hostname}:8801`, {
//const client = mqtt.connect(`ws://10.3.141.1:8801`, {
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
