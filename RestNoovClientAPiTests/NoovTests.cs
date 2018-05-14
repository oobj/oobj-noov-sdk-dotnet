using Microsoft.VisualStudio.TestTools.UnitTesting;
using Noov.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.IO;

namespace Noov.Rest.Tests
{
    [TestClass()]
    public class NoovTest
    {
        const String apiKeyNoPermission = "40a5fa55-dee1-4894-9a2e-966072962b92";
        const String apiSecretNoPermission = "90f26f36876e4840fba0867f2a249410dc67eaa2e60bd117622d0b8c1ad47f86";
        const String appNameNoPermission = "NoPermission";
        const String appDevEmailNoPermission = "demo@noov.com.br";

        const String apiKeyFullPermission = "meu api key";
        const String apiSecretFullPermission = "meu secret";
        const String appNameFullPermission = "nome do meu applicativo";
        const String appDevEmailFullPermission = "email do meu aplicativo";
        
        Noov NoPermissionInstance;
        Noov FullPermissionInstance;
        private String conteudoXml = null;

        [TestInitialize]
        public void Initialize()
        {
            NoPermissionInstance = Noov.CreateInstance(apiKeyNoPermission, apiSecretNoPermission, appNameNoPermission, appDevEmailNoPermission);
            FullPermissionInstance = Noov.CreateInstance(apiKeyFullPermission, apiSecretFullPermission, appNameFullPermission, appDevEmailFullPermission);
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("RestNoovClientAPiTests.Nfe300Proc.xml");

            using (StreamReader reader = new StreamReader(stream))
            {
                conteudoXml =  reader.ReadToEnd();
            }

        }

        [TestMethod()]
        public void GetTimestampDeveRetornarInteiroLongoMaiorQueZero()
        { 
            long result = NoPermissionInstance.getTimestamp();

            Assert.IsTrue(result > 0);
        }

        [TestMethod()]
        public void GetAuthTokenDeveObterUmTokenValidoQuandoAutenticado()
        {
            string authtoken = NoPermissionInstance.GetAuthToken();
            Assert.IsFalse(String.IsNullOrEmpty(authtoken));
        }

        [TestMethod()]
        [ExpectedException(typeof(WebException))]
        public void NaoDeveAceitarRequisicoesADocumentosSemAcessoDoAplicativo()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("docKey", "80221013000200");
            try { 
            NoPermissionInstance.MakeAuthenticatedGetRequest("/app/enrichment", parameters, "text/plain");
            }catch(WebException e)
            {
                Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
                throw;
            }
        }

        [TestMethod()]
        public void QualquerAutorizadoPodeSubmeterDocumentos()
        {
            string resposta = FullPermissionInstance.MakeAuthenticatedPostRequest("/app/dfe", conteudoXml, "application/xml");
            Assert.AreEqual("{\"meta\":{\"message\":\"Documento enviado com sucesso.\"}}", resposta);
        }

        //[TestMethod()]
        public void DeveSuportarRequisicoesDoTipoPutAutenticadas()
        {
            Assert.Fail();
        }


        //[TestMethod()]
        public void DeveSuportarRequisicoesDoTipoDeleteAutenticadas()
        {
            Assert.Fail();
        }
    }
}