using System;
using System.Linq;
using SampleSupport;

namespace Task
{
    public partial class LinqSamples
    {
        [Category("Restriction Operators")]
        [Title("Task 2 without grouping")]
        [Description("This sample return the list of customers and suppliers in same country and city")]
        public void Linq21()
        {
            var list = _dataSource.Customers
                .Select(x => new
                {
                    CustumerName = x.CompanyName,
                    Suppliers = _dataSource.Suppliers
                        .Where(s => string.Equals(x.Country, s.Country, StringComparison.OrdinalIgnoreCase) &&
                                    string.Equals(x.City, s.City, StringComparison.OrdinalIgnoreCase))
                        .Select(s => s.SupplierName)
                })
                .Where(x => x.Suppliers.Any());
            Dump(list);
        }

        [Category("Restriction Operators")]
        [Title("Task 2 with grouping")]
        [Description("This sample return return the list of customers and suppliers in same country and city")]
        public void Linq22()
        {
            var list = _dataSource.Customers
                .GroupJoin(_dataSource.Suppliers,
                    x => new { x.City, x.Country },
                    y => new { y.City, y.Country },
                    (x, y) => new { CustomerName = x.CompanyName, Suppliers = y.Select(s => s.SupplierName) })
                .Where(x => x.Suppliers.Any());
            Dump(list);
        }
    }
}
