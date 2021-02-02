using DS = DataSource;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakePropValueFloat : DS.PropValueFloatBase
    {
        public float Value { get; set; }

        public override object GetValue()
        {
            return Value;
        }

        public override float GetValueDirect()
        {
            return Value;
        }
    }
}