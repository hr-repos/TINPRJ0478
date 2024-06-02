# install docker (full source="https://www.digitalocean.com/community/tutorials/how-to-install-and-use-docker-on-ubuntu-20-04")

    sudo apt update

    # install packages so that apt can use packages over HTTPS
    sudo apt install apt-transport-https ca-certificates curl software-properties-common

    # add GPG key for the official Docker repository to your system
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -

    # add the Docker repository to APT source
    sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable" # press ENTER to continue

    # check if you are about to install from the Docker repo instead of the default Ubuntu repo
    apt-cache policy docker-ce

    # install DOCKER
    sudo apt install docker-ce

    # to use "docker run" do
    sudo usermod -aG docker $USER
    newgrp docker
#end


# build docker image
cd opcua-server
sudo docker build -t opcua_server .

# run docker image
docker run --rm -it -p 8100:8100 opcua_server
