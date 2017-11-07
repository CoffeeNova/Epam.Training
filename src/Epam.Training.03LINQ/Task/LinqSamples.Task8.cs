using System.Linq;
using SampleSupport;

namespace Task
{
    public partial class LinqSamples
    {
        [Category("Restriction Operators")]
        [Title("Task 8")]
        [Description("This sample return products at a group price")]
        public void Linq81()
        {
            var biggestPrice = _dataSource.Products.OrderByDescending(x => x.UnitPrice).First().UnitPrice;
            var list = _dataSource.Products
                .GroupBy(x => new
                {
                    Cheap = x.UnitPrice <= biggestPrice / 3,
                    Middle = x.UnitPrice < biggestPrice / 3 && x.UnitPrice > 2 * biggestPrice / 3,
                    Expensive = x.UnitPrice >= 2 * biggestPrice / 3
                });
            Dump(list);
        }
    }
}
