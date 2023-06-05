using WorkBench.UOMS;

namespace WorkBench.UOMS
{
    public class customPressureUOM : PressureUOM
    {
        public customPressureUOM(string customUOMname, double factor)
        {
            Name = customUOMname;
            Factor = factor;
        }
        public override string Name { get; }

        public override double Factor { get; }
    }

}