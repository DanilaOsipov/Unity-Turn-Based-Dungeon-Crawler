using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int rowsCount = 40;
    [SerializeField] private int columnsCount = 40;
    [SerializeField] private int minSectorsHeight = 7;
    [SerializeField] private int minSectorsWidth = 7;

    [SerializeField] private GameObject floorGridPrefab;
    [SerializeField] private GameObject wallCubePrefab;

    [SerializeField] private GameObject player;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int minEnemiesCount = 10;
    [SerializeField] private int maxEnemiesCount = 15;

    [SerializeField] private GameObject entranceTriggerPrefab;

    [SerializeField] private GameObject healingPrefab;

    [SerializeField] private GameObject exitCubePrefab;

    [SerializeField] private GameObject ceilingGridPrefab;

    public static int EnemiesCount { get; set; }

    private int roomsCounter;
    private List<RoomsTreeNode> rooms;

    private Dictionary<Transform, Vector3> objectPositionPairs;

    public static float MovementOffset { get; private set; }

    private void Awake()
    {
        GenerateDungeon();

        MovementOffset = floorGridPrefab.transform.localScale.x;
    }

    private void GenerateDungeon()
    {
        rooms = new List<RoomsTreeNode>();

        objectPositionPairs = new Dictionary<Transform, Vector3>();

        MapCharValue[,] map = new MapCharValue[rowsCount, columnsCount];
        
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                map[i, j] = new MapCharValue();

        RoomsTreeNode root = new RoomsTreeNode { Value = map };

        SliceMap(map, root);

        CreateEntrances(map, root);

        CreateFloor(map);

        rooms.Sort(Compare);

        roomsCounter = rooms.Count / 2;

        SpawnPlayer(map);

        CreateExit(map);

        EnemiesCount = Random.Range(minEnemiesCount, maxEnemiesCount);
        SpawnEnemies(map, EnemiesCount);

        new Pathfinding(map, objectPositionPairs);

        Messenger<int>.AddListener(GameEvent.PLAYER_ENTERED_THE_ROOM, RemoveEntranceTriggers);

        SpawnHealings(map);

        CreateWalls(map);

        CreateCeiling(map);
    }

    private void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvent.PLAYER_ENTERED_THE_ROOM, RemoveEntranceTriggers);
    }

    private void CreateCeiling(MapCharValue[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].Value != MapChar.Wall && map[i, j].Value != MapChar.Player)
                {
                    GameObject cube = Instantiate(ceilingGridPrefab);
                    cube.transform.SetParent(transform);
                    cube.transform.localPosition = new Vector3(i * cube.transform.localScale.x,
                                                               wallCubePrefab.transform.localScale.y + cube.transform.localScale.y / 2, 
                                                               j * cube.transform.localScale.x);
                }
            }
        }
    }

    private void CreateExit(MapCharValue[,] map)
    {
        Vector3 vector = objectPositionPairs[player.transform];
        int idx0 = (int)vector.x, idx1 = (int)vector.z;

        int max = minSectorsHeight > minSectorsWidth ? minSectorsWidth : minSectorsHeight;
        int idx = Random.Range(1, max / 2);

        int width = map.GetLength(1) - 1;
        int height = map.GetLength(0) - 1;

        GameObject exit = Instantiate(exitCubePrefab);
        exit.transform.SetParent(transform);

        if (idx0 > map.GetLength(0) / 2)
        {
            if (idx1 > map.GetLength(1) / 2)
            {
                exit.transform.localPosition = new Vector3(idx * exit.transform.localScale.x,
                                                           exit.transform.localScale.y / 2,
                                                           0);

                map[idx, 0].Value = MapChar.Exit;
            }
            else
            {
                map[0, width - idx].Value = MapChar.Exit;

                exit.transform.localPosition = new Vector3(0,
                                                           exit.transform.localScale.y / 2,
                                                           (width - idx) * exit.transform.localScale.x);
            }
        }
        else
        {
            if (idx1 > map.GetLength(1) / 2)
            {
                map[height - idx, 0].Value = MapChar.Exit;

                exit.transform.localPosition = new Vector3((height - idx) * exit.transform.localScale.x,
                                                           exit.transform.localScale.y / 2,
                                                           0);
            }
            else
            {
                map[height, width - idx].Value = MapChar.Exit;

                exit.transform.localPosition = new Vector3(height * exit.transform.localScale.x,
                                                           exit.transform.localScale.y / 2,
                                                           (width - idx) * exit.transform.localScale.x);
            }
        }
    }

    private void SpawnHealings(MapCharValue[,] map)
    {
        for (int i = 1; i < rooms.Count / 2; i++)
        {
            RoomsTreeNode room = rooms[i];

            GameObject healing = Instantiate(healingPrefab);

            IndexOf(map, room.Value[room.Value.GetLength(0) / 2, room.Value.GetLength(1) / 2], out int idx0, out int idx1);

            healing.transform.SetParent(transform);
            healing.transform.localPosition = new Vector3(idx0 * floorGridPrefab.transform.localScale.x, transform.localScale.y / 2,
                                                          idx1 * floorGridPrefab.transform.localScale.z);
        }
    }

    private void SpawnEnemies(MapCharValue[,] map, int enemiesCount)
    {
        if (objectPositionPairs.ContainsKey(transform))
            return;

        for (int i = 0; i < enemiesCount; i++)
        {
            if (roomsCounter == rooms.Count)
                roomsCounter = rooms.Count / 2;

            RoomsTreeNode room = rooms[roomsCounter];

            IndexOf(map, room.Value[Random.Range(1, room.Value.GetLength(0) - 1),
                                    Random.Range(1, room.Value.GetLength(1) - 1)], out int idx0, out int idx1);
            
            GameObject obj = Instantiate(enemyPrefab);

            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.startRoomId = roomsCounter;
                enemy.target = player;
            }

            Spawn(obj.transform, map, MapChar.Enemy, idx0, idx1);

            if (room.EntranceTriggers == null)
            {
                CreateEntranceTriggers(room, map);

                foreach (var et in room.EntranceTriggers)
                {
                    if (et != null) et.roomId = roomsCounter;
                }
            }

            roomsCounter++;
        }
    }

    private void RemoveEntranceTriggers(int roomId)
    {
        List<EntranceTrigger> entranceTriggers = null;

        for (int i = rooms.Count / 2; i < rooms.Count; i++)
        {
            if (rooms[i].EntranceTriggers != null)
            {
                if (rooms[i].EntranceTriggers.Count > 0)
                {
                    if (rooms[i].EntranceTriggers[0] != null && rooms[i].EntranceTriggers[0].roomId == roomId)
                    {
                        entranceTriggers = rooms[i].EntranceTriggers;
                        break;
                    }
                }
            }
        }

        foreach (var et in entranceTriggers)
        {
            Destroy(et.gameObject);
        }

        entranceTriggers = null;
    }

    private void CreateEntranceTriggers(RoomsTreeNode room, MapCharValue[,] map)
    {
        room.EntranceTriggers = new List<EntranceTrigger>();

        for (int i = 1; i < room.Value.GetLength(0) - 1; i++)
        {
            if (room.Value[i, 0].Value == MapChar.Empty)
            {
                IndexOf(map, room.Value[i, 0], out int idx0, out int idx1);
                room.EntranceTriggers.Add(CreateEntranceTrigger(idx0, idx1));
            }
            else if (room.Value[i, room.Value.GetLength(1) - 1].Value == MapChar.Empty)
            {
                IndexOf(map, room.Value[i, room.Value.GetLength(1) - 1], out int idx0, out int idx1);
                room.EntranceTriggers.Add(CreateEntranceTrigger(idx0, idx1));
            }
        }

        for (int j = 1; j < room.Value.GetLength(1) - 1; j++)
        {
            if (room.Value[0, j].Value == MapChar.Empty)
            {
                IndexOf(map, room.Value[0, j], out int idx0, out int idx1);
                room.EntranceTriggers.Add(CreateEntranceTrigger(idx0, idx1));
            }
            else if (room.Value[room.Value.GetLength(0) - 1, j].Value == MapChar.Empty)
            {
                IndexOf(map, room.Value[room.Value.GetLength(0) - 1, j], out int idx0, out int idx1);
                room.EntranceTriggers.Add(CreateEntranceTrigger(idx0, idx1));
            }
        }
    }

    private void SpawnPlayer(MapCharValue[,] map)
    {
        if (objectPositionPairs.ContainsKey(transform))
            return;

        RoomsTreeNode room = rooms[0];

        IndexOf(map, room.Value[room.Value.GetLength(0) / 2, room.Value.GetLength(1) / 2], out int idx0, out int idx1);

        Spawn(player.transform, map, MapChar.Player, idx0, idx1);
    }

    private void Spawn(Transform transform, MapCharValue[,] map, MapChar mapChar, int idx0, int idx1)
    {
        map[idx0, idx1].Value = mapChar;

        objectPositionPairs.Add(transform, new Vector3(idx0, 0, idx1));
        
        float yPos = 0;

        Collider collider = transform.GetComponent<Collider>();
        if (collider != null)
        {
            yPos = collider.bounds.size.y / 2;
        }

        transform.SetParent(this.transform);
        transform.localPosition = new Vector3(idx0 * floorGridPrefab.transform.localScale.x, yPos, 
                                              idx1 * floorGridPrefab.transform.localScale.z);
    }

    public int Compare(RoomsTreeNode x, RoomsTreeNode y)
    {
        if (x.Value.Length == y.Value.Length) return 0;
        else if (x.Value.Length < y.Value.Length) return -1;
        else return 1;
    }

    private void CreateWalls(MapCharValue[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].Value == MapChar.Wall)
                {
                    GameObject cube = Instantiate(wallCubePrefab);
                    cube.transform.SetParent(transform);
                    cube.transform.localPosition = new Vector3(i * cube.transform.localScale.x,
                                                               cube.transform.localScale.y / 2, j * cube.transform.localScale.x);
                }
            }
        }
    }

    private void CreateFloor(MapCharValue[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].Value == MapChar.Empty)
                {
                    GameObject grid = Instantiate(floorGridPrefab);
                    grid.transform.SetParent(transform);
                    grid.transform.localPosition = new Vector3(i * grid.transform.localScale.x,
                                                               -grid.transform.localScale.y / 2, j * grid.transform.localScale.z);
                }
            }
        }
    }

    private void SliceMap(MapCharValue[,] map, RoomsTreeNode root)
    {
        MarkWalls(map);

        MatrixSlice matrixSlice = (MatrixSlice)Random.Range(0, (int)MatrixSlice.Count);

        int position = matrixSlice == MatrixSlice.Vertical ?
                       Random.Range(minSectorsWidth, map.GetLength(1) - minSectorsWidth) :
                       Random.Range(minSectorsHeight, map.GetLength(0) - minSectorsHeight);

        if (!MatrixHelper<MapCharValue>.IsSliceable(map, matrixSlice, position, minSectorsHeight, minSectorsWidth))
        {
            rooms.Add(root);

            return;
        }
            
        MatrixHelper<MapCharValue>.Slice(map, matrixSlice, position, out MapCharValue[,] firstSlice, out MapCharValue[,] secondSlice);

        RoomsTreeNode right = new RoomsTreeNode { Value = firstSlice, Parent = root };
        RoomsTreeNode left = new RoomsTreeNode { Value = secondSlice, Parent = root };
        root.Right = right;
        root.Left = left;

        SliceMap(firstSlice, right);
        SliceMap(secondSlice, left);
    }

    private void CreateEntrances(MapCharValue[,] map, RoomsTreeNode root)
    {
        if (root.Right == null && root.Left == null)
        {
            CreateEntrances(root.Parent.Right, root.Parent.Left, map);
            return;
        }

        CreateEntrances(map, root.Right);
        CreateEntrances(map, root.Left);
    }

    private void CreateEntrances(RoomsTreeNode from, RoomsTreeNode to, MapCharValue[,] map)
    {
        if (from.IsChecked && to.IsChecked)
            return;

        from.IsChecked = to.IsChecked = true;

        IndexOf(map, from.Value[0, 0], out int fromIdx0, out int fromIdx1);
        IndexOf(map, to.Value[0, 0], out int toIdx0, out int toIdx1);

        if (fromIdx0 == toIdx0)
        {
            int position = Random.Range(1, from.Value.GetLength(0) - 1);

            if (fromIdx1 < toIdx1)
            {
                from.Value[position, from.Value.GetLength(1) - 1].Value = default;
                to.Value[position, 0].Value = default;

                from.Value[position, from.Value.GetLength(1) - 2].Value = default;
                to.Value[position, 1].Value = default;
            }
            else
            {
                to.Value[position, from.Value.GetLength(1) - 1].Value = default;
                from.Value[position, 0].Value = default;

                to.Value[position, from.Value.GetLength(1) - 2].Value = default;
                from.Value[position, 1].Value = default;
            }
        }
        else
        {
            int position = Random.Range(1, from.Value.GetLength(1) - 1);

            if (fromIdx0 < toIdx0)
            {
                from.Value[from.Value.GetLength(0) - 1, position].Value = default;
                to.Value[0, position].Value = default;

                from.Value[from.Value.GetLength(0) - 2, position].Value = default;
                to.Value[1, position].Value = default;
            }
            else
            {
                to.Value[from.Value.GetLength(0) - 1, position].Value = default;
                from.Value[0, position].Value = default;

                to.Value[from.Value.GetLength(0) - 2, position].Value = default;
                from.Value[1, position].Value = default;
            }
        }

        if (from.Parent.Parent == null && to.Parent.Parent == null)
            return;

        CreateEntrances(from.Parent.Parent.Right, from.Parent.Parent.Left, map);
    }

    private EntranceTrigger CreateEntranceTrigger(int idx0, int idx1)
    {
        GameObject entrance = Instantiate(entranceTriggerPrefab);
        entrance.transform.SetParent(transform);
        entrance.transform.localPosition = new Vector3(idx0 * entrance.transform.localScale.x,
                                                       entrance.transform.localScale.y / 2, idx1 * entrance.transform.localScale.z);

        return entrance.GetComponent<EntranceTrigger>();
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

    private void MarkWalls(MapCharValue[,] map)
    {
        int rows = map.GetLength(0);
        int clmns = map.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            if (i == 0 || i == rows - 1)
            {
                for (int j = 0; j < clmns; j++)
                {
                    if (map[i, j].Value != MapChar.Wall)
                        map[i, j].Value = MapChar.Wall;
                }
            }
            else
            {
                if (map[i, 0].Value != MapChar.Wall)
                    map[i, 0].Value = MapChar.Wall;

                if (map[i, clmns - 1].Value != MapChar.Wall)
                    map[i, clmns - 1].Value = MapChar.Wall;
            }
        }
    }
}

