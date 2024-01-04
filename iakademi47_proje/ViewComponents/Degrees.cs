using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using System.Xml.Linq;

namespace iakademi47_proje.ViewComponents
{
    public class Degrees : ViewComponent
    {


        public string Invoke()

        {
            string apikey = "52b72dad903d5a0244a91d029fce3686";
            string city = "izmir";
            string url = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&mode=xml&lang=tr&units=metric&appid=" + apikey;

            XDocument weather = XDocument.Load(url);
            string temperature = weather.Descendants("temperature").ElementAt(0).Attribute("value").Value;

            //var iconurl = weather.Descendants("weather").ElementAt(0).Attribute("icon").Value;
            //ViewBag.icon = "https://api.openweathermap.org/img/w/" + iconurl + ".png";

            return $"{temperature} Derece";

        }
    }
}
