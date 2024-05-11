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

# to build docker image
sudo docker build -t opcua_server .

# to run docker image
sudo docker run --rm -it -p 8100:8100 opcua_server
