using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProject
{
    public class DodoPizzaForm
    {
        public string City { get; set; }
        public string AddressOfPizzeria { get; set; }
        public string JobPosition { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string PhoneNumber { get; set; }
        public string SocialMediaLink { get; set; }




        private DodoPizzaForm(string city, string addressofPizzeria, string jobPosition, string userName, string userLastName, string phoneNumber, string socialMediaLink)
        {
            City = city;
            AddressOfPizzeria = addressofPizzeria;
            JobPosition = jobPosition;
            UserName = userName;
            UserLastName = userLastName;
            PhoneNumber = phoneNumber;
            SocialMediaLink = socialMediaLink;
        }
        public static DodoPizzaForm GetDataFromDodoPizzaForm(string request)
        {
            if (string.IsNullOrEmpty(request))
                return null;
            string data = request.Split('?')[1];

            string[] requestTokens = data.Split('&');
            string city = GetProperty(requestTokens[0]);
            string addressOfPizzeria = GetProperty(requestTokens[1]);
            string jobPosition = GetProperty(requestTokens[2]);
            string userName = GetProperty(requestTokens[3]);
            string userLastName = GetProperty(requestTokens[4]);
            string phoneNumber = GetProperty(requestTokens[5]);
            string socialMediaLink = GetProperty(requestTokens[6]);
            
            return new DodoPizzaForm(city, addressOfPizzeria, jobPosition, userName, userLastName, phoneNumber, socialMediaLink);
        }

        private static string GetProperty(string lineData)
        {
            return lineData.Split('=')[1];
        }
    }
}
