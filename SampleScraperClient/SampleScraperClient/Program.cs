using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScrapySharp.Core;
using ScrapySharp.Html.Parsing;
using ScrapySharp.Network;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html.Forms;
using System.Collections;

namespace SampleScraperClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup the browser
            ScrapingBrowser Browser = new ScrapingBrowser();
            Browser.AllowAutoRedirect = true; // Browser has many settings you can access in setup
            Browser.AllowMetaRedirect = true;
            WebPage[] webPages = new WebPage[31];
            List<List<string>> flights = new List<List<string>>();
            for (var i = 1; i <= 31; i++)
            {
                string day = "";
                //go to the home page
                if (i < 10)
                {
                     day = "0" + i;
                } else
                {
                    day = i.ToString();
                }
                WebPage PageResult = Browser.NavigateToPage(new Uri("http://www.norwegian.com/uk/booking/flight-tickets/select-flight/?D_City=OSL&A_City=RIX&TripType=1&D_Day="+ day + "&D_Month=201610&D_SelectedDay=02&R_Day=10&R_Month=201609&R_SelectedDay=10&CurrencyCode=GBP"));
                // get first piece of data, the page title
                // get a list of data from a table
                List<String> info = new List<string>();
                //List<List<string>> flights = new List<List<string>>();

                var tables = PageResult.Html.CssSelect(".avadaytable");
                //string[] info = new string[16];
                var y = 0;

                foreach (var table in tables)
                {

                    foreach (var row in table.SelectNodes("tbody/tr"))
                    {

                        foreach (var cell in row.SelectNodes("td"))
                        {
                            info.Add(cell.InnerText);
                        }

                        if (info.Count == 16)
                        {
                            flights.Add(info);
                            info = new List<string>();
                        }
                    }
                }
            }
            foreach(var item in flights)
            {
                //36 request
                //min 31
                //Console.WriteLine(item.ElementAt(2));
                Console.WriteLine("Departure airport:  arrival airport:  departure time:  arrival:  lovest price: " );
                if (item.ElementAt(2)=="Direct")
                {
                    Console.WriteLine(item.ElementAt(9) + "     " + item.ElementAt(10) + "              " + item.ElementAt(0) + "            " + item.ElementAt(1) + "     " + item.ElementAt(4));
                }
            }    
        }
    }
}
