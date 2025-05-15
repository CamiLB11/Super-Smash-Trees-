using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2_SuperSmashTrees
{
    public interface ITree
    {
        void insert(int value);
        string PrintInOrder();
        int GetSize();
        bool RootTwoChildren();
        Node GetRoot();
    }
}
