using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RoomsTreeNode 
{
    public bool IsChecked { get; set; }
    public MapCharValue[,] Value { get; set; }
    public RoomsTreeNode Parent { get; set; }
    public RoomsTreeNode Right { get; set; }
    public RoomsTreeNode Left { get; set; }
    public List<EntranceTrigger> EntranceTriggers { get; } = new List<EntranceTrigger>();
}

