using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleLab.TreeNode;

public static class BuildTreeAndRun
{
    public static void BuildAndRun()
    {
        //     A
        //    / \
        //   B   C
        //  / \ / \
        //  D E F G


        Console.WriteLine(@"     A");
        Console.WriteLine(@"    / \");
        Console.WriteLine(@"   B   C");
        Console.WriteLine(@"  / \ / \");
        Console.WriteLine(@"  D E F G");
        Console.WriteLine("");

        Node a = new Node('A');
        Node b = new Node('B');
        Node c = new Node('C');
        Node d = new Node('D');
        Node e = new Node('E');
        Node f = new Node('F');
        Node g = new Node('G');

        a.Left = b;
        a.Right = c;
        b.Left = d;
        b.Right = e;
        c.Left = f;
        c.Right = g;

        Console.WriteLine("Printing left tree view");
        PrintLeftView(a);
        Console.WriteLine("");
        Console.ReadKey();
        Console.WriteLine("Printing Right tree view");
        PrintRightView(a);
        Console.ReadKey();
    }

    public static void PrintLeftView(Node root)
    {
        if (root == null) return;

        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            int levelCount = queue.Count;

            for (int i = 0; i < levelCount; i++)
            {
                Node current = queue.Dequeue();

                if (i == 0)
                    Console.Write(current.Value + " ");

                if (current.Left != null)
                    queue.Enqueue(current.Left);

                if (current.Right != null)
                    queue.Enqueue(current.Right);
            }
        }
    }

    public static void PrintRightView(Node root)
    {
        if (root == null) return;

        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            int levelCount = queue.Count;

            for (int i = 0; i < levelCount; i++)
            {
                Node current = queue.Dequeue();

                if (current.Left != null)
                    queue.Enqueue(current.Left);

                if (current.Right != null)
                    queue.Enqueue(current.Right);

                if (i == levelCount - 1)
                    Console.Write(current.Value + " ");
            }
        }
    }

}
