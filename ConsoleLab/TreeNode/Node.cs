using System;
using System.Collections.Generic;
namespace ConsoleLab.TreeNode;
public class Node
{
    public char Value;
    public Node Left;
    public Node Right;

    public Node(char value)
    {
        Value = value;
    }
}