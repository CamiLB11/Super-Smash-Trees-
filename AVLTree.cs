using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2_SuperSmashTrees
{
    public class Node
    {
        public int key, height;
        public Node left, right;

        public Node(int key)
        {
            this.key = key;
            this.height = 1;
            this.left = this.right = null;
        }
    }

    public class AVLTree : ITree
    {
        private Node root;

        private int height(Node node)
        {
            if (node == null) return 0;
            return node.height;
        }

        private int getBalance(Node node)
        {
            if (node == null)
                return 0;
            return height(node.right) - height(node.left);
        }

        public Node RotateLeft(Node node)
        {
            Node right = node.right;
            Node temp = right.left;

            right.left = node;
            node.right = temp;

            node.height = 1 + Math.Max(height(node.left), height(node.right));
            right.height = 1 + Math.Max(height(right.left), height(right.right));

            return right;
        }

        public Node RotateRight(Node node)
        {
            Node left = node.left;
            Node temp = left.right;

            left.right = node;
            node.left = temp;

            node.height = 1 + Math.Max(height(node.left), height(node.right));
            left.height = 1 + Math.Max(height(left.left), height(left.right));

            return left;
        }


        public Node rotateLeftRight(Node node)
        {
            RotateLeft(node.left);
            RotateRight(node);
            return node;
        }

        public Node rotateRightLeft(Node node)
        {
            RotateRight(node.right);
            RotateLeft(node);
            return node;
        }

        public void insert(int key)
        {
            root = insertRecursive(root, key);
        }

        private Node insertRecursive(Node node, int key)
        {
            if (node == null)
                return new Node(key);

            if (key < node.key)
                node.left = insertRecursive(node.left, key);
            else if (key > node.key)
                node.right = insertRecursive(node.right, key);
            else
                return node;

            node.height = 1 + Math.Max(height(node.left), height(node.right));

            int balance = getBalance(node);

            if (balance < -1 && key < node.left.key)
                return RotateRight(node);

            if (balance > 1 && key > node.right.key)
                return RotateLeft(node);

            if (balance < -1 && getBalance(node.left) > 0)
            {
                node.left = RotateLeft(node.left);
                return RotateRight(node);
            }

            if (balance > 1 && getBalance(node.right) < 0)
            {
                node.right = RotateRight(node.right);
                return RotateLeft(node);
            }

            return node;
        }

        public void erase(int key)
        {
            root = eraseRecursive(root, key);
        }

        private Node eraseRecursive(Node node, int key)
        {
            if (node == null)
                return null;

            if (key < node.key)
                node.left = eraseRecursive(node.left, key);
            else if (key > node.key)
                node.right = eraseRecursive(node.right, key);
            else
            {
                if (node.left == null)
                    return node.right;
                else if (node.right == null)
                    return node.left;

                Node minRight = GetMin(node.right);
                node.key = minRight.key;
                node.right = eraseRecursive(node.right, minRight.key);
            }

            node.height = 1 + Math.Max(height(node.left), height(node.right));

            int balance = getBalance(node);

            if (balance > 1 && getBalance(node.right) >= 0)
                return RotateLeft(node);

            if (balance > 1 && getBalance(node.right) < 0)
            {
                node.right = RotateRight(node.right);
                return RotateLeft(node);
            }

            if (balance < -1 && getBalance(node.left) <= 0)
                return RotateRight(node);

            if (balance < -1 && getBalance(node.left) > 0)
            {
                node.left = RotateLeft(node.left);
                return RotateRight(node);
            }


            return node;
        }

        private Node GetMin(Node node)
        {
            while (node.left != null)
                node = node.left;
            return node;
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


        public string PrintInOrder()
        {
            return PrintInOrderRecursive(root).Trim();
        }

        private string PrintInOrderRecursive(Node node)
        {
            if (node == null)
            {
                return "";
            }

            string left = PrintInOrderRecursive(node.left);
            string current = $"{node.key} ";
            string right = PrintInOrderRecursive(node.right);

            return left + current + right;
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
    