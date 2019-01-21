using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar : MonoBehaviour
{
    [Tooltip("Resolution of the grid, ex: 10 will produce 10x10 grid")]
    public int n;

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

    void Start()
    {
        initializeNodes();  // Init nodes with default values
        //findPath();
    }

    public void findPath()
    {
        while (true) // not reached goal or explored every node
        {
            currentNode = openNodes[findNodeWithLowestFCost()]; // Node with the highest fVal
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);   // Node has already been explored

            if (currentNode == goalNode)    // path has been found
            {
                return;
            }

            for (int i = 0; i < currentNode.neighbors.Count; i++)
            {
                if (!currentNode.neighbors[i].isObstacle && !inClosedSet(currentNode.neighbors[i]))   // make sure neighbors aren't obstacle and haven't been explored yet
                {
                    // calculate g,h,f values for nodes
                    currentNode.neighbors[i].gCost = currentNode.gCost + costToTravelToNode(currentNode.neighbors[i], currentNode);
                    // set parents of nodes 
                    // check if the movement cost from neighbor node back to origin is smaller
                    //if (new path to neighbor is shorter OR !inOpenSet(currentNode.neighbors[i])) {

                    //}
                }
            }
        }
    }


    //private int pathToNeighbor(Node currentNode, Node neighborNode, int totalCost = 0)
    //{
    //    // go down gCost of traversing through neighbors
    //    Node parent = currentNode.parent;

    //    if (parent == null) {
    //        return totalCost;
    //    }
    //    else {
    //        totalCost += costToTravelToNode(currentNode, parentNode);

    //        return 1;
    //    }
    //}

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
        int lowest = openNodes[0].fCost;
        int index = 0;
        for (int i = 0; i < openNodes.Count; i++)
        {
            if (openNodes[i].fCost < lowest)
            {
                lowest = openNodes[i].fCost;
                index = i;
            }
        }
        return index;
    }

    void initializeNodes()
    {
        openNodes = new List<Node>();
        closedNodes = new List<Node>();

        int count = 0;
        grid = new List<List<Node>>(n);

        for (int i = 0; i < n; i++)
        {
            List<Node> temp = new List<Node>(n);
            for (int j = 0; j < n; j++)
            {
                Node node = new Node(count, i, j);    // add Node with nodeNumber
                temp.Add(node);
                count++;
            }
            grid.Add(temp);
        }

        // initialize the starting Node and goal Nodes
        startNode = grid[startingX][startingY];
        startNode.gCost = 0;        // starting Node has a gCost of 0
        startNode.fCost = 0;
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
