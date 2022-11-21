using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace PIDtickets
{
    public partial class Places
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

        [JsonProperty("openingHours")]
        public OpeningHour[] OpeningHours { get; set; }
        public override string ToString()
        {
            return $"{ID}:{Type}:{Name}:{Address}:{OpeningHours}:{Lat}:{Lon}";
        }
        public double Distance { get; set; }
    }
    public partial class OpeningHour
    {
        [JsonProperty("from")]
        public long From { get; set; }

        [JsonProperty("to")]
        public long To { get; set; }

        [JsonProperty("hours")]
        public string Hours { get; set; }
    }
    public partial class CurrentPlace
    {
        [JsonProperty("features")]
        public Feature[] Features { get; set; }
    }

    public partial class Feature
    {
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
    }

    public partial class Geometry
    {
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }
    }
    public partial class MainPage : ContentPage
    {
        public static List<Places> places;
        public MainPage()
        {
            InitializeComponent();
            string jsonFileName = "Places.json";
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
            using (var reader = new System.IO.StreamReader(stream))
            {
                string jsonString = reader.ReadToEnd();
                List<Places> places = JsonConvert.DeserializeObject<List<Places>>(jsonString);
            }
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            CurrentPlace currentPlace = ComunicationWithAPI(input.Text).Result;
            Places[] nearestPlaces = Compare(currentPlace);
            foreach (var item in nearestPlaces)
            {
                Out.Text += item.Name + ", " + item.Address;
            }
        }

        private static async Task<CurrentPlace> ComunicationWithAPI(string s)
        {
            HttpContent content = new StringContent(s, Encoding.UTF8, "application/json");
            var htc = new HttpClient();
            htc.DefaultRequestHeaders.Add("User-Agent", "WHATEVER VALUE");
            string uri = "https://nominatim.openstreetmap.org/search?q=" + s + "&format=geojson";
            var response = await htc.PutAsync(uri, content);
            string result = await response.Content.ReadAsStringAsync();
            CurrentPlace currentPlace = JsonConvert.DeserializeObject<CurrentPlace>(result);
            return currentPlace;
        }
        private static Places[] Compare(CurrentPlace currentPlace)
        {
            Places[] nearestPlaces = new Places[3];
            nearestPlaces[0] = new Places();
            nearestPlaces[1] = new Places();
            nearestPlaces[2] = new Places();
            for (int i = 0; i < places.Count; i++)
            {
                double a = 0;
                double b = 0;
                if ((double)places[i].Lat > currentPlace.Features[0].Geometry.Coordinates[1])
                {
                    a = (double)places[i].Lat - currentPlace.Features[0].Geometry.Coordinates[1];
                }
                else
                {
                    a = currentPlace.Features[0].Geometry.Coordinates[1] - (double)places[i].Lat;
                }

                if ((double)places[i].Lon > currentPlace.Features[0].Geometry.Coordinates[0])
                {
                    b = (double)places[i].Lon - currentPlace.Features[0].Geometry.Coordinates[0];
                }
                else
                {
                    b = currentPlace.Features[0].Geometry.Coordinates[0] - (double)places[i].Lon;
                }
                double c = Math.Sqrt(a * a + b * b);

                if (c < nearestPlaces[0].Distance || nearestPlaces[0].Distance == 0)
                {
                    Places p = new Places();
                    p = places[i];
                    p.Distance = c;
                    nearestPlaces[0] = p;
                }
                else
                {
                    if (c < nearestPlaces[1].Distance || nearestPlaces[1].Distance == 0)
                    {
                        Places p = new Places();
                        p = places[i];
                        p.Distance = c;
                        nearestPlaces[1] = p;
                    }
                    else
                    {
                        if (c < nearestPlaces[2].Distance || nearestPlaces[2].Distance == 0)
                        {
                            Places p = new Places();
                            p = places[i];
                            p.Distance = c;
                            nearestPlaces[2] = p;
                        }
                    }
                }
            }
            return nearestPlaces;
        }
    }
}
