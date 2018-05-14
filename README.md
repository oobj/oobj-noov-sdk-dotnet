# oobj-noov-sdk-dotnet
Implementação .NET de interface para conexão com API REST Noov
## RestNoovClientAPi

Biblioteca contendo o componente que acessa a API. 

O componente a ser utilizado é o Noov.Rest.Noov

Criançao de instância: 
>const String apiKey = "40a5fa55-dee1-4894-9a2e-966072962b92";
const String apiSecret = "90f26f36876e4840fba0867f2a249410dc67eaa2e60bd117622d0b8c1ad47f86";
const String appName = "NoPermission";
const String appEmail = "demo@noov.com.br";
Noov NoovInstance = Noov.CreateInstance(apiKey, apiSecret, appName, appEmail);

Envio de arquivo:

>string resposta = Noov.MakeAuthenticatedPostRequest("/app/dfe", conteudoXml, "application/xml");


## ConsoleApplication

Aplciação de exemplo que instancia o componente e simula uma requisição

## RestNoovClientAPiTest

Testes do componente de acesso.
