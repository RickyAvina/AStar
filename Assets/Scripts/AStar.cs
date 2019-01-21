using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar : MonoBehaviour
{
    [Tooltip("Resolution of the grid, ex: 10 will produce 10x10 grid")]
    public int n;

    public double D = 1;
    public double D2 = Math.Sqrt(2);

    [Tooltip("Coordinates of the startingNode")]
    public int startingX, startingY;
    [Tooltip("Coordinates of the goalNode")]
    public int goalX, goalY;

    private Node startNode;
    private Node goalNode;

    List<List<Node>> grid;  // Reference to the entire n x n grid of nodes

    List<Node> openNodes;   // List of nodes for which F-Cost have been calculated
    List<Node> closedNodes; // List of nodes that have already been evaluated

    private Node currentNode;   // used for tracking in loop
    List<Node> path;            // the final path


    void Start()
    {
        initializeNodes();  // Init nodes with default values
        findPath();
    }

    public void findPath()
    {
        while (true) // not reached goal or explored every node
        {
            currentNode = openNodes[findNodeWithLowestFCost()]; // Node with the highest fVal
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);   // Node has already been explored

            if (currentNode.nodeNumber == goalNode.nodeNumber)    // path has been found
            {
                Debug.Log($"We've reached the end at node {currentNode.nodeNumber}\nat pos ({currentNode.x},{currentNode.y})!");
                path = new List<Node>();
                path = getPath(currentNode, path);
                printPath(path);
                return;
            }

            //Debug.Log($"Num neighbors: {currentNode.neighbors.Count}");
            for (int i = 0; i < currentNode.neighbors.Count; i++)
            {
                if (!currentNode.neighbors[i].isObstacle && !inClosedSet(currentNode.neighbors[i]))   // make sure neighbors aren't obstacle and haven't been explored yet
                {
                    //  || !inOpenSet(currentNode.neighbors[i])

                    if (currentNode.neighbors[i].parent == null || pathIsShorter(currentNode, currentNode.neighbors[i].parent)) {    // parent neighbor node to current node if current node doesn't have a parent or if the current node provides a better path than their current parent
                        // set fcost of neighbor, for now, we precompute fcost, so there's no need
                        currentNode.neighbors[i].parent = currentNode;  // set parent

                        if (!inOpenSet(currentNode.neighbors[i]))
                        {
                            openNodes.Add(currentNode.neighbors[i]);
                        }
                    }
                }
            }
        }
    }

    private double calculateDist(Node node, Node node2)
    {
        // This is only an approximation, real distances can be calculated later if needed
        // Educlidean distance - http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html

        double dx = Math.Abs(node.x - node2.x);
        double dy = Math.Abs(node.y - node2.y);
        return D * Math.Sqrt(dx * dx + dy * dy);
    }

    private bool pathIsShorter(Node n1, Node n2)
    {
        return calculatePathCost(n1) > calculatePathCost(n2);
    }

    private List<Node> getPath(Node node, List<Node> nodes)
    {
        if (node.parent == null)
        {
            return nodes;
        }

        nodes.Add(node);
        return getPath(node.parent, nodes);
    }

    private void printPath(List<Node> nodes)
    {
        string _str = "";
        for (int i = 0; i < nodes.Count; i++)
        {
            _str += nodes[i].ToString() + "\n";
            
        }

        print(_str);
    }

    private int calculatePathCost(Node node, int cost=0)
    {
        if (node.parent == null)
        {
            return cost;
        }

        return calculatePathCost(node.parent, cost + costToTravelToNode(node, node.parent));
    }

    private int costToTravelToNode(Node currentNode, Node parentNode)   // currentNode can be though as the neighbor
    {
        return Convert.ToInt32(10*Math.Sqrt(Math.Pow(currentNode.y - parentNode.y, 2) + Math.Pow(currentNode.x - parentNode.x, 2)));
    }

    private bool inClosedSet(Node node)
    {
        return closedNodes.Contains(node);
    }

    private bool inOpenSet(Node node)
    {
        return openNodes.Contains(node);
    }

    private int findNodeWithLowestFCost()
    {
        // finds neighbor nodes with lowest f cost then h cost if tied
        return openNodes.IndexOf(openNodes.OrderBy(s => s.fCost).ThenBy(s => s.hCost).ToList()[0]);
    }

    void initializeNodes()
    {
        openNodes = new List<Node>();
        closedNodes = new List<Node>();

        int count = 0;
        grid = new List<List<Node>>(n);     // Initializing the empty grid of nodes
        startNode = new Node(-1, startingX, startingY);     // keep only a temporary reference to the start node to calculate h/g values for nodes
        goalNode = new Node(-2, goalX, goalY);

        for (int i = 0; i < n; i++)                         
        {
            List<Node> temp = new List<Node>(n);
            for (int j = 0; j < n; j++)
            {
                Node node = new Node(count, i, j);    // add Node with nodeNumber
                node.gCost = calculateDist(node, startNode);   // distance from node to start
                node.hCost = calculateDist(node, goalNode);
                temp.Add(node);
                count++;
            }
            grid.Add(temp);
        }

        // initialize the starting Node and goal Nodes
        startNode = grid[startingX][startingY];
        startNode.gCost = 0;        // starting Node has a gCost of 0
        //startNode.fCost = 0;
        openNodes.Add(startNode);  // add startingNOdes to openNodes for use in the first iteration of findPath

        goalNode = grid[goalX][goalY];          // keep reference to the goal node
        initializeNodeNeighbors();
    }

    void initializeNodeNeighbors()
    {
        // initialize neighbors for all Nodes

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++) // assuming n x n grid
            {
                    if (i > 0 && j > 0) // make sure top left can be added
                    {
                        grid[i][j].neighbors.Add(grid[i - 1][j - 1]);
                    }
                    if (i < n-1 && j > 0) {     // top right
                        grid[i][j].neighbors.Add(grid[i + 1][j - 1]);
                    }
                    if (i > 0) // make sure left can be added
                    {
                        grid[i][j].neighbors.Add(grid[i - 1][j]);
                    }
                    if (j > 0) // make sure top can be added
                    {
                        grid[i][j].neighbors.Add(grid[i][j - 1]);
                    }
                    if (j < n-1)  // make sure bottom can be added
                    {
                        grid[i][j].neighbors.Add(grid[i][j + 1]);
                    }
                    if (i < n-1) // make sure right can be added
                    {
                        grid[i][j].neighbors.Add(grid[i + 1][j]);
                    }
                    if (i < n-1 && j < n-1) // make sure bottom right can be added
                    {
                        grid[i][j].neighbors.Add(grid[i + 1][j + 1]);
                    }
                    if (i > 0 && j < n - 1) // bottom left
                    {
                        grid[i][j].neighbors.Add(grid[i - 1][j + 1]);
                    }
            }
        }
    }
}
