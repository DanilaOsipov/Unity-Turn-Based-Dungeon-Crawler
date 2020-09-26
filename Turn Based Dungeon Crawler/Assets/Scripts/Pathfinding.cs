using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Pathfinding
{
    public static Pathfinding Instance { get; private set; }

    private List<PathNode> openList;
    private List<PathNode> closedList;
    private MapCharValue[,] map;
    private Dictionary<Transform, Vector3> objectPositionPairs;
    private PathNode[,] pathNodes;

    public void UpdatePosition(Transform transform, MapChar mapChar, Vector3 position)
    {
        Vector3 vector = objectPositionPairs[transform];
        int idx0 = (int)vector.x, idx1 = (int)vector.z;
        map[idx0, idx1].Value = MapChar.Empty;

        objectPositionPairs[transform] = position;
        idx0 = (int)position.x;
        idx1 = (int)position.z;

        map[idx0, idx1].Value = mapChar;
    }

    public Pathfinding(MapCharValue[,] map, Dictionary<Transform, Vector3> objectPositionPairs)
    {
        Instance = this;
        this.map = map;
        this.objectPositionPairs = objectPositionPairs;
        pathNodes = new PathNode[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < pathNodes.GetLength(0); i++)
        {
            for (int j = 0; j < pathNodes.GetLength(1); j++)
            {
                pathNodes[i, j] = new PathNode(new Vector3(i, 0, j));
            }
        }
    }

    public List<PathNode> FindPath(Transform from, Transform to)
    {
        for (int i = 0; i < pathNodes.GetLength(0); i++)
        {
            for (int j = 0; j < pathNodes.GetLength(1); j++)
            {
                pathNodes[i, j].GCost = int.MaxValue;
                pathNodes[i, j].CalculateFCost();
                pathNodes[i, j].CameFrom = null;
            }
        }

        Vector3 position = objectPositionPairs[from];
        PathNode startNode = pathNodes[(int)position.x, (int)position.z];

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        startNode.GCost = 0;
        startNode.HCost = CalculateDistanceCost(objectPositionPairs[from], objectPositionPairs[to]);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode.Position == objectPositionPairs[to])
                return CalculatePath(currentNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbour in GetNeighbours(currentNode))
            {
                if (closedList.Contains(neighbour)) continue;

                int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode.Position, 
                                                                               neighbour.Position);

                if (tentativeGCost < neighbour.GCost)
                {
                    neighbour.CameFrom = currentNode;
                    neighbour.GCost = tentativeGCost;
                    neighbour.HCost = CalculateDistanceCost(neighbour.Position,
                                                            objectPositionPairs[to]);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }

        return null;
    }

    private List<PathNode> GetNeighbours(PathNode currentNode)
    {
        List<PathNode> res = new List<PathNode>();

        Vector3 position = currentNode.Position;
        int idx0 = (int)position.x;
        int idx1 = (int)position.z;

        if (idx0 + 1 >= 0 && idx1 >= 0 &&
            (map[idx0 + 1, idx1].Value == MapChar.Empty ||
            map[idx0 + 1, idx1].Value == MapChar.Enemy))
            res.Add(pathNodes[idx0 + 1, idx1]);

        if (idx0 - 1 >= 0 && idx1 >= 0 &&
            (map[idx0 - 1, idx1].Value == MapChar.Empty ||
            map[idx0 - 1, idx1].Value == MapChar.Enemy))
            res.Add(pathNodes[idx0 - 1, idx1]);

        if (idx0 >= 0 && idx1 + 1 >= 0 &&
            (map[idx0, idx1 + 1].Value == MapChar.Empty ||
            map[idx0, idx1 + 1].Value == MapChar.Enemy))
            res.Add(pathNodes[idx0, idx1 + 1]);

        if (idx0 >= 0 && idx1 - 1 >= 0 &&
            (map[idx0, idx1 - 1].Value == MapChar.Empty ||
            map[idx0, idx1 - 1].Value == MapChar.Enemy))
            res.Add(pathNodes[idx0, idx1 - 1]);

        return res;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();

        path.Add(endNode);

        PathNode currentNode = endNode;

        while (currentNode.CameFrom != null)
        {
            path.Add(currentNode.CameFrom);
            currentNode = currentNode.CameFrom;
        }

        path.Reverse();

        return path;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodes)
    {
        PathNode lowestFCostNode = pathNodes[0];

        for (int i = 1; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].FCost < lowestFCostNode.FCost)
                lowestFCostNode = pathNodes[i];
        }

        return lowestFCostNode;
    }

    private int CalculateDistanceCost(Vector3 from, Vector3 to)
    {
        return (int)(Mathf.Abs(from.x - to.x) + Mathf.Abs(from.z - to.z)) * 10;
    }
}

