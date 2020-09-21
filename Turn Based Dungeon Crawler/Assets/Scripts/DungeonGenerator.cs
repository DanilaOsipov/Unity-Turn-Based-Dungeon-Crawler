using UnityEngine;

[RequireComponent(typeof(DungeonMap))]
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int rowsCount;
    [SerializeField] private int columnsCount;
    [SerializeField] private int minSectorsHeight;
    [SerializeField] private int minSectorsWidth;

    [SerializeField] private GameObject floorGridPrefab;
    [SerializeField] private GameObject wallCubePrefab;

    private DungeonMap dungeonMap;

    private void Awake()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        MapCharValue[,] map = new MapCharValue[rowsCount, columnsCount];
        
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                map[i, j] = new MapCharValue();

        RoomsTreeNode root = new RoomsTreeNode { Value = map };

        SliceMap(map, root);

        CreateEntrance(map, root);

        CreateWalls(map);

        CreateFloor(map);

        dungeonMap = GetComponent<DungeonMap>();
        dungeonMap.Initialize(map, root, floorGridPrefab.transform.localScale.x);
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
                if (map[i, j].Value != MapChar.Wall)
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
            return;
            
        MatrixHelper<MapCharValue>.Slice(map, matrixSlice, position, out MapCharValue[,] firstSlice, out MapCharValue[,] secondSlice);

        RoomsTreeNode right = new RoomsTreeNode { Value = firstSlice, Parent = root };
        RoomsTreeNode left = new RoomsTreeNode { Value = secondSlice, Parent = root };
        root.Right = right;
        root.Left = left;

        SliceMap(firstSlice, right);
        SliceMap(secondSlice, left);
    }

    private void CreateEntrance(MapCharValue[,] map, RoomsTreeNode root)
    {
        if (root.Right == null && root.Left == null)
        {
            CreateEntrance(root.Parent.Right, root.Parent.Left, map);
            return;
        }
        else 

        CreateEntrance(map, root.Right);
        CreateEntrance(map, root.Left);
    }

    private void CreateEntrance(RoomsTreeNode from, RoomsTreeNode to, MapCharValue[,] map)
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

        CreateEntrance(from.Parent.Parent.Right, from.Parent.Parent.Left, map);
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

