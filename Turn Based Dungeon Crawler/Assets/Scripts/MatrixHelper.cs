using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MatrixHelper<T>
{
    public static void Slice(T[,] matrix, MatrixSlice matrixSlice, int position, out T[,] firstSlice, out T[,] secondSlice)
    {
        int rowsCount = matrix.GetLength(0);
        int columnsCount = matrix.GetLength(1);

        if (matrixSlice == MatrixSlice.Vertical)
        {
            firstSlice = new T[rowsCount, position];
            secondSlice = new T[rowsCount, columnsCount - position];
        }
        else
        {
            firstSlice = new T[position, columnsCount];
            secondSlice = new T[rowsCount - position, columnsCount];
        }

        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                if (matrixSlice == MatrixSlice.Vertical)
                {
                    if (j < position) firstSlice[i, j] = matrix[i, j];
                    else secondSlice[i, j - firstSlice.GetLength(1)] = matrix[i, j];
                }
                else
                {
                    if (i < position) firstSlice[i, j] = matrix[i, j];
                    else secondSlice[i - firstSlice.GetLength(0), j] = matrix[i, j];
                }
            }
        }
    }

    public static T[,] Cut(T[,] matrix, int positionX, int positionY, int sectorHeight, int sectorWidth)
    {
        T[,] res = new T[sectorHeight, sectorWidth];

        for (int i = 0; i < sectorHeight; i++)
        {
            for (int j = 0; j < sectorWidth; j++)
            {
                res[i, j] = matrix[i + positionY, j + positionX];
            }
        }

        return res;
    }

    public static bool IsSliceable(T[,] matrix, MatrixSlice matrixSlice, int position, int minSectorsHeight, int minSectorsWidth)
    {
        if (matrixSlice == MatrixSlice.Vertical)
        {
            return position >= minSectorsWidth &&
                    matrix.GetLength(1) - position >= minSectorsWidth &&
                    matrix.GetLength(0) >= minSectorsHeight;
        }
        else
        {
            return position >= minSectorsHeight &&
                    matrix.GetLength(0) - position >= minSectorsHeight &&
                    matrix.GetLength(1) >= minSectorsWidth;
        }
    }

    public static bool IsCutable(T[,] matrix, int positionX, int positionY, int sectorHeight, int sectorWidth)
    {
        return matrix.GetLength(0) - positionY >= sectorHeight &&
               matrix.GetLength(1) - positionX >= sectorWidth;
    }
}

