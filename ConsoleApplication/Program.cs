using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        const String apiKeyNoPermission = "40a5fa55-dee1-4894-9a2e-966072962b92";
        const String apiSecretNoPermission = "90f26f36876e4840fba0867f2a249410dc67eaa2e60bd117622d0b8c1ad47f86";
        const String appNameNoPermission = "NoPermission";
        const String appDevEmailNoPermission = "demo@noov.com.br";

        static void Main(string[] args)
        {
            try
            {
                var noov = Noov.Rest.Noov.CreateInstance(apiKeyNoPermission, apiSecretNoPermission, appNameNoPermission, appDevEmailNoPermission);

                Console.WriteLine("Timestamp: {0}", noov.getTimestamp());

                Console.WriteLine("AuthToken: {0}", noov.GetAuthToken());

                Dictionary< string, string> prms = new Dictionary<string, string>();
                prms.Add("type", "STORE");
                Console.WriteLine("MakeAuthenticatedGetRequest: {0}", noov.MakeAuthenticatedGetRequest("/app/group", prms, "application/xml"));

            }catch (Exception e) {
                Console.WriteLine("Deu ruim: {0}", e.Message );
            }
        }
    }
}
