using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IMapObject
{
    void Move(Vector3 to);
    MapChar MapChar { get; }
}

