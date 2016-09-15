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
                WebPage PageResult = Browser.NavigateToPage(new Uri("http://www.norwegian.com/uk/booking/flight-tickets/select-flight/?D_City=OSL&A_City=RIX&TripType=1&D_Day="+ day + "&D_Month=201610&D_SelectedDay="+ day +"&R_Day=" + day +"&R_Month=201610&R_SelectedDay=" +day+ "&CurrencyCode=GBP"));

                PageWebForm form = PageResult.FindFormById("aspnetForm");
                
                //if (i ==  2)
                //{
                //    form["__EVENTARGUMENT"] = "0|DY1072OSLRIX|9|0|0";
                //    form["ctl01$ctl00$MainContentRegion$MainRegion$ctl00$hifRouteIdOutbound"] = "DY1072OSLRIX";
                //    form["FlightSelectOutbound"] = "0|DY1072OSLRIX|9|0|0";
                //    form["__EVENTTARGET"] = "ctl01_ctl00_MainContentRegion_MainRegion_ctl00_ipcResultOutbound_lbtPostBackFaker";
                //    form["__ASYNCPOST"] = "true";
                //    form["ctl01$ctl00$MainContentRegion$MainRegion$ctl00$hifAvaDayColumnTypeOutbound"] = "1";
                //    form["ctl01$ctl00$ScriptManager$ScriptManager$ScriptManager1"] = "ctl01$ctl00$ScriptManager$ScriptManager$ScriptManager1|ctl01_ctl00_MainContentRegion_MainRegion_ctl00_ipcResultOutbound_lbtPostBackFaker";
                //    form["hdnFlightSelectOutboundStandardLowFarePlus0Exp"] = "0|DY1072OSLRIX|9|0|0";
                //    form.FormFields.RemoveAt(36);
                //    form.Method = HttpVerb.Post;
                //    form.Action = form.Action.Replace("amp;", "");
                //    WebPage resultPage = form.Submit();
                //    var temp = resultPage.Html.CssSelect("#bookingPrice_TaxesToggleIcon").ElementAt(0).ParentNode.ParentNode.NextSibling.InnerText;
                //    //var temp2 = resultPage.Html.
                //    Console.WriteLine(temp);
                //}
                //form[]
                // get first piece of data, the page title
                // get a list of data from a table
                List<String> info = new List<string>();
                //List<List<string>> flights = new List<List<string>>();

                var tables = PageResult.Html.CssSelect(".avadaytable");
                //string[] info = new string[16];
                var value = "";

                foreach (var table in tables)
                {

                    foreach (var row in table.SelectNodes("tbody/tr"))
                    {



                        foreach (var cell in row.SelectNodes("td"))
                        {
                            //if (cell.SelectNodes("input") != null)
                            //{
                            //    foreach (var item in cell.SelectNodes("input"))
                            //    {
                            //        var at = cell.Attributes;
                            //        var fso = cell.GetAttributeValue("FlightSelectOutbound");

                            //        var value = "";
                            //        value = cell.GetAttributeValue("value");
                            //    }
                            //}
                            //var title = cell.GetAttributeValue("title");
                            var temp = cell.InnerHtml.IndexOf("value");
                            if (temp > -1 && value=="")
                            {
                                var substring = cell.InnerHtml.Substring(temp);
                                var indefOf = substring.IndexOf(">");
                                value = substring.Substring(7, indefOf - 8);

                            }
                            info.Add(cell.InnerText);
                        }

                        if (info.Count == 16)
                        {
                            info.Add(form["ctl01$ctl00$MainContentRegion$MainRegion$ctl00$ipcFareCalendarSearchBar$ddlCurrency"]);
                            //Console.WriteLine(value);
                            form["__EVENTARGUMENT"] = value;
                            form["FlightSelectOutbound"] = value;
                            form["hdnFlightSelectOutboundStandardLowFarePlus0Exp"] = value;
                            form["__EVENTTARGET"] = "ctl01_ctl00_MainContentRegion_MainRegion_ctl00_ipcResultOutbound_lbtPostBackFaker";
                            form["__ASYNCPOST"] = "true";
                            form["ctl01$ctl00$MainContentRegion$MainRegion$ctl00$hifAvaDayColumnTypeOutbound"] = "1";
                            form["ctl01$ctl00$ScriptManager$ScriptManager$ScriptManager1"] = "ctl01$ctl00$ScriptManager$ScriptManager$ScriptManager1|ctl01_ctl00_MainContentRegion_MainRegion_ctl00_ipcResultOutbound_lbtPostBackFaker";
                           // var test = value.Substring(2, value.Length - 8);
                            form["ctl01$ctl00$MainContentRegion$MainRegion$ctl00$hifRouteIdOutbound"] = value.Substring(2,value.Length-8);
                            form.FormFields.RemoveAt(36);
                            form.Method = HttpVerb.Post;
                            form.Action = form.Action.Replace("amp;", "");
                            WebPage resultPage = form.Submit();
                            var temp = "";
                            if (resultPage.Html.CssSelect("#bookingPrice_TaxesToggleIcon").Count() > 0)
                            {
                                temp = resultPage.Html.CssSelect("#bookingPrice_TaxesToggleIcon").ElementAt(0).ParentNode.ParentNode.NextSibling.InnerText;
                            }
                            info.Add(temp);
                            flights.Add(info);
                            info = new List<string>();
                        }
                    }
                }
            }
            Console.WriteLine("Departure airport:  arrival airport:  connection airport:  departure time:  arrival:  lovest price: ");
            foreach (var item in flights)
            {
                //36 request
                //min 31
                //Console.WriteLine(item.ElementAt(2));
                string price = "";
                if(item.ElementAt(4) != "")
                {
                    price = item.ElementAt(4);
                } else if(item.ElementAt(6) != "")
                {
                    price = item.ElementAt(6);
                } else
                {
                    price = item.ElementAt(8);
                }
                var connectionAirport = "       ";
                var test = item.ElementAt(15).IndexOf("in ");
                var test2 = item.ElementAt(15).IndexOf("Departure ");
                
                if (test> 0)
                {
                    connectionAirport = item.ElementAt(15).Substring(test + 3);
                }
                if (test2 > 0)
                {
                    var space = item.ElementAt(15).Substring(test2 + 10).IndexOf(" ");
                    connectionAirport = item.ElementAt(15).Substring(test2 + 10,space);


                }
                //Console.WriteLine("Departure airport:  arrival airport:  connection airport:  departure time:  arrival:  lovest price: " );
                if (item.ElementAt(2) == "Direct") { 
               
                    Console.WriteLine(item.ElementAt(9) + "     " + item.ElementAt(10) + "              " + connectionAirport +"              " + item.ElementAt(0) + "            " + item.ElementAt(1) + "     " + price + "           " + item.ElementAt(17).Substring(2));
                }
            }    
        }
    }
}
