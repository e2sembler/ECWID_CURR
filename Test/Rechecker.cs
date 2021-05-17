using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ECWID_CURR
{
     static class Rechecker
    {
        private static int Hour;
        internal static void Init()
        {
             const double CD = 3600000;//1hr
            var timer = new Timer(CD);
            timer.Elapsed += Timing;
            timer.AutoReset = true;
            timer.SynchronizingObject = null;
            if (System.Configuration.ConfigurationManager.AppSettings.Get("Час обновления") == null)
            { System.Windows.Forms.MessageBox.Show("В файле конфига отсутствует \"EUR_ID\" параметр"); Environment.Exit(0x1); }
            Hour = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("Час обновления"));
        }
         static private void Timing(object ebj, ElapsedEventArgs e)
        {
            if(DateTime.Now.Hour==Hour || Source.last_error)
            if (Source.thread_sleep)
                Weeb.Get_Currency_Update();
        }
    }
}
