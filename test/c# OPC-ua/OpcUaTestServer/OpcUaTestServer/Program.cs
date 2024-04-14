using Opc.Ua.Configuration;
using Opc.Ua;
using Opc.Ua.Server;

//ApplicationInstance application = new()
//{
//    ApplicationName = "My OPC UA Server",
//    ApplicationType = ApplicationType.Server,
//    ConfigSectionName = "Opc.Ua.Server"
//};


ApplicationConfiguration config = new();
ServerBase server = new();


Uri uri = new("opc.tcp://127.0.0.1:8080");
server.CreateConnection(uri, 200);

config.ApplicationName = "testOpcuaServer";
config.ApplicationUri = "opc.tcp://127.0.0.1:8080";
config.CertificateValidator.AutoAcceptUntrustedCertificates = true;
config.ClientConfiguration = new();
config.CertificateValidator = new();

server.Start();
////await config.Validate(ApplicationType.ClientAndServer);
//ApplicationInstance applicationInstance = new(config);
//applicationInstance.StartAsService(server);


//checks if key exists if not he makes one
//var result = await applicationInstance.CheckApplicationInstanceCertificate(true, 0);

//await applicationInstance.Start(server);

//server.Start(config);