namespace GamePush
{
    public sealed class GP_Undefined
    {
        public static readonly GP_Undefined Value = new GP_Undefined();

        private GP_Undefined() { }

        public override string ToString() => "undefined";
    }
}
