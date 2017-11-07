using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SampleSupport;
using Task.Data;

namespace Task
{
    public partial class LinqSamples
    {
        [Category("Restriction Operators")]
        [Title("Task 2 without grouping")]
        [Description("This sample return return the list of customers and suppliers in same country and city")]
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
            var comparer = new SuplierComparer();

            var list = _dataSource.Customers
                .Select(x => new
                {
                    CustumerName = x.CompanyName,
                    Suppliers = _dataSource.Suppliers
                        .GroupBy(s => s, comparer)
                        .Select(s => s.Key.SupplierName)
                })
                .Where(x => x.Suppliers.Any());
            Dump(list);
        }

        private class SuplierComparer : IEqualityComparer, IEqualityComparer<Supplier>
        {
            public bool Equals(object x, object y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(object obj)
            {
                throw new NotImplementedException();
            }

            public bool Equals(Supplier x, Supplier y)
            {
                if (x == null || y == null)
                    return false;

                return string.Equals(x.Country, y.Country, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(x.City, y.City, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(Supplier obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
