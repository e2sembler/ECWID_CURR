using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Configuration;

namespace ECWID_CURR
{
    static class Weeb
    {
        private static int counter = 0;
        private static uint USD_ID, EUR_ID;
        private static ulong storeID;
        private static string Token;
        private static double USD, EU;
        private static DateTime last_update;
        static internal void Init()
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "Config.config");

            if (ConfigurationManager.AppSettings.AllKeys.Contains("Store ID"))
            storeID = Convert.ToUInt64(ConfigurationManager.AppSettings.Get("Store ID"));
            else {MessageBox.Show("В файле конфига отстутсвует \"Store ID\" параметр"); Environment.Exit(0x1);}
            if (ConfigurationManager.AppSettings.Get("Access token") != null)
                Token = ConfigurationManager.AppSettings.Get("Access token");
            else { MessageBox.Show("В файле конфига отсутствует \"Access token\" параметр"); Environment.Exit(0x1); }
            if (ConfigurationManager.AppSettings.Get("USD_ID") != null)
                USD_ID = Convert.ToUInt32(ConfigurationManager.AppSettings.Get("USD_ID"));
            else { MessageBox.Show("В файле конфига отсутствует \"USD_ID\" параметр"); Environment.Exit(0x1); }
            if (ConfigurationManager.AppSettings.Get("EUR_ID") != null)
                EUR_ID = Convert.ToUInt32(ConfigurationManager.AppSettings.Get("EUR_ID"));
            else { MessageBox.Show("В файле конфига отсутствует \"EUR_ID\" параметр"); Environment.Exit(0x1); }
        }
      static  public void Get_Currency_Update()
        {
            var WC = new WebClient();
            Source.thread_sleep = false;
            try {
                string json_string = WC.DownloadString("https://www.cbr-xml-daily.ru/daily_json.js");
                dynamic files = JsonConvert.DeserializeObject(json_string);
                if (last_update == (DateTime)files["Date"])
                {
                    IconBar.No_Need_UPD("Данные не являются устаревшими " + last_update);
                    Source.thread_sleep = true;
                    return;
                }
                USD = (double)files["Valute"]["USD"]["Value"];
                EU = (double)files["Valute"]["EUR"]["Value"];
                Get_Products(ref WC);
                last_update = (DateTime) files["Date"];
                IconBar.Update_text(ref last_update, ref USD, ref EU);
                counter = 0;
            }
            catch (WebException E)
            {
                IconBar.No_Need_UPD(E.Data.ToString());
                IconBar.Update_text(E.Message);
                Source.last_error = true;
            }
            finally
            {
                WC.Dispose();
                Source.thread_sleep = true;
            }
            }
        public class Obj
        {
            public attributes[] attributes = new attributes[1];
        }
        
        public class attributes
        {
            public uint id;
            public string value;
        }
        static private void Get_Products(ref WebClient WC)
        {
            WC.Encoding = Encoding.UTF8;
            WC.BaseAddress = "https://app.ecwid.com";
            string responce;
            var reply = new Obj();
            reply.attributes[0] = new attributes();
            dynamic array;
            for (uint i = 0, count, itemID, attributeID; ; i += 100)
            {
                 responce = WC.DownloadString("/api/v3/" + storeID + "/products?offset="+i+"&limit=100&token=" + Token+ "&sortBy=NAME_ASC");
                counter++;
                 array = JsonConvert.DeserializeObject(responce);
                count = (uint) array["items"].Count;
                if (count == 0)
                    break;
                for(int j=0; j<count; j++)
                {
                    itemID = Convert.ToUInt32(array["items"][j]["id"]);
                    for(int l=0; l<array["items"][j]["attributes"].Count;l++)
                    {
                        attributeID = Convert.ToUInt32(array["items"][j]["attributes"][l]["id"]);
                        if (attributeID == USD_ID || attributeID == EUR_ID)
                            Update_Product(ref WC, ref reply, itemID, attributeID, Convert.ToUInt32(array["items"][j]["price"]));
                    }
                }

            }
        }
        private static void Update_Product(ref WebClient WC, ref Obj reply, uint item_id, uint attr_id, uint price)
        {
            uint edit_price =(uint) (attr_id == USD_ID ? price/USD : price/EU);
            reply.attributes[0].id = attr_id;
            reply.attributes[0].value = edit_price.ToString("F");
            var answer = JsonConvert.SerializeObject(reply);
            WC.UploadString("/api/v3/" + storeID + "/products/" + item_id + "?token=" + Token, "PUT", answer);
            if (++counter == 599)
                System.Threading.Thread.Sleep(6001);
        }
    }
}
