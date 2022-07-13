using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.Milvus.Utils
{
    public static class CollectionUtils
    {
        public static bool IsNotEmpty(this IList list)
        {
            return list != null && list.Count > 0;
        }
    }
}
