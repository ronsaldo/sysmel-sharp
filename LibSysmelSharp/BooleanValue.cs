
namespace LibSysmelSharp
{
    public class BooleanValue : Value
    {
        public bool value;

        BooleanValue(bool value) : base(null)
        {
            this.value = value;
        }

        private static BooleanValue falseSingletonValue = new BooleanValue(false);
        public static BooleanValue GetFalse()
        {
            return falseSingletonValue;
        }

        private static BooleanValue trueSingletonValue = new BooleanValue(true);
        public static BooleanValue GetTrue()
        {
            return trueSingletonValue;
        }
        public static BooleanValue GetWithValue(bool value)
        {
            return value ? GetTrue() : GetFalse();
        }

        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().BooleanType;
        }

        public override bool AsBoolean()
        {
            return value;
        }

        public override bool IsBooleanValue()
        {
            return true;
        }

    }
}
