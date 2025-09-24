using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.SwUtils
{
    public static class DynamicCast
    {
        public static T Cast<T>(this object obj)
        {
            if (obj == null)
            {
                if (typeof(T).IsValueType)
                {
                    throw new Exception();
                }
            }
            return (T)obj;
        }

        public static IList<T> CastList<T>(dynamic obj)
        {
            IList<T> result = new List<T>();
            if (obj != null)
            {
                foreach (var item in obj)
                {
                    result.Add((T)(item));
                }
            }
            return result;
        }
    }
}
