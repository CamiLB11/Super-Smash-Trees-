using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2_SuperSmashTrees
{
    // Clase del nodo en AVLTree


    public class BSTree : ITree
    {
        Node root;

        public BSTree()
        {
            root = null;
        }

        public void insert(int key)
        {
            root = insertRecursive(root, key);
        }

        private Node insertRecursive(Node root, int key)
        {
            if (root == null)
            {
                root = new Node(key);
                return root;
            }

            if (key < root.key)
            {
                root.left = insertRecursive(root.left, key);
            }
            else if (key > root.key)
            {
                root.right = insertRecursive(root.right, key);
            }

            return root;
        }

        public void delete(int key)
        {
            root = deleteRecursive(root, key);
        }

        private Node deleteRecursive(Node root, int newKey)
        {
            if (root == null)
                return root;

            if (newKey < root.key)
                root.left = deleteRecursive(root.left, newKey);
            else if (newKey > root.key)
                root.right = deleteRecursive(root.right, newKey);
            else
            {
                if (root.left == null)
                    return root.right;
                else if (root.right == null)
                    return root.left;

                root.key = minValue(root.right);
                root.right = deleteRecursive(root.right, root.key);
            }
            return root;
        }

        private int minValue(Node root)
        {
            int minValue = root.key;
            while (root.left != null)
            {
                minValue = root.left.key;
                root = root.left;
            }
            return minValue;
        }


        public string PrintInOrder()
        {
            return inOrderRecursive(root);
        }

        private string inOrderRecursive(Node root)
        {
            if (root == null)
            {
                return "";
            }
            return inOrderRecursive(root.left) + root.key + " " + inOrderRecursive(root.right);
        }

        public string preOrder()
        {
            return preOrderRecursive(root);
        }

        private string preOrderRecursive(Node root)
        {
            if (root == null)
            {
                return "";
            }

            return root.key + " " + preOrderRecursive(root.left) + preOrderRecursive(root.right);
        }

        public string postOrder()
        {
            return postOrderRecursive(root);

        }

        private string postOrderRecursive(Node root)
        {
            if (root == null)
            {
                return "";
            }
            return postOrderRecursive(root.left) + postOrderRecursive(root.right) + root.key + " ";
        }

        public bool contains(int value)
        {
            return containsRecursive(root, value);
        }

        private bool containsRecursive(Node root, int value)
        {
            if (root == null)
                return false;
            if (value == root.key)
                return true;

            if (value < root.key)
                return containsRecursive(root.left, value);
            else
                return containsRecursive(root.right, value);
        }

        public int GetSize()
        {
            return GetSizeRecursive(root);
        }

        private int GetSizeRecursive(Node node)
        {
            if (node == null)
                return 0;
            return 1 + GetSizeRecursive(node.left) + GetSizeRecursive(node.right);
        }

        public bool RootTwoChildren()
        {
            return root != null && root.left != null && root.right != null;
        }

        public Node GetRoot()
        {
            return root;
        }
    }
}

