using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
namespace ECWID_CURR
{
    class Source
    {
        public static bool thread_sleep { get; set; } = true;
        public static bool last_error { get; set; } = false;
        [STAThread]
        static void Main()
        {
            var init = new Initialization();
           init.Init();
            Task.Factory.StartNew(() => Weeb.Get_Currency_Update());
            System.Windows.Forms.Application.Run();
        }
    }
}
