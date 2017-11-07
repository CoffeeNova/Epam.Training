using System.Linq;
using SampleSupport;

namespace Task
{
    public partial class LinqSamples
    {
        [Category("Restriction Operators")]
        [Title("Task 7")]
        [Description("This sample return category products")]
        public void Linq71()
        {
            var list = _dataSource.Products
                .GroupBy(x => x.Category)
                .Select(x => new
                {
                    Category = x.Key,
                    Availability = x.GroupBy(y => y.UnitsInStock > 0)
                        .OrderBy(y => y.Last().UnitPrice)
                });

            Dump(list);
        }
    }
}
