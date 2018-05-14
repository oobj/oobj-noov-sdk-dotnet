using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Web.Script.Serialization;


namespace Noov.Rest
{
    /// <summary>
    /// Utilitário para de auxílio a utilização dos serviços REST da api NOOV
    /// </summary>
    public class Noov
    {

        #region CONSTANTES

        public const String URI_BASE = "http://rest.noov.com.br/v1";
        public const String URI_AUTHENTICATION = "/auth/login";
        public const String URI_TIMESTAMP = "/auth/timestamp";
        public const String ALGORITMO_HASH = "HMACSHA256";

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region ATRIBUTOS


        private String authToken;

        private String apiKey;
        private String apiSecret;
        private String appName;
        private String appDevEmail;
        #endregion

        #region INSTANCIACAO
        private Noov(String apiKey, String apiSecret, String appName, String appDevEmail)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.appName = appName;
            this.appDevEmail = appDevEmail;
        }

        /// <summary>
        /// Retorna uma instancia do objeto Noov configurada com credenciais de acesso a aplicação rest noov
        /// </summary>
        /// <param name="apiKey">chave de acesso a api</param>
        /// <param name="apiSecret">senha de acesso a api</param>
        /// <param name="appName">nome da aplicacao acessando a api</param>
        /// <param name="appDevEmail">email de desenvolvedor da aplicação acessando a api</param>
        /// <returns></returns>
        public static Noov CreateInstance(String apiKey, String apiSecret, String appName, String appDevEmail)
        {
            return new Noov(apiKey, apiSecret, appName, appDevEmail);
        }
        #endregion

        #region UTIL HTTP METHODS

        /// <summary>
        /// Requisita o recurso informado em uri no contexto do serviço rest do noov
        /// </summary>
        /// <param name="uri">endereço do recurso no contexto do noov (deve iniciar com /)</param>
        /// <param name="parameters">mapa de parametros a serem enviados na requisicao</param>
        /// <returns>Retorna o conteúdo do servidor em formato String</returns>
        public String MakeGetRequest(String uri, Dictionary<String, String> parameters) {
            return MakeGetRequest(uri, null, null);
        }

        /// <summary>
        /// Realiza uma requisição do tipo GET já autenticando, se necessário, com as credenciais informadas na instanciação
        /// deste objeto
        /// </summary>
        /// <param name="uri">enderço a ser requisitado (deve iniciar com /)</param>
        /// <param name="parameters">parâmetros da requisição</param>
        /// <param name="contentType">tipo do conteúdo</param>
        /// <returns></returns>
        public String MakeAuthenticatedGetRequest(String uri, Dictionary<String, String> parameters, String contentType)
        {
            return MakeGetRequest(uri, parameters, GetAuthHeader(contentType));
        }

        /// <summary>
        /// Requisita o recurso informado em uri no contexto do serviço rest do noov
        /// </summary>
        /// <param name="uri">endereço do recurso no contexto do noov (deve iniciar com /)</param>
        /// <param name="parameters">mapa de parametros a serem enviados na requisicao</param>
        /// <param name="headers">conjunto de chava/valor a serem adicionados no cabeçalho da requisicao</param>
        /// <returns>Retorna o conteúdo do servidor em formato String</returns>
        public String MakeGetRequest(String uri, Dictionary<String, String> parameters, NameValueCollection headers)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            if (parameters != null) {
                foreach (var item in parameters)
                {
                    client.QueryString.Add(item.Key, item.Value);
                }
            }
            
            if (headers != null)
            {
                client.Headers.Add(headers);
            }
#if DEBUG
            Console.WriteLine("Requisitando: GET " + URI_BASE + uri);
#endif
            return client.DownloadString(URI_BASE + uri);
        }

        /// <summary>
        /// Realiza uma requisição do tipo POST ao endereço apontado no serividor rest noov
        /// </summary>
        /// <param name="uri">resource a ser acessado (deve iniciar com /)</param>
        /// <param name="payload">conteúdo a ser enviado ao resource</param>
        /// <param name="headers">cabeçalho da requisição</param>
        /// <returns></returns>
        public String MakePostRequest(String uri, String payload, NameValueCollection headers)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(headers);
#if DEBUG
            Console.WriteLine("Requisitando: POST " + URI_BASE + uri);
#endif
            return client.UploadString(URI_BASE + uri, payload);
        }

        /// <summary>
        /// Realiza uma requisição POST já autenticando, se necessário ao endereço informado do servidor rest noov
        /// </summary>
        /// <param name="uri">resource a ser acessado (deve iniciar com /)</param>
        /// <param name="payload">conteúdo a ser enviado ao resource</param>
        /// <param name="contentType">tipo do conteúdo</param>
        /// <returns></returns>
        public String MakeAuthenticatedPostRequest(String uri, String payload, String contentType)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(GetAuthHeader(contentType));
#if DEBUG
            Console.WriteLine("Requisitando (ATENTICADO): POST " + URI_BASE + uri);
#endif
            return client.UploadString(URI_BASE + uri, payload);
        }


        /// <summary>
        /// Realiza uma requisição do tipo PUT ao endereço apontado no serividor rest noov
        /// </summary>
        /// <param name="uri">resource a ser acessado (deve iniciar com /)</param>
        /// <param name="payload">conteúdo a ser enviado ao resource</param>
        /// <param name="headers">cabeçalho da requisição</param>
        /// <returns></returns>
        public String MakePutRequest(String uri, String payload, NameValueCollection headers)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(headers);
            return client.UploadString(URI_BASE + uri, "PUT", payload);
        }

        /// <summary>
        /// Requisita que recurso(s) seja destruído(s) do servidor rest noov
        /// </summary>
        /// <param name="uri">endereço do recurso no contexto do noov (deve iniciar com /)</param>
        /// <param name="parameters">mapa de parametros a serem enviados na requisicao</param>
        /// <param name="headers">conjunto de chava/valor a serem adicionados no cabeçalho da requisicao</param>
        /// <returns>Retorna o conteúdo do servidor em formato String</returns>
        public String MakeDeleteRequest(String uri, Dictionary<String, String> parameters, NameValueCollection headers)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            if (headers != null)
            {
                client.Headers.Add(headers);
            }
            return Convert.ToString(client.UploadValues(URI_BASE + uri, "DELETE", toNameValueCollection(parameters)));
        }

#endregion

#region OPERATIONAL METHODS

        /// <summary>
        /// Faz uma requisicao ao servidor rest noov tentando obter um timestamp daquele servidor
        /// </summary>
        /// <returns>timestamp obtido do servidor ou timestamp local caso nao seja possivel obter resposta</returns>
        public long getTimestamp()
        {
            long timestamp = CurrentTimestamp();
            String result = MakeGetRequest(URI_TIMESTAMP, null);

            JavaScriptSerializer ser = new JavaScriptSerializer();

            IDictionary<string, object> jsonValues =  ParseJson(result);
            var strTimestamp = jsonValues["timestamp"];
            return Convert.ToInt64(strTimestamp);
        }

        /// <summary>
        /// Faz uma requisição de autenticação no servidor rest noov para obter um token de autorização de utilização
        /// da api
        /// </summary>
        /// <returns>um token de autorização que deverá ser utilizado em todas as requisições seguras da api</returns>
        public string GetAuthToken()
        {
            if (authToken != null)
            {
                return authToken;
            }
            else
            {
                long timestamp = getTimestamp();

                string hashSecret = EncodeToken(appName + appDevEmail + timestamp, apiSecret);

                var contentObject = new Dictionary<string, object>();
                contentObject.Add("apiKey", apiKey);
                contentObject.Add("timestamp", timestamp);
                contentObject.Add("secret", hashSecret);

                var jsonPayload = String.Format("{{ \"apiKey\": \"{0}\", \"timestamp\": \"{1}\", \"secret\": \"{2}\"}}",
                    apiKey, timestamp, hashSecret);

                String jsonResponse = MakePostRequest(URI_AUTHENTICATION, jsonPayload, new NameValueCollection { { "Content-Type", "application/json" } });

                authToken = GetToken(jsonResponse);

                return authToken;
            }
        }

#endregion

#region OTHER UTIL METHODS

        private NameValueCollection GetAuthHeader(string contentType)
        {
            var headers = new NameValueCollection();
            headers.Add("Content-Type", contentType);
            headers.Add("Authorization", "Bearer " + GetAuthToken());
            return headers;
        }

        private string GetToken(string jsonResponse)
        {
            var map = ParseJson(jsonResponse);
            if (map.ContainsKey("token"))
            {
                return Convert.ToString(map["token"]);
            } else
            {
                return string.Empty;
            }
        }

        private long ConvertTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }

        private long CurrentTimestamp()
        {
            TimeSpan elapsedTime = DateTime.Now - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }

        private IDictionary<string, object> ParseJson(String json)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            Dictionary<string, object> jsonValues = ser.Deserialize<Dictionary<string, object>>(json);
            return jsonValues;
        }


        private NameValueCollection toNameValueCollection(IDictionary<string, string> dictionary)
        {
            var result = new NameValueCollection();
            if (dictionary != null)
            {
                foreach (var item in dictionary)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        private string EncodeToken(string token, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            var mac = new HMACSHA256(key);
            var content = Encoding.UTF8.GetBytes(token);
            var hash = mac.ComputeHash(content);

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

#endregion
    }
}
