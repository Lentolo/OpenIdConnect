namespace TCPOS.Authentication.Utils.Extensions
{
    internal static class ChainExtensions
    {
        public static T ChainIf<T>(this T obj, bool condition, Action<T> action)
        {
            if(condition)
            {
                action(obj);
            }

            return obj;
        }
    }
}
