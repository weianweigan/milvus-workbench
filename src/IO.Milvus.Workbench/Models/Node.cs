using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.Milvus.Workbench.Models
{
    public abstract class Node
    {
        public string Name { get; set; }

        public string Description { get; set; }


    }

    public class MilvusManagerNode:Node
    {

    }
}
