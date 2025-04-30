using CRM_system.Services;
using CRM_system.ViewModels;
using Microsoft.Maui.Controls;


namespace CRM_system.Views;

public partial class MainPage : ContentPage
{

    private readonly AppDatabase _db;
    private readonly MainViewModel _viewModel;

    public MainPage(AppDatabase db)
    {
        InitializeComponent();
        _db = db;
        _viewModel = new MainViewModel(db);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDealsAsync(); // Загрузка сделок при возврате на главную страницу
    }

    private async void OnAddDealClicked(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(CreateDealPage), typeof(CreateDealPage)); // Один раз можно
        await Shell.Current.GoToAsync($"{nameof(CreateDealPage)}", true, new Dictionary<string, object>
    {
        { "Database", _db }
    });


    }
}


