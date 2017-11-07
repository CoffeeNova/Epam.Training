using System.Linq;
using SampleSupport;

namespace Task
{
    public partial class LinqSamples
    {
        [Category("Restriction Operators")]
        [Title("Task 4")]
        [Description("This sample return the list of customers with their first order date.")]
        public void Linq41()
        {
            var list = _dataSource.Customers
                .Select(x => new
                {
                    CustomerName = x.CompanyName,
                    FirstOrder = x.Orders.FirstOrDefault()?.OrderDate.ToString("MM.yyyy")
                });
            Dump(list);
        }
    }
}
