
using System.Collections.ObjectModel;
using CRM_system.Services;
using CRM_system.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace CRM_system.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AppDatabase _db;

    public string ProductNames { get; set; } = "";

    public ObservableCollection<DealViewModel> Deals { get; } = new();

    public List<string> StatusOptions { get; } = new() { "Новая", "Завершена", "Отменена" };

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

            // Получаем список названий продуктов
            var productNames = new List<string>();
            foreach (var item in items)
            {
                var product = await _db.GetProductAsync(item.ProductId);
                if (product != null)
                    productNames.Add(product.Name);
            }


            Deals.Add(new DealViewModel(_db, sale.Id)
            {
                Date = sale.Date,
                CustomerName = customer.ContactPerson,
                TotalItems = items.Sum(i => i.Quantity),
                TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice),
                Discount = items.Sum(i => i.Discount),
                IsWholesale = sale.IsWholesale ? "Да" : "Нет",
                ProductList = string.Join(", ", productNames),
                Status = sale.Status
            });
        }
    }
}

public class DealViewModel : ObservableObject

{

    private readonly AppDatabase _db;
    private readonly int _saleId;

    public DealViewModel(AppDatabase db, int saleId)
    {
        _db = db;
        _saleId = saleId;
    }

    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = "";
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public string IsWholesale { get; set; } = "";

    public string ProductList { get; set; } = "";

    public List<string> StatusOptions { get; set; } = new() { "Новая", "Завершена", "Отменена" };

    public string SelectedStatus
    {
        get => Status;
        set
        {
            if (Status != value)
            {
                Status = value;
                //Здесь можешь вызывать метод обновления в БД, если нужно
            }
        }
    }

    private string _status = "";
    public string Status
    {
        get => _status;
        set
        {
            SetProperty(ref _status, value);
            _ = UpdateStatusInDbAsync(value);
        }
    }

    private async Task UpdateStatusInDbAsync(string newStatus)
    {
        var sale = await _db.GetSaleAsync(_saleId);
        sale.Status = newStatus;
        await _db.SaveSaleAsync(sale);
    }


}
