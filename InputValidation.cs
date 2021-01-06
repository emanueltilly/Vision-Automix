using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Vision_Automix
{
    class InputValidation
    {
        private static void ShowErrorMessage(string friendlyControlName, string description)
        {
            MessageBox.Show((friendlyControlName + " is incorrectly formatted. " + description + " The data will not be saved, please re-enter the data and save again."), "User input incorrectly formatted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static bool IpAddress(string friendlyControlName, string fieldA, string fieldB, string fieldC, string fieldD)
        {
            //Check that fields only contains numbers
            if (StringIsDigitsOnly(fieldA) && StringIsDigitsOnly(fieldB) && StringIsDigitsOnly(fieldC) && StringIsDigitsOnly(fieldD) != true)
            { ShowErrorMessage(friendlyControlName, "One or more of the IP address fields do contain character other than numbers."); return false; }



            //Check that string is a correct IP address
            string[] parts = new string[4] { fieldA, fieldB, fieldC, fieldD };
            if (parts.Length < 4)
            {
                ShowErrorMessage(friendlyControlName, "IP Address is not correctly formated. One or more fields have a length of more than 3 characters."); return false;
            }
            else
            {
                foreach (string part in parts)
                {
                    byte checkPart = 0;
                    if (!byte.TryParse(part, out checkPart))
                    {
                        ShowErrorMessage(friendlyControlName, "IP Address is not correctly formated. "); return false;
                    }
                }
                return true;
            }

        }

        public static bool Port(string friendlyControlName, string sourcePort)
        {
            if (StringIsDigitsOnly(sourcePort) != true)
            { ShowErrorMessage(friendlyControlName, "Port number contains one or more characters other than numbers."); return false; }
            else { return true; }
        }

        public static bool FriendlyName(string friendlyControlName, string sourceText)
        {
            if (sourceText.Length > 20) { ShowErrorMessage(friendlyControlName, "The name was over 20 characters long."); return false; }
            else { return true; }
        }

        private static bool StringIsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}
