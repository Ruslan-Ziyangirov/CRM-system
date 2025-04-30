using CRM_system.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_system.Services
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _db;

        public AppDatabase(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<Product>().Wait();
            _db.CreateTableAsync<Customer>().Wait();
            _db.CreateTableAsync<Sale>().Wait();
            _db.CreateTableAsync<SaleItem>().Wait();
        }

        #region Product
        public Task<Customer> GetCustomerAsync(int id) =>_db.Table<Customer>().Where(c => c.Id == id).FirstOrDefaultAsync();
        public Task<int> SaveProductAsync(Product item) => item.Id != 0 ? _db.UpdateAsync(item) : _db.InsertAsync(item);
        public Task<int> DeleteProductAsync(Product item) => _db.DeleteAsync(item);
        #endregion

        #region Customer
        public Task<List<Customer>> GetCustomersAsync() => _db.Table<Customer>().ToListAsync();
        public Task<int> SaveCustomerAsync(Customer item) => item.Id != 0 ? _db.UpdateAsync(item) : _db.InsertAsync(item);
        public Task<int> DeleteCustomerAsync(Customer item) => _db.DeleteAsync(item);
        #endregion

        #region Sale
        public Task<List<Sale>> GetSalesAsync() => _db.Table<Sale>().ToListAsync();
        public Task<int> SaveSaleAsync(Sale item) => item.Id != 0 ? _db.UpdateAsync(item) : _db.InsertAsync(item);
        public Task<int> DeleteSaleAsync(Sale item) => _db.DeleteAsync(item);
        #endregion

        #region SaleItem
        public Task<List<SaleItem>> GetSaleItemsAsync(int saleId) => _db.Table<SaleItem>().Where(si => si.SaleId == saleId).ToListAsync();
        public Task<int> SaveSaleItemAsync(SaleItem item) => item.Id != 0 ? _db.UpdateAsync(item) : _db.InsertAsync(item);
        public Task<int> DeleteSaleItemAsync(SaleItem item) => _db.DeleteAsync(item);

        internal async Task<IEnumerable<object>> GetProductsAsync()
        {
            var list = await _db.Table<Product>().ToListAsync();
            return list.Cast<object>();
        }
        #endregion
    }
}
