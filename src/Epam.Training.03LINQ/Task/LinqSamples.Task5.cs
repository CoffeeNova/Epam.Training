using System.Linq;
using SampleSupport;

namespace Task
{
    public partial class LinqSamples
    {
        [Category("Restriction Operators")]
        [Title("Task 5")]
        [Description("This sample return the list of customers with their first order date.")]
        public void Linq51()
        {
            var list = _dataSource.Customers
                .OrderBy(x => x.Orders.FirstOrDefault()?.OrderDate)
                .ThenByDescending(x => x.Orders.FirstOrDefault()?.Total)
                .ThenBy(x => x.CompanyName)
                .Select(x => new
                 {
                     Custumer = x.CompanyName,
                     FirstOrder = x.Orders?.FirstOrDefault()?.OrderDate.ToString("MM.yyyy")
                 });
            Dump(list);
        }
    }
}
