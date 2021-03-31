using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Volume_Break_Webpage.CLsFolder;

namespace VolumeBreakOut.Models
{
    public class LoginM
    {
        [Required(ErrorMessage = "Username is required")] // make the field required
        [Display(Name = "Username")]  // Set the display name of the field
        public string username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        Ldap ldap = new Ldap();

        public bool IsValid(string _username, string _password)
        {
            bool retVal = false;

            try
            {

                string value = ldap.CheckLogin(_username, _password);

                if (value == "0")
                {
                    retVal = false;
                }
                else
                {
                    retVal = true;
                }
                return retVal;
            }
            catch (Exception ex)
            {
                retVal = false;
            }

            return retVal;

        }
    }
}