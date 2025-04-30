using CRM_system.Services;
using System.IO;
using Microsoft.Maui.Storage;

namespace CRM_system

{
    public partial class App : Application
    {
        public AppDatabase Database { get; }
        public App()
        {
            InitializeComponent();
            string dbPath = Path.Combine(
        FileSystem.AppDataDirectory, "crm_database.db3");
            Database = new AppDatabase(dbPath);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}