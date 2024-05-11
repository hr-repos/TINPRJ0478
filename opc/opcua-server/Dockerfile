FROM python:3.10

# Create app directory
WORKDIR /app

RUN pip install opcua

# Bundle app source
COPY src /app

EXPOSE 8100
CMD [ "python", "OpcuaServer.py" ]