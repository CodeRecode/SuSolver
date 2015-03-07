using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class Solver
    {
        public static bool Solve(ref int[,] grid)
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (grid[i, j] == 0)
                    {
                        List<bool> nums = CheckNeighbors(grid, i, j);
                        for (int k = 1; k < 10; k++)
                        {
                            if (nums[k])
                            {
                                grid[i, j] = k;
                                if (Solve(ref grid))
                                {
                                    return true;
                                }
                            }
                        }
                        grid[i, j] = 0;
                        return false;
                    }
                }
            }
            return true;
        }

        public static List<bool> CheckNeighbors(int[,] grid, int x, int y)
        {
            List<bool> nums = new List<bool>() { false, true, true, true, true, true, true, true, true, true };
            int squareX = (x / 3) * 3,
                squareY = (y / 3) * 3;

            for (int i = 0; i < 9; ++i)
            {
                nums[grid[x, i]] = false;
                nums[grid[i, y]] = false;
                nums[grid[squareX + (i / 3), squareY + (i % 3)]] = false;
            }

            return nums;
        }
    }
}
