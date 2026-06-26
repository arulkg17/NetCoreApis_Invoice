using ConsoleLab.TreeNode;
using System.Xml.Linq;

public class Program
{
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



    public static void Main()
    {
        // Tree No print for left and Right side nodes.
        BuildTreeAndRun.BuildAndRun();
    }
}