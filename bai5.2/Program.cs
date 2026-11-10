
using bai5._2;
using KaraokeManagement.GUI;
using System;
using System.Windows.Forms;

namespace KaraokeManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmMain());
        }
    }
}
