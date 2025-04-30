namespace CRM_system.Views;
using CRM_system.ViewModels;
using CRM_system.Services;
using CRM_system.Models;

[QueryProperty(nameof(Database), "Database")]
public partial class CreateDealPage : ContentPage
{
    private AppDatabase _db;

    public AppDatabase Database
    {
        get => _db;
        set
        {
            _db = value;
            BindingContext = new CreateDealViewModel(_db);
        }
    }

    public CreateDealPage()
    {
        InitializeComponent();
    }
}