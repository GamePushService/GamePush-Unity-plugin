using System;

namespace GamePush.Core
{
    public class Helpers
    {
        public static T ConvertValue<T>(object value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
            
        }
    }
    
}
