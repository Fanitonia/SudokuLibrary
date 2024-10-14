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
        private Cell[,] field = new Cell[9, 9];
        private Cell[,] solvedField = new Cell[9, 9];
        private Stopwatch timer = new Stopwatch();
        private int step = 0;
        private bool endGame = false;
        private readonly ConsoleColor userForeColor = Console.ForegroundColor;
        private readonly ConsoleColor userBackColor = Console.BackgroundColor;
        private const int EMPTY_CELL = 0;

        private class Cell
        {
            public int value = 0;
            public List<int> pNumbers = [1, 2, 3, 4, 5, 6, 7, 8, 9];
            public bool canChange = true;
            public Cell() { }
            public Cell(int value, List<int> pNumbers, bool canChange)
            {
                this.value = value;
                this.pNumbers.Clear();
                this.pNumbers.AddRange(pNumbers);
                this.canChange = canChange;
            }
        }

        private struct Cursor
        {
            public static int x = 0;
            public static int y = 0;
        }

        public void StartGame()
        {
            GenerateField();
            do
            {
                PrintBoard(field);
                HandleInput();
            } while(true);
        }

        private void GenerateField()
        {
            Console.Write("Generating...");
            Random random = new Random();
            int x, y;
            do
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                        field[i, j] = new Cell();
                }

                for (int i = 0; i < 30; i++)
                {
                    do
                    {
                        y = random.Next(9);
                        x = random.Next(9);
                    } while (field[y, x].value != EMPTY_CELL);

                    try
                    {
                        field[y, x].value = field[y, x].pNumbers[random.Next(field[y, x].pNumbers.Count)];
                        field[y, x].canChange = false;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    UpdateAllPotentials(field);
                }
            } while (!Solve());
        }
        // i can't explain this
        private void PrintBoard(Cell[,] board)
        {
            Console.Clear();
            int fieldX = 0, fieldY = 0;

            Console.WriteLine("┌───────┬───────┬───────┐");
            // rows of board
            for (int y = 2; y <= 12; y++)
            {
                // columns of board
                for (int x = 1; x <= 25; x++)
                {
                    if (x % 8 == 1)
                    {
                        if (y == 5 || y == 9 || y == 14)
                            Console.Write("┼");
                        else
                            Console.Write("│");
                        continue;
                    }

                    if (y % 4 == 1)
                    {
                        Console.Write("─");
                        continue;
                    }

                    if(x % 2 == 0)
                    {
                        Console.Write(" ");
                        continue;
                    }

                    if (x % 2 == 1)
                    {
                        if(fieldY == Cursor.y && fieldX == Cursor.x && endGame != true)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.Write(" ");
                            Console.BackgroundColor = userBackColor;
                        }
                        else if (board[fieldY, fieldX].value != EMPTY_CELL)
                        {
                            if (board[fieldY, fieldX].canChange)
                                Console.ForegroundColor = ConsoleColor.Red;

                            Console.Write(board[fieldY, fieldX].value);
                            Console.ForegroundColor = userForeColor;
                        }
                            
                        else
                            Console.Write(" ");

                        fieldX++;
                    }
                }
                if (y % 4 != 1)
                    fieldY++;

                fieldX = 0;
                Console.WriteLine();
            }
            Console.WriteLine("└───────┴───────┴───────┘");

            Console.WriteLine();
            if(!endGame)
                Console.WriteLine("Move -> UP, DOWN, RIGHT, LEFT arrow buttons\nPick Number -> 1-9\nDelete Number -> Del\nAuto Solve -> S\nClose -> Q");
        }

        private void HandleInput()
        {
            ConsoleKeyInfo input;
            input = Console.ReadKey(true);

            if(char.IsNumber(input.KeyChar) && field[Cursor.y, Cursor.x].value == EMPTY_CELL)
            {
                field[Cursor.y, Cursor.x].value = (int) char.GetNumericValue(input.KeyChar);
                return;
            }
            switch (input.Key)
            {
                case ConsoleKey.UpArrow:
                    if(Cursor.y > 0)
                        Cursor.y--;
                    break;
                case ConsoleKey.DownArrow:
                    if(Cursor.y < 8)
                        Cursor.y++;
                    break;
                case ConsoleKey.LeftArrow:
                    if(Cursor.x > 0)
                        Cursor.x--;
                    break;
                case ConsoleKey.RightArrow:
                    if(Cursor.x < 8)
                        Cursor.x++;
                    break;
                case ConsoleKey.Delete:
                    if(field[Cursor.y, Cursor.x].canChange)
                        field[Cursor.y, Cursor.x].value = 0;
                    break;
                case ConsoleKey.Q:
                    Environment.Exit(0);
                    break;
                case ConsoleKey.S:
                    EndGame();
                    break;
                default:
                    break;
            }

            if (IsSudokuValid(field))
                WinGame();
            else if (endGame)
                EndGame();
        }

        private void EndGame()
        {
            endGame = true;
            PrintBoard(solvedField);
            Console.WriteLine($"Sudoku solved in {timer.ElapsedMilliseconds} ms\nTotal Steps: {step}");
            QuitOrRestart();
        }

        private void WinGame()
        {
            endGame = true;
            PrintBoard(field);
            Console.WriteLine("\n     YOU DID WIN!!!");
            QuitOrRestart();
        }

        private void QuitOrRestart()
        {
            endGame = false;
            Console.WriteLine("\nPress Enter to play again or Q to quit.");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        StartGame(); 
                        break;
                    case ConsoleKey.Q:
                        Environment.Exit(0);
                        break;
                }
            }

        }
    }
}