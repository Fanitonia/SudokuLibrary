﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{

    public partial class Sudoku
    {
        // checks if generated sudoku table can be solved
        private bool Solve()
        {
            Random random = new Random();
            CopyBoard();

            // checks how many cells are there that it's potentialNumber count is 1
            int count = 0;
            foreach (Cell cell in solvedField)
            {
                if (cell.pNumbers.Count == 1 && cell.value == EMPTY_CELL)
                    count++;
            }

            // if there is 0 cells it'll be hard for player so generate new field
            if (count == 0)
                return false;

            step = 0;
            timer.Restart();
            timer.Start();
            do
            {
                foreach (Cell cell in solvedField)
                {
                    if (cell.pNumbers.Count == 0 && cell.value == EMPTY_CELL)
                        return false;

                    step++;
                }

                foreach (Cell cell in solvedField)
                {
                    if (cell.pNumbers.Count == FindSmallestPotential(solvedField) && cell.value == EMPTY_CELL)
                    {
                        cell.value = cell.pNumbers[random.Next(cell.pNumbers.Count)];
                        UpdateAllPotentials(solvedField);
                        break;
                    }

                    step++;
                }
            } while (!IsSudokuValid(solvedField));
            timer.Stop();
            return true;
        }

        // copies field to solvedField to solve sudoku without changing main field
        private void CopyBoard()
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                    solvedField[y, x] = new Cell(field[y, x].value, field[y, x].pNumbers, field[y, x].canChange);
            }
        }

        private bool IsPositionSuitable(int y, int x, int number, Cell[,] board)
        {
            // checks the column and the row of the position
            for (int i = 0; i < 9; i++)
            {
                if (board[y, i].value == number)
                    return false;

                else if (board[i, x].value == number)
                    return false;
            }

            // finding top left position of the 3x3 are position's part of
            int rowStart = y - (y % 3);
            int columnStart = x - (x % 3);

            // checks the 3x3 area position's part of
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[rowStart + i, columnStart + j].value == number)
                        return false;
                }
            }
            return true;
        }

        // updates specific cell's potentials list
        private void UpdateCellPotentials(int y, int x, Cell[,] board)
        {
            List<int> tmpNumbers = new List<int>();
            for (int i = 1; i <= 9; i++)
            {
                if (IsPositionSuitable(y, x, i, board))
                    tmpNumbers.Add(i);
            }
            board[y, x].pNumbers.Clear();
            board[y, x].pNumbers.AddRange(tmpNumbers);
        }

        // updates all cells in field
        private void UpdateAllPotentials(Cell[,] board)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                    UpdateCellPotentials(y, x, board);
            }
        }

        private int FindSmallestPotential(Cell[,] board)
        {
            int smallest = 9;
            foreach (Cell cell in board)
            {
                if (cell.pNumbers.Count < smallest && cell.value == EMPTY_CELL)
                    smallest = cell.pNumbers.Count;
            }

            return smallest;
        }

        private bool IsSudokuValid(Cell[,] board)
        {
            int tmpHolder;
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    tmpHolder = board[y, x].value;
                    board[y, x].value = EMPTY_CELL;
                    if (IsPositionSuitable(y, x, tmpHolder, board) && tmpHolder != EMPTY_CELL)
                        board[y, x].value = tmpHolder;
                    else
                        return false;
                }
            }
            return true;
        }
    }
}