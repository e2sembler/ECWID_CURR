using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace ECWID_CURR
{
    static class IconBar
    {
        private static ContextMenu menu = new ContextMenu() ;
        private static NotifyIcon notify_icon = new NotifyIcon()
        {
            Visible = true,
            Icon = new Icon("Icon.ico"),
            Text="Идет инициализация"
        };
       public static void  Init()
        {
        menu.MenuItems.AddRange(new MenuItem[] { new MenuItem("Обновить данные", Update), new MenuItem("Выход", Exit) });
            notify_icon.ContextMenu = menu;
        }
        private static void Update(object e, EventArgs arg)
        {
            if(Source.thread_sleep) 
            System.Threading.Tasks.Task.Factory.StartNew(()=> Weeb.Get_Currency_Update());
        }

        internal static void Update_text(ref DateTime date, ref double USD, ref double EU)
        {
            notify_icon.Text = "Курс: " + date.ToString("MM/dd HH:mm")+"\n USD: "+USD.ToString("F")+"\n EUR: "+EU.ToString("F");
        }
        internal static void Update_text(string reason)
        {
            if (reason.Length > 64)
                notify_icon.Text = reason.Substring(0, 64);
            else
                notify_icon.Text = reason;
        }
        private static void Exit(object e, EventArgs arg)
        {
            Environment.Exit(0x1);
        }
        public static void No_Need_UPD(string reason)
        {
             notify_icon.ShowBalloonTip(5000, "Данные не обновлены", reason, ToolTipIcon.Info);
        }

    }
}
