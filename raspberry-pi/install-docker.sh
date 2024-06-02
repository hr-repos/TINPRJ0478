# docker
sudo apt update
sudo apt upgrade
sudo reboot
curl -sSL https://get.docker.com | sh
sudo usermod -aG docker tunnel

# run docker compose
#ga naar de folder die de docker-compose.yml file heeft die je wilt runnen
#(-d is zo dat je de uitput niet op het scherm geprint krijgt)
sudo docker compose up -d
