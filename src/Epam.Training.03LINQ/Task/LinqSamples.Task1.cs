using System.Linq;
using SampleSupport;

namespace Task
{
    public partial class LinqSamples
    {
        private void Task1(decimal value)
        {
            var custumers = _dataSource.Customers.
                Where(x => x.Orders
                            .Sum(o => o.Total) > value);

            Dump(custumers);
        }

        [Category("Restriction Operators")]
        [Title("Task 1 - 100m")]
        [Description("This sample return return all custumers whose total orders more then set 100m")]
        public void Linq11()
        {
            var value = 100m;
            Task1(value);
        }

        [Category("Restriction Operators")]
        [Title("Task 1 - 20000")]
        [Description("This sample return return all custumers whose total orders more then set 20000")]
        public void Linq12()
        {
            var value = 20000m;
            Task1(value);
        }

        [Category("Restriction Operators")]
        [Title("Task 1 - 30000")]
        [Description("This sample return return all custumers whose total orders more then set 30000")]
        public void Linq13()
        {
            var value = 30000m;
            Task1(value);
        }

        [Category("Restriction Operators")]
        [Title("Task 1 - 40000")]
        [Description("This sample return return all custumers whose total orders more then set 40000")]
        public void Linq14()
        {
            var value = 40000m;
            Task1(value);
        }
    }
}
