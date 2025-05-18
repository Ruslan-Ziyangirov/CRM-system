using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM_system.Models;
using CRM_system.Services;
using System.Collections.ObjectModel;

namespace CRM_system.ViewModels;

public partial class CreateDealViewModel : ObservableObject
{
    private readonly AppDatabase _db;

    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<ProductWithQuantity> SelectedProducts { get; } = new();

    [ObservableProperty]
    private int productQuantity = 1;

    [ObservableProperty] private string newCustomerContact = string.Empty;
    [ObservableProperty] private string newCustomerPhone = string.Empty;
    [ObservableProperty] private string newCustomerAddress = string.Empty;

    [ObservableProperty] private string newProductName = string.Empty;
    [ObservableProperty] private string newProductWholesale = string.Empty;
    [ObservableProperty] private string newProductRetail = string.Empty;
    [ObservableProperty] private string newProductDescription = string.Empty;

    [ObservableProperty]
    private Customer? selectedCustomer;

    [ObservableProperty]
    private DateTime dealDate = DateTime.Today;

    [ObservableProperty]
    private Product? newProductToAdd;

    public string Summary => $"Всего товаров: {SelectedProducts.Sum(p => p.Quantity)}, Сумма: {SelectedProducts.Sum(p => p.Total):C}";

    private readonly INavigation _navigation;

    public CreateDealViewModel(AppDatabase db)
    {
        _db = db;

        SelectedProducts.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Summary));
        LoadCustomersAsync();
        LoadProductsAsync();
    }

    public async Task LoadCustomersAsync()
    {
        var list = await _db.GetCustomersAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Customers.Clear();
            foreach (var c in list)
                Customers.Add(c);
        });
    }

    public async Task LoadProductsAsync()
    {
        var list = await _db.GetProductsAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Products.Clear();
            foreach (Product p in list)
                Products.Add(p);
        });
    }

    [RelayCommand]
    private void AddSelectedProduct()
    {
        if (NewProductToAdd is not null)
        {
            SelectedProducts.Add(new ProductWithQuantity
            {
                Product = NewProductToAdd,
                Quantity = ProductQuantity,
                UnitPrice = NewProductToAdd.RetailPrice
            });
            NewProductToAdd = null;
            ProductQuantity = 1;
        }
    }

    [RelayCommand]
    public async Task SaveDealAsync()
    {

        Console.WriteLine("👉 SaveDealAsync() вызван");

        if (SelectedCustomer is null || SelectedProducts.Count == 0)
        {
            Console.WriteLine("⛔ Не выбран покупатель или нет товаров");
            return;
        }


        if (SelectedCustomer is null || SelectedProducts.Count == 0)
            return;

        var sale = new Sale
        {
            CustomerId = SelectedCustomer.Id,
            Date = DealDate,
            IsWholesale = SelectedProducts.Sum(p => p.Quantity) >= 10,
            Status = "Новая"
        };

        await _db.SaveSaleAsync(sale);
        Console.WriteLine($"✅ Сделка сохранена: {sale.Id}");

        foreach (var p in SelectedProducts)
        {
            var item = new SaleItem
            {
                SaleId = sale.Id,
                ProductId = p.Product.Id,
                Quantity = p.Quantity,
                UnitPrice = p.UnitPrice,
                Discount = p.Quantity >= 10 ? p.UnitPrice * 0.1m : 0
            };
            await _db.SaveSaleItemAsync(item);
            Console.WriteLine($"🟢 Товар сохранен: {p.Product.Name}");
        }

        // Возврат на главную страницу
        await Shell.Current.DisplayAlert("Успешно", "Сделка сохранена", "Ок");
        await Shell.Current.GoToAsync("..");
        Console.WriteLine("🔁 Попытка возврата на главную страницу");
    }

    [RelayCommand]
    private async Task SaveNewCustomerAsync()
    {
        var customer = new Customer
        {
            ContactPerson = NewCustomerContact,
            Phone = NewCustomerPhone,
            Address = NewCustomerAddress
        };

        await _db.SaveCustomerAsync(customer);
        await LoadCustomersAsync(); // метод, который перезагружает ObservableCollection
    }

    [RelayCommand]
    private async Task SaveNewProductAsync()
    {
        var product = new Product
        {
            Name = NewProductName,
            WholesalePrice = decimal.Parse(NewProductWholesale),
            RetailPrice = decimal.Parse(NewProductRetail),
            Description = NewProductDescription
        };

        await _db.SaveProductAsync(product);
        await LoadProductsAsync(); // метод, который перезагружает ObservableCollection
    }


}

public partial class ProductWithQuantity : ObservableObject
{
    public Product Product { get; set; } = default!;
    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private decimal unitPrice;
    public decimal Total => Quantity * UnitPrice;

    partial void OnQuantityChanged(int oldValue, int newValue) =>
    OnPropertyChanged(nameof(Total));

    partial void OnUnitPriceChanged(decimal oldValue, decimal newValue) =>
        OnPropertyChanged(nameof(Total));
}