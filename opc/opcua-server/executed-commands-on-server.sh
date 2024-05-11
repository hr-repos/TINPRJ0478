get .net core:
# Get Ubuntu version
declare repo_version=$(if command -v lsb_release &> /dev/null; then lsb_release -r -s; else grep -oP '(?<=^VERSION_ID=).+' /etc/os-release | tr -d '"'; fi)

# Download Microsoft signing key and repository
wget https://packages.microsoft.com/config/ubuntu/$repo_version/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

# Install Microsoft signing key and repository
sudo dpkg -i packages-microsoft-prod.deb

# Clean up
rm packages-microsoft-prod.deb

# Update packages
sudo apt update

# install ASP.net core V8
sudo apt install aspnetcore-runtime-8.0

# install .net core V8
sudo apt install dotnet-runtime-8.0

# install pip and opcua
sudo apt update
sudo apt install python3-pip
pip3 install opcua


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
sudo docker build -t opcua_server .

# run docker image
docker run --rm -it -p 8100:8100 opcua_server
