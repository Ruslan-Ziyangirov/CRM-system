namespace CRM_system;
using CRM_system.Views;


    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(CreateDealPage), typeof(CreateDealPage));
        }
    }

