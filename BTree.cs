using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2_SuperSmashTrees
{
    public class BTreeNode
    {
        public List<int> Keys;
        public List<BTreeNode> Children;
        public bool IsLeaf;
        public int MinDegree;

        public BTreeNode(int t, bool isLeaf)
        {
            this.MinDegree = t;
            this.IsLeaf = isLeaf;
            this.Keys = new List<int>(2 * t - 1);
            this.Children = new List<BTreeNode>(2 * t);
        }

        public int FindKey(int key)
        {
            int idx = 0;
            while (idx < Keys.Count && Keys[idx] < key)
                ++idx;
            return idx;
        }
    }

    public class BTree : ITree
    {
        private BTreeNode root;
        private int minDegree;
        private int nodeCount = 0;
        private int keyCount = 0;

        public BTree(int t)
        {
            this.root = null;
            this.minDegree = t;
        }

        public void insert(int key)
        {
            if (root == null)
            {
                root = new BTreeNode(minDegree, true);
                root.Keys.Add(key);
                nodeCount = 1;
                keyCount = 1;
                return;
            }

            if (root.Keys.Count == 2 * minDegree - 1)
            {
                BTreeNode newRoot = new BTreeNode(minDegree, false);
                newRoot.Children.Add(root);
                SplitChild(newRoot, 0);
                root = newRoot;
                nodeCount++;
            }

            InsertNonFull(root, key);
        }

        private void SplitChild(BTreeNode parent, int index)
        {
            BTreeNode child = parent.Children[index];
            BTreeNode newChild = new BTreeNode(minDegree, child.IsLeaf);
            parent.Keys.Insert(index, child.Keys[minDegree - 1]);

            for (int j = 0; j < minDegree - 1; j++)
                newChild.Keys.Add(child.Keys[j + minDegree]);

            child.Keys.RemoveRange(minDegree - 1, minDegree);

            if (!child.IsLeaf)
            {
                for (int j = 0; j < minDegree; j++)
                    newChild.Children.Add(child.Children[j + minDegree]);

                child.Children.RemoveRange(minDegree, minDegree);
            }

            parent.Children.Insert(index + 1, newChild);
            nodeCount++;
        }

        private void InsertNonFull(BTreeNode node, int key)
        {
            int i = node.Keys.Count - 1;

            if (node.IsLeaf)
            {
                while (i >= 0 && key < node.Keys[i])
                {
                    i--;
                }
                node.Keys.Insert(i + 1, key);
                keyCount++;
            }
            else
            {
                while (i >= 0 && key < node.Keys[i])
                {
                    i--;
                }
                i++;

                if (node.Children[i].Keys.Count == 2 * minDegree - 1)
                {
                    SplitChild(node, i);
                    if (key > node.Keys[i])
                    {
                        i++;
                    }
                }
                InsertNonFull(node.Children[i], key);
            }
        }

        public string PrintInOrder()
        {
            if (root == null)
                return "";

            StringBuilder sb = new StringBuilder();
            InOrderTraversal(root, sb);
            return sb.ToString().Trim();
        }

        private void InOrderTraversal(BTreeNode node, StringBuilder sb)
        {
            int i;
            for (i = 0; i < node.Keys.Count; i++)
            {
                if (!node.IsLeaf)
                    InOrderTraversal(node.Children[i], sb);

                sb.Append(node.Keys[i] + " ");
            }

            if (!node.IsLeaf)
                InOrderTraversal(node.Children[i], sb);
        }

        public int GetSize()
        {
            return keyCount;
        }

        public bool RootTwoChildren()
        {
            return root != null && !root.IsLeaf && root.Children.Count >= 2;
        }

        public Node GetRoot()
        {
            // Como BTree tiene una estructura diferente, creamos un nodo
            // sólo para propósitos de visualización
            if (root == null)
                return null;

            Node displayNode = new Node(root.Keys.Count > 0 ? root.Keys[0] : 0);
            ConvertToDisplayNodes(root, displayNode);
            return displayNode;
        }

        private void ConvertToDisplayNodes(BTreeNode btreeNode, Node displayNode)
        {
            if (btreeNode == null || btreeNode.Keys.Count == 0)
                return;

            // Para simplificar la visualización, solo usamos la primera clave de cada nodo
            displayNode.key = btreeNode.Keys[0];

            if (!btreeNode.IsLeaf && btreeNode.Children.Count > 0)
            {
                // Crear nodo izquierdo si hay hijos
                if (btreeNode.Children.Count > 0 && btreeNode.Children[0] != null)
                {
                    displayNode.left = new Node(btreeNode.Children[0].Keys.Count > 0 ? btreeNode.Children[0].Keys[0] : 0);
                    ConvertToDisplayNodes(btreeNode.Children[0], displayNode.left);
                }

                // Crear nodo derecho para representar el resto de hijos
                if (btreeNode.Children.Count > 1 && btreeNode.Children[1] != null)
                {
                    displayNode.right = new Node(btreeNode.Children[1].Keys.Count > 0 ? btreeNode.Children[1].Keys[0] : 0);
                    ConvertToDisplayNodes(btreeNode.Children[1], displayNode.right);
                }
            }
        }

        // Métodos para los retos de B-Tree
        public int CountInternalNodes()
        {
            if (root == null)
                return 0;

            return CountInternalNodesRecursive(root);
        }

        private int CountInternalNodesRecursive(BTreeNode node)
        {
            if (node == null)
                return 0;

            int count = node.IsLeaf ? 0 : 1;

            if (!node.IsLeaf)
            {
                foreach (var child in node.Children)
                {
                    count += CountInternalNodesRecursive(child);
                }
            }

            return count;
        }

        public int CountKeys()
        {
            return keyCount;
        }
    }
}
