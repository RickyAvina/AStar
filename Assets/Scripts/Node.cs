using System.Collections.Generic;

internal class Node
{
    int nodeNumber;
    public int x;  // x pos in grid
    public int y;  // y pos in grid
    public int gCost;
    int hCost;
    public bool isObstacle = false;
    public int fCost;
    public Node parent;
    public List<Node> neighbors;   // Each neighbor has a GCOST (cost of moving there) and HCost (how far that node is from start) anf Fcost (G+H)
                            // These values are being recalculated every step
                            // if 2 nodes have the same fcost, use lowest HCost
                            // All of the neighbors are parented to the original node they came from, only parent if other noddes dont have parent or if new parent has better f ost

    public Node(int nodeNumber, int x, int y)
    {
        this.nodeNumber = nodeNumber;
        this.x = x;
        this.y = y;
        neighbors = new List<Node>();   // neighbors will be of varied list for every Node
        //this.hValue = hValue;
    }

    public static bool operator ==(Node n1, Node n2)
    {
        return n1.nodeNumber == n2.nodeNumber;
    }

    public static bool operator !=(Node n1, Node n2)
    {
        return n1.nodeNumber != n2.nodeNumber;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"Node {nodeNumber}";
    }
}