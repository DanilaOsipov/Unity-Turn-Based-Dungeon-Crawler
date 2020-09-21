using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : MonoBehaviour, IComparer<RoomsTreeNode>
{
    private MapCharValue[,] map;
    private float gridsOffset;
    private Dictionary<string, Vector3> obs;
    private RoomsTreeNode roomsTreeRoot;
    private List<RoomsTreeNode> rooms;
    private int roomsCounter;

    public void Initialize(MapCharValue[,] map, RoomsTreeNode roomsTreeRoot, float gridsOffset)
    {
        this.roomsTreeRoot = roomsTreeRoot;
        this.map = map;
        this.gridsOffset = gridsOffset;

        obs = new Dictionary<string, Vector3>();
        rooms = new List<RoomsTreeNode>();
        GetRooms(roomsTreeRoot);
        rooms.Sort(Compare);

        roomsCounter = rooms.Count / 2;
    }

    public int Compare(RoomsTreeNode x, RoomsTreeNode y)
    {
        if (x.Value.Length == y.Value.Length) return 0;
        else if (x.Value.Length < y.Value.Length) return -1;
        else return 1;
    }

    private void GetRooms(RoomsTreeNode roomsTreeRoot)
    {
        if (roomsTreeRoot.Left == null && roomsTreeRoot.Right == null)
        {
            rooms.Add(roomsTreeRoot);
            return;
        }

        GetRooms(roomsTreeRoot.Right);
        GetRooms(roomsTreeRoot.Left);
    }

    public void SpawnRandomly(Transform transform, IMapObject mapObject)
    {
        if (obs.ContainsKey(transform.name))
            return;
        
        if (mapObject.MapChar == MapChar.Player)
        {
            RoomsTreeNode room = rooms[0];

            IndexOf(map, room.Value[room.Value.GetLength(0) / 2, room.Value.GetLength(1) / 2], out int idx0, out int idx1);

            Spawn(transform, mapObject, idx0, idx1);
        }
        else if (mapObject.MapChar == MapChar.Enemy)
        {
            if (roomsCounter == rooms.Count)
                roomsCounter = rooms.Count / 2;

            RoomsTreeNode room = rooms[roomsCounter];

            IndexOf(map, room.Value[Random.Range(1, room.Value.GetLength(0)), 
                                    Random.Range(1, room.Value.GetLength(1))], out int idx0, out int idx1);

            Spawn(transform, mapObject, idx0, idx1);

            roomsCounter++;
        }
    }

    private void Spawn(Transform transform, IMapObject mapObject, int idx0, int idx1)
    {
        if (obs.ContainsKey(transform.name))
            return;

        map[idx0, idx1].Value = mapObject.MapChar;

        obs.Add(transform.name, new Vector3(idx0, 0, idx1));

        transform.SetParent(this.transform);
        transform.localPosition = new Vector3(idx0 * gridsOffset, transform.localScale.y / 2, idx1 * gridsOffset);
    }

    private void IndexOf(MapCharValue[,] map, MapCharValue item, out int idx0, out int idx1)
    {
        idx0 = idx1 = -1;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == item)
                {
                    idx0 = i;
                    idx1 = j;
                    return;
                }
            }
        }
    }

    private MapChar GetMapChar(int idx0, int idx1)
    {
        return map[idx0, idx1].Value;
    }

    public void Move(Transform transform, Vector3 direction, IMapObject mapObject)
    {
        Vector3 start = obs[transform.name];
        Vector3 end = start + direction;

        int idx0 = (int)end.x;
        int idx1 = (int)end.z;

        if (GetMapChar(idx0, idx1) != MapChar.Empty)
            return;

        obs[transform.name] = end;

        map[(int)start.x, (int)start.z].Value = MapChar.Empty;
        map[idx0, idx1].Value = mapObject.MapChar;

        mapObject.Move(new Vector3(idx0 * gridsOffset, transform.localScale.y / 2, idx1 * gridsOffset));
    }
}
