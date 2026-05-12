using System;
using System.Windows.Forms;
using AirportManagement.Views;

namespace AirportManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
        }
    }
}
