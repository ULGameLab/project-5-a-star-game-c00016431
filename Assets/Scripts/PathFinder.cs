using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MapGen;

public class Node
{
    public Node cameFrom = null; //parent node
    public double priority = 0; // F value
    public double costSoFar = 0; // G Value
    public Tile tile;

    public Node(Tile _tile, double _priority, Node _cameFrom, double _costSoFar)
    {
        cameFrom = _cameFrom;
        priority = _priority; 
        costSoFar = _costSoFar;
        tile = _tile;
    }
}

public class PathFinder
{
    List<Node> TODOList = new List<Node>();
    List<Node> DoneList = new List<Node>();
    Tile goalTile = null;

    public PathFinder()
    {
    }

    // TODO: Find the path based on A-Star Algorithm
    public Queue<Tile> FindPathAStar(Tile start, Tile goal)
    {
        Queue<Tile> path = new Queue<Tile>();
        Node startNode = new Node(start, HeuristicsDistance(start, goal), null, 0);
        
        TODOList.Add(startNode);

        while(TODOList.Count != 0)
        {
            Node current = TODOList[0];

            for(int i = 0; i < TODOList.Count; i++)
            {
                if(TODOList[i].priority < current.priority)
                {
                    current = TODOList[i];
                }
            }

            TODOList.Remove(current);
            DoneList.Add(current);

            if (current.tile == goal)
            {
                path = RetracePath(current);
                break;
            }

            List<Node> neighbors = new List<Node>();
            for (int i = 0; i<current.tile.Adjacents.Count; i++)
            {
                neighbors.Add(new Node(current.tile.Adjacents[i], (current.costSoFar + 10) + (HeuristicsDistance(current.tile.Adjacents[i], goal)), current, current.costSoFar + 10));
            }

            for(int i = 0; i<neighbors.Count; i++)
            {
                if(neighbors[i].tile.isPassable && !TODOList.Contains(neighbors[i]) && !DoneList.Contains(neighbors[i]))
                {
                    TODOList.Add(neighbors[i]);
                }

            }
            
        }
        
        return path; // Returns optimized path
    }

    // TODO: Find the path based on A-Star Algorithm
    // In this case avoid a path passing near an enemy tile
    public Queue<Tile> FindPathAStarEvadeEnemy(Tile start, Tile goal)
    {
        Queue<Tile> path = new Queue<Tile>();
        Node startNode = new Node(start, HeuristicsDistance(start, goal), null, 0);

        TODOList.Add(startNode);

        while (TODOList.Count != 0)
        {
            Node current = TODOList[0];

            for (int i = 0; i < TODOList.Count; i++)
            {
                if (TODOList[i].priority < current.priority)
                {
                    current = TODOList[i];
                }
            }

            TODOList.Remove(current);
            DoneList.Add(current);

            if (current.tile == goal)
            {
                path = RetracePath(current);
                break;
            }

            List<Node> neighbors = new List<Node>();
            for (int i = 0; i < current.tile.Adjacents.Count; i++)
            {
                neighbors.Add(new Node(current.tile.Adjacents[i], (current.costSoFar + 10) + (HeuristicsDistance(current.tile.Adjacents[i], goal)), current, current.costSoFar + 10));
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].tile.isPassable && !TODOList.Contains(neighbors[i]) && !DoneList.Contains(neighbors[i]))
                {
                    TODOList.Add(neighbors[i]);
                }

            }

        }

        return path; // Returns optimized path
    }

    // Manhattan Distance with horizontal/vertical cost of 10
    double HeuristicsDistance(Tile currentTile, Tile goalTile)
    {
        int xdist = Math.Abs(goalTile.indexX - currentTile.indexX);
        int ydist = Math.Abs(goalTile.indexY - currentTile.indexY);
        // Assuming cost to move horizontally and vertically is 10
        //return manhattan distance
        return (xdist * 10 + ydist * 10);
    }

    // Retrace path from a given Node back to the start Node
    Queue<Tile> RetracePath(Node node)
    {
        List<Tile> tileList = new List<Tile>();
        Node nodeIterator = node;
        while (nodeIterator.cameFrom != null)
        {
            tileList.Insert(0, nodeIterator.tile);
            nodeIterator = nodeIterator.cameFrom;
        }
        return new Queue<Tile>(tileList);
    }

    // Generate a Random Path. Used for enemies
    public Queue<Tile> RandomPath(Tile start, int stepNumber)
    {
        List<Tile> tileList = new List<Tile>();
        Tile currentTile = start;
        for (int i = 0; i < stepNumber; i++)
        {
            Tile nextTile;
            //find random adjacent tile different from last one if there's more than one choice
            if (currentTile.Adjacents.Count < 0)
            {
                break;
            }
            else if (currentTile.Adjacents.Count == 1)
            {
                nextTile = currentTile.Adjacents[0];
            }
            else
            {
                nextTile = null;
                List<Tile> adjacentList = new List<Tile>(currentTile.Adjacents);
                ShuffleTiles<Tile>(adjacentList);
                if (tileList.Count <= 0) nextTile = adjacentList[0];
                else
                {
                    foreach (Tile tile in adjacentList)
                    {
                        if (tile != tileList[tileList.Count - 1])
                        {
                            nextTile = tile;
                            break;
                        }
                    }
                }
            }
            tileList.Add(currentTile);
            currentTile = nextTile;
        }
        return new Queue<Tile>(tileList);
    }

    private void ShuffleTiles<T>(List<T> list)
    {
        // Knuth shuffle algorithm :: 
        // courtesy of Wikipedia :) -> https://forum.unity.com/threads/randomize-array-in-c.86871/
        for (int t = 0; t < list.Count; t++)
        {
            T tmp = list[t];
            int r = UnityEngine.Random.Range(t, list.Count);
            list[t] = list[r];
            list[r] = tmp;
        }
    }
}
