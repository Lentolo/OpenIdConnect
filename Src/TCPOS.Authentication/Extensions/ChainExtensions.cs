using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPOS.Authentication.Extensions
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
