using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathNode
{
    public Vector3 Position { get; }

    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get; set; }

    public PathNode CameFrom { get; set; }

    public PathNode(Vector3 position)
    {
        Position = position;
    }

    public void CalculateFCost()
    {
        FCost = GCost + HCost;
    }
}

