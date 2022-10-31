using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace PIDtickets
{
    public class Places
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("lat")]
        public decimal Lat { get; set; }

        [JsonProperty("lon")]
        public decimal Lon { get; set; }

        [JsonProperty("hours")]
        public string OpeningHours { get; set; }
        public override string ToString()
        {
            return $"{ID}:{Type}:{Name}:{Address}:{OpeningHours}:{Lat}:{Lon}";
        }
    }
    public partial class MainPage : ContentPage
    {
        public Places p = new Places
        {
            ID = "dp3",
            Type = "ticketMachine",
            Name = "Pasáž Sofie",
            Address = "Sofijské náměstí 3400, Modřany, Praha 4",
            OpeningHours = "8:00-20:00",
            Lat = 50.00562m,
            Lon = 14.4182835m,
        };
        public MainPage()
        {
            InitializeComponent();
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            string inpu = input.Text;
            ComunicationWithAPI(inpu);
        }
        
        private async Task ComunicationWithAPI(string s)
        {
            HttpContent content = new StringContent(s, Encoding.UTF8, "application/json");
            HttpClient htc = new HttpClient();
            string address = "https://nominatim.openstreetmap.org/search?q=" + s + "&format=geojson"; //Proč yhazuje chybu? Chyba: Server neslyší:System.Net.Http.HttpRequestException: Nemohlo být vytvořeno žádné připojení, protože cílový počítač je aktivně odmítl.
                                                                                                      //--->System.Net.Sockets.SocketException(10061): Nemohlo být vytvořeno žádné připojení, protože cílový počítač je aktivně odmítl.
                                                                                                      //at System.Net.Http.ConnectHelper.ConnectAsync(String host, Int32 port, CancellationToken cancellationToken)
                                                                                                      //-- - End of inner exception stack trace-- -
             HttpResponseMessage response = await htc.PostAsync(address, content);
            Out.Text = response.ToString() + Environment.NewLine + response.IsSuccessStatusCode.ToString() + Environment.NewLine + response.StatusCode + Environment.NewLine + response.Content.ToString();
        }
    }
}
