using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GameStore.WebUI.Helper
{
    public class CreditAuthorizationClient
    {

        public static String GenerateClientRequestHash(String pSecretValue, String pAppId, String pTransId)
        {
            try
            {
                String secretPartA = pSecretValue.Substring(0, 5);
                String secretPartB = pSecretValue.Substring(5, 5);
                String val = secretPartA + "-" + pAppId + "-" + pTransId + "-" + secretPartB;
                var pwdBytes = Encoding.UTF8.GetBytes(val);

                SHA256 hashAlg = new SHA256Managed();
                hashAlg.Initialize();
                var hashedBytes = hashAlg.ComputeHash(pwdBytes);
                var hash = Convert.ToBase64String(hashedBytes);
                return hash;
            }
            catch 
            {
                return null;
            }
        }

        public static bool VerifyServerResponseHash(String pHash, String pSecretValue, String pAppId, String pTransId, String pTransAmount, String pAppStatus)
        {
            String secretPartA = pSecretValue.Substring(0, 5);
            String secretPartB = pSecretValue.Substring(5, 5);
            String val = secretPartA + "-" + pAppId + "-" + pTransId + "-" + pTransAmount + "-" + pAppStatus + "-" + secretPartB;
            var pwdBytes = Encoding.UTF8.GetBytes(val);

            SHA256 hashAlg = new SHA256Managed();
            hashAlg.Initialize();
            var hashedBytes = hashAlg.ComputeHash(pwdBytes);
            var hash = Convert.ToBase64String(hashedBytes);

            if (hash == pHash)
                return true;
            else
                return false;
        }
    }
}