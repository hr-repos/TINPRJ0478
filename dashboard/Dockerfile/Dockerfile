FROM eclipse-mosquitto:latest

# Stel de standaard configuratie in
COPY mosquitto.conf /mosquitto/config/mosquitto.conf

# Stel gebruikers en wachtwoorden in (optioneel)
COPY passwordfile /mosquitto/config/passwordfile

# Expose MQTT port
EXPOSE 1883
