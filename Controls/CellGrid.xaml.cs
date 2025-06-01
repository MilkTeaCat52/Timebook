using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Timebook.Helper;
using Windows.Foundation;
using Windows.Foundation.Collections;

using CellID = System.Guid;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public class TableData
    {
        public List<List<CellID>> CellMatrix { get; set; } = new();
        public int Rows { get; set; } = 5;
        public int Columns { get; set; } = 11;

        public TableData() { }

        public TableData(TableData data)
        {
            this.CellMatrix = data.CellMatrix;
            this.Rows = data.Rows;
            this.Columns = data.Columns;
        }
    }

    public sealed partial class CellGrid : UserControl
    {
        TableData tableData;

        public CellGrid()
        {
            this.tableData = DataHelper.CreateTable();

            this.InitializeComponent();

            InitializeGrid();
            PopulateTableData();
            PopulateGrid();
        }

        public CellGrid(TableData tableData)
        {
            this.tableData = tableData;

            this.InitializeComponent();

            InitializeGrid();
            PopulateGrid();
        }

        void InitializeGrid()
        {

            RootGrid.RowDefinitions.Clear();
            RootGrid.ColumnDefinitions.Clear();

            RootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
            RootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            for (int i = 0; i < tableData.Rows; i++)
            {
                RootGrid.RowDefinitions.Add(new RowDefinition {Height= new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < tableData.Columns; i++)
            {
                RootGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star) });
            }
        }

        void PopulateTableData()
        {
            int rows = tableData.Rows;
            int columns = tableData.Columns;

            for (int i = 0; i < rows; i++)
            {
                tableData.CellMatrix.Add(new List<CellID>());
                for (int j = 0; j < columns; j++)
                {
                    tableData.CellMatrix[i].Add(CellID.Empty);
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j=0; j < columns; j++)
                {
                    var cellID = DataHelper.CreateCellData();
                    tableData.CellMatrix[i][j] = cellID;
                }
            }
        }

        void PopulateGrid()
        {
            int rows = tableData.Rows;
            int columns = tableData.Columns;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var cellID = tableData.CellMatrix[i][j];
                    var cell = new CellButton(cellID);

                    cell.Margin = new Thickness(2, 2, 2, 2);

                    Grid.SetRow(cell, i + 1);
                    Grid.SetColumn(cell, j + 1);
                    RootGrid.Children.Add(cell);
                }
            }
        }
    }
}
