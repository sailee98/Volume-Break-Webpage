using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.DirectoryServices;

namespace Volume_Break_Webpage.CLsFolder
{
    public class Ldap
    {
        public String CheckLogin(string userName, string password)
        {
            string dominName = string.Empty;
            string adPath = string.Empty;
            string strError = string.Empty;

            try
            {
                dominName = System.Configuration.ConfigurationManager.ConnectionStrings["DirectoryDomain"].ConnectionString;
                adPath = System.Configuration.ConfigurationManager.ConnectionStrings["DirectoryPath"].ConnectionString;
                if (!String.IsNullOrEmpty(dominName) && !String.IsNullOrEmpty(adPath))
                {
                    if (true == AuthenticateUser(dominName, userName, password, adPath, out strError))
                    {
                        return "1";
                    }
                    else
                        return "0";
                    //dominName = string.Empty;
                    //adPath = string.Empty;

                }
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
            finally
            {

            }
        }

        public static bool AuthenticateUser(string domain, string username, string password, string LdapPath, out string Errmsg)
        {

            Errmsg = "";
            username = username.ToLower();
            DirectoryEntry entry = new DirectoryEntry(LdapPath, username, password);
            try
            {
                var credentials = new NetworkCredential(username, password, domain);



                // Bind to the native AdsObject to force authentication.

                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if (null == result)
                {
                    return false;
                }
                // Update the new path to the user in the directory
                LdapPath = result.Path;
                string _filterAttribute = (String)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                Errmsg = ex.Message;
                return false;
                throw new Exception("Error authenticating user." + ex.Message);
            }
            return true;
        }
    }
}