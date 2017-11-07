using System.Linq;
using SampleSupport;

namespace Task
{
    public partial class LinqSamples
    {
        [Category("Restriction Operators")]
        [Title("Task 6")]
        [Description("This sample return the list of customers which indicate a non-digital postal code or the region is not filled or the operator code is not specified in the phone.")]
        public void Linq61()
        {
            var list = _dataSource.Customers
                .Where(x => !x.PostalCode.All(char.IsDigit) 
                    || string.IsNullOrEmpty(x.Region) 
                    || !x.Phone.StartsWith("(", System.StringComparison.Ordinal));

            Dump(list);
        }
    }
}
