
using System.Collections.ObjectModel;
using CRM_system.Services;
using CRM_system.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace CRM_system.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AppDatabase _db;

    public ObservableCollection<DealViewModel> Deals { get; } = new();

    public MainViewModel(AppDatabase db)
    {
        _db = db;

    }

    public async Task LoadDealsAsync()
    {
        Deals.Clear();
        var sales = await _db.GetSalesAsync();

        if (!sales.Any())
        {
            Console.WriteLine("⚠️ Нет сохранённых сделок в БД.");
            return;
        }

        foreach (var sale in sales)
        {
            var customer = await _db.GetCustomerAsync(sale.CustomerId);
            var items = await _db.GetSaleItemsAsync(sale.Id);
            Deals.Add(new DealViewModel
            {
                Date = sale.Date,
                CustomerName = customer.ContactPerson,
                TotalItems = items.Sum(i => i.Quantity),
                TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice),
                Discount = items.Sum(i => i.Discount),
                IsWholesale = sale.IsWholesale ? "Да" : "Нет"
            });
        }
    }
}

public class DealViewModel
{
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = "";
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public string IsWholesale { get; set; } = "";


}
