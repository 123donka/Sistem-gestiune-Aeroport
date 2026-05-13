using System;
using System.Windows.Forms;
using AirportManagement.Views;
using AirportManagement.Data;

namespace AirportManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            DatabaseInitializer.EnsureDatabase();
            Application.Run(new LoginForm());
        }
    }
}
