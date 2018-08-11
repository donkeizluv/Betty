using System;
using System.DirectoryServices.Protocols;

namespace Betty.Auth
{
    static public class WindowsAuth
    {
        //Pretty fast
        //additional logic
        public static bool Authenticate(string username, string password, string domain)
        {
            try
            {
                //string userdn;
                using (var lconn = new LdapConnection(new LdapDirectoryIdentifier(domain)))
                {
                    lconn.Bind(new System.Net.NetworkCredential(username, password, domain));
                    return true;
                }
            }
            catch (LdapException)
            {
                return false;
            }
        }
    }
}
