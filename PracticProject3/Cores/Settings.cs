using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticProject3.DownloadData;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Windows.Forms;
using System.IO;

namespace PracticProject3.Cores
{
    static class Settings
    {
        static public string FirstName;
        static public string SecondName;
        static public string ThirdName;
        static public int UserId = -1;
        static public string Permission;

        static public List<User> Users = new List<User>();
        static public List<Permission> Permissions = new List<Permission>();

        static string rsa_PublicKey = MakePublicKey();
        static string rsa_PrivateKey = MakePrivateKey();

        static public string EncryptString(string str) 
        {
            byte[] input = Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(str)));
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(rsa_PublicKey);
            return Convert.ToBase64String(rsa.Encrypt(input, true));
        }

        static public string DecryptString(string str) 
        {
            byte[] output = Convert.FromBase64String(str);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(rsa_PrivateKey);
            return Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(rsa.Decrypt(output, true))));
        }

        static public string ToHashSHA256(string str) 
        {
            SHA256 hash = SHA256.Create();
            return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "").ToLowerInvariant();
        }

        static public string ToHashMD5(string str)
        {
            MD5 hash = MD5.Create();
            return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "").ToLowerInvariant();
        }

        static string MakePublicKey() 
        {
            string xml = "";
            string Modulus = "1Yn9thiD4Ie50eKu3WoIx0DRrmCuqLniLFzxJbPg0wVMMJmVhaYVLHe/6fG7MzR8+lnWKoETs1rM+XtNIa4RCwr3S/At8FzhEb5I6RtDsBE4/pDBYoD5zBDWCuW4Mw2nDBuc8bimIJp6f4lgavefHs0DSXAJmbgekydGrauaxMU=";
            string Exponent = "AQAB";
            xml = $"<RSAKeyValue><Modulus>{Modulus}</Modulus><Exponent>{Exponent}</Exponent></RSAKeyValue>";
            return xml;
        }

        static string MakePrivateKey()
        {
            string xml = "";
            string Modulus = "1Yn9thiD4Ie50eKu3WoIx0DRrmCuqLniLFzxJbPg0wVMMJmVhaYVLHe/6fG7MzR8+lnWKoETs1rM+XtNIa4RCwr3S/At8FzhEb5I6RtDsBE4/pDBYoD5zBDWCuW4Mw2nDBuc8bimIJp6f4lgavefHs0DSXAJmbgekydGrauaxMU=";
            string Exponent = "AQAB";
            string P = "9z22lBftDdQEo5EPjksZ5QsA4LPLSZ1T8xN8+rNi/NBMHI5KiU4Vh9mCcTnaaxUuS1b9KCKS5XpLvMb93YbPow==";
            string Q = "3RqgT95EvJeyAjkoj1pvjlIl1KmLvgf9tRVy2e3lfDfudJsFaKH/FK1AEuDA9lbi/SfZdLxk0K7sgpn43FLAdw==";
            string DP = "B5r+Ys/ZmvNKsT1lHRtyUHlmLYNaXYlZa8DNt2iNQvmlXbuCz3Z3cxZdzN3aujCMnASxBOsLWcPJUKVOaeIbwQ==";
            string DQ = "V5Cq+taZPGOLgSvn71YuphvMlbuL93est+HFuZ9o6lgwXQfkDXtSzXqLzK4sXaMthHsauQu/lZIhYtvd3kv7ow==";
            string InverseQ = "tkPBlHMXrtzaaWClNBTvO/XZYh8P7um+SA36M03Hv95rSXw45IUtdRLqCDDgvhS+TjRngGCDCX+XDBlKtLEYRg==";
            string D = "fl3VvXAOgs3vIgyyjKbWIQvzyey/D2xJMxWhQz2Km3Y0G0KYIGojd5g6AvihXnD+UKzfN3IBNY9TY4QbJgY7YKdpM/yR9H35YvpA5v5/G70pPzk+oCeh3XqajjzzwchHoBr9yMuuKbSUC9TV9j2dYq98LfNW9F6SUsSOxU+VlVU=";
            xml = $"<RSAKeyValue><Modulus>{Modulus}</Modulus><Exponent>{Exponent}</Exponent><P>{P}</P><Q>{Q}</Q><DP>{DP}</DP><DQ>{DQ}</DQ><InverseQ>{InverseQ}</InverseQ><D>{D}</D></RSAKeyValue>";
            return xml;
        }
    }
}
