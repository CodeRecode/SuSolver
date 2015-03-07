using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBox[,] gridFields;
        Stopwatch watch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            InitializeGrid();
        }

        /// <summary>
        /// Initialize the Sudoku grid
        /// </summary>
        private void InitializeGrid()
        {
            gridFields = new TextBox[9, 9];

            for (int i = 0; i < 9; i++)
            {
                SudokuGrid.RowDefinitions.Add(new RowDefinition());
                SudokuGrid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < 9; j++)
                {
                    TextBox textBox = new TextBox();
                    textBox.Height = 50;
                    textBox.Width = 50;
                    Grid.SetRow(textBox, i);
                    Grid.SetColumn(textBox, j);

                    double borderBottom = 0.99, borderRight = 0.99, borderDefault = 0.99;
                    if (i == 2 || i == 5)
                        borderBottom += 2.0;
                    if (j == 2 || j == 5)
                        borderRight += 2.0;

                    textBox.BorderBrush = Brushes.Black;
                    textBox.BorderThickness = new Thickness(borderDefault, borderDefault, borderRight, borderBottom);
                    textBox.Padding = new Thickness(borderDefault, borderDefault, borderRight, borderBottom);

                    textBox.FontSize = 30;
                    textBox.FontStyle = FontStyles.Italic;
                    textBox.Foreground = Brushes.CornflowerBlue;
                    textBox.TextAlignment = TextAlignment.Center;
                    textBox.MaxLength = 1;

                    if ((i % 2 == 0 && j % 2 == 1) || (i % 2 == 1 && j % 2 == 0))
                        textBox.Background = Brushes.LightGray;

                    SudokuGrid.Children.Add(textBox);
                    gridFields[i, j] = textBox;
                }
            }
        }

        /// <summary>
        /// Solve the Sudoku and write the solution to the grid
        /// </summary>
        private void SolveClick(object sender, RoutedEventArgs e)
        {
            int[,] grid = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    char letter = gridFields[i, j].Text.Length == 0 ? '0' : gridFields[i, j].Text[0];

                    if (letter >= '0' && letter <= '9')
                        grid[i, j] = letter - '0';
                    else
                        grid[i, j] = 0;
                }
            }

            watch.Reset();
            watch.Start();

            bool solved = Solver.Solve(ref grid);

            watch.Stop();

            if(solved)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        gridFields[i, j].Text = grid[i, j].ToString();
                    }
                }
                StatusText.Text = "Solution was found in " + watch.ElapsedMilliseconds / 1000.0 + " seconds";
                StatusText.Foreground = Brushes.Black;
            }
            else
            {
                StatusText.Text = "No solution was found.";
                StatusText.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Launch the open file dialog then load the file into the grid
        /// </summary>
        private void LoadFromFileClick(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "";

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text Files (*.txt)|*.txt";
            dialog.InitialDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            bool result = dialog.ShowDialog() ?? false;

            if (result == true)
            {
                string[] inputLines = File.ReadAllLines(dialog.FileName);
                for (int i = 0; i < 9; i++)
                {
                    string input = inputLines[i];
                    for (int j = 0; j < 9; j++)
                    {
                        TextBox textBox = gridFields[i, j];
                        if (input[j] > '0' && input[j] <= '9')
                        {
                            textBox.Text = input[j].ToString();
                            textBox.FontStyle = FontStyles.Normal;
                            textBox.FontWeight = FontWeights.Heavy;
                            textBox.Foreground = Brushes.Black;
                            textBox.IsReadOnly = true;
                        }
                        else
                        {
                            textBox.Text = "";
                            textBox.FontStyle = FontStyles.Italic;
                            textBox.FontWeight = FontWeights.Normal;
                            textBox.Foreground = Brushes.CornflowerBlue;
                            textBox.IsReadOnly = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Launch the save file dialog and write out the current grid
        /// </summary>
        private void SaveToFileClick(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "";

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text Files (*.txt)|*.txt";
            dialog.InitialDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            bool result = dialog.ShowDialog() ?? false;

            if (result == true)
            {
                string[] outputLines = new string[9];
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (gridFields[i, j].Text != string.Empty && gridFields[i, j].Text[0] > '0' && gridFields[i, j].Text[0] <= '9')
                        {
                            outputLines[i] += gridFields[i, j].Text;
                        }
                        else
                        {
                            outputLines[i] += "0";
                        }
                    }
                }
                File.WriteAllLines(dialog.FileName, outputLines);
            }
        }

        /// <summary>
        /// Reset the grid to the default state
        /// </summary>
        private void ResetGridClick(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "";

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox textBox = gridFields[i, j];
                    textBox.Text = "";
                    textBox.FontStyle = FontStyles.Italic;
                    textBox.FontWeight = FontWeights.Normal;
                    textBox.Foreground = Brushes.CornflowerBlue;
                    textBox.IsReadOnly = false;
                }
            }
        }
    }
}
