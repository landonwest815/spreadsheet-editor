using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Shapes;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        private const string allTopLabels = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string initialTopLabels = "ABCDEFGHIJKL";
        private int numOfTopLabels;
        private int numOfLeftLabels = 25;
        private string currentlySelectedCell = "";

        /// <summary>
        ///   Definition of the method signature that must be true for clear methods
        /// </summary>
        private delegate void Clear();

        /// <summary>
        ///   Notifier Pattern.
        ///   When ClearAll(); is called, every "attached" method is called.
        /// </summary>
        private event Clear ClearAll;

        private Dictionary<string, List<MyEntry>> Entries = new Dictionary<string, List<MyEntry>>();

        private List<VerticalStackLayout> Columns = new List<VerticalStackLayout>();

        /// <summary>
        ///    Definition of what information (method signature) must be sent
        ///    by the Entry when it is modified.
        /// </summary>
        /// <param name="col"> col (char) in grid, e.g., A5 </param>
        /// <param name="row"> row (int) in grid,  e.g., A5 </param>
        public delegate void ActionOnCompleted(char col, int row);

        public delegate void ActionOnFocused(char col, int row);

        public class MyEntry : Entry
        {
            // ROW & COLUMN VARIABLES
            int row = 0;
            char column;

            /// <summary>
            ///   Function provided by "outside world" to be called whenever
            ///   this entry is modified
            /// </summary>
            private ActionOnCompleted onChange;

            /// <summary>
            ///   Function provided by "outside world" to be called whenever
            ///   this entry is focused
            /// </summary>
            private ActionOnFocused onFocus;

            /// <summary>
            ///   build an Entry element with the row "remembered"
            /// </summary>
            /// <param name="row"> unique identifier for this item </param>
            /// <param name="changeAction"> outside action that should be invoked after this cell is modified </param>
            public MyEntry(int row, ActionOnCompleted changeAction, ActionOnFocused focusAction) : base()
            {
                // SET THE ROW
                this.row = row;
                // SIZING
                this.HeightRequest = 40;
                this.WidthRequest = 75;
                // TEXT
                this.FontAttributes = FontAttributes.Bold;
                this.FontSize = 15;
                // ALIGNMENT
                this.HorizontalTextAlignment = TextAlignment.End;
                this.VerticalTextAlignment = TextAlignment.Center;

                // Action to take when the user presses enter on this cell
                this.Completed += CellChangedValue;
                // Action to take when the use clicks on this cell
                this.Focused += IsFocus;

                // "remember" outside worlds request about what to do when we change.
                onChange = changeAction;
                // "remember" outside worlds request about what to do when we focus.
                onFocus = focusAction;
            }

            /// <summary>
            ///   Remove focus and text from this widget
            /// </summary>
            public void ClearAndUnfocus()
            {
                this.Unfocus();
                this.Text = "";
            }

            public void SetColumn(char cellColumn)
            {
                this.column = cellColumn;
            }

            /// <summary>
            ///   Action to take when the value of this entry widget is changed
            ///   and the Enter Key pressed.
            /// </summary>
            /// <param name="sender"> ignored, but should == this </param>
            /// <param name="e"> ignored </param>
            private void CellChangedValue(object sender, EventArgs e)
            {
                Unfocus();

                // Inform the outside world that we have changed
                onChange(column, row);
            }

            private void IsFocus(object sender, EventArgs e)
            {
                // Inform the outside world that we have focused
                onFocus(column, row + 1);
            }
        }

        public class ColumnLabel : Border
        {
            public ColumnLabel(char label) : base()
            {
                // ORANGE BORDER
                this.Stroke = Color.FromRgb(212, 212, 210);
                this.StrokeThickness = 0;
                // SIZING
                this.HeightRequest = 40;
                this.WidthRequest = 75;
                // SPACING
                this.HorizontalOptions = LayoutOptions.Center;
                // ROUND CORNERS
                this.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5, 5, 5, 5) };
                    // LABEL
                    Content =
                        new Label
                        {
                            // TEXT
                            Text = $"{label}",
                            TextColor = Color.FromRgb(28, 28, 28),
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 15,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            // ORANGE BACKGROUND
                            BackgroundColor = Color.FromRgb(212, 212, 210)
                        };
            }
        }

        public class RowLabel : Border
        {
            public RowLabel(int row) : base() 
            {
                // ORANGE BORDER
                this.Stroke = Color.FromRgb(255, 149, 0);
                this.StrokeThickness = 0;
                // SIZING
                this.HeightRequest = 40;
                this.WidthRequest = 75;
                // SPACING
                this.HorizontalOptions = LayoutOptions.Center;
                // ROUND CORNERS
                this.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5, 5, 5, 5) };
                // LABEL
                Content =
                    new Label
                    {
                        // TEXT
                        Text = $"{row + 1}",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 15,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        // GRAY BACKGROUND
                        BackgroundColor = Color.FromRgb(255, 149, 0)
                    };
               
            }
        }

        public MainPage()
        {
            InitializeComponent();

            // GET THE NUMBER OF TOP LABELS
            numOfTopLabels = initialTopLabels.Length;

            AddInitialEntriesToGrid(numOfTopLabels, numOfLeftLabels);

            AddInitialTopLabels(numOfTopLabels);

            CreateNewColumnButton();

            AddInitialLeftLabels(numOfLeftLabels);

            CreateNewRowButton();
        }

        private void FileMenuNew(object sender, EventArgs e)
        {

        }

        private void FileMenuOpenAsync(object sender, EventArgs e)
        {

        }

        private void AddColumnClicked(object sender, EventArgs e)
        {
            // ADD A LIST TO THE ENTRIES DICTIONARY FOR THE NEW COLUMN
            Entries.Add(allTopLabels[numOfTopLabels].ToString(), new List<MyEntry>());

            // ADD A NEW COLUMN TO THE COLUMNS LIST
            VerticalStackLayout column = new VerticalStackLayout();
            Columns.Add(column);

            // ADD THE COLUMN TO THE GRID
            Grid.Add(column);

            // LOOP THROUGH THE ROWS AND ADD ENTRIES
            for (int j = 0; j < numOfLeftLabels; j++)
            {
                // ADD ENTRY
                Entries[allTopLabels[numOfTopLabels].ToString()].Add(new MyEntry(j, handleCellChanged, cellClickedOn));
                // ADD ENTRY TO COLUMN
                column.Add(Entries[allTopLabels[numOfTopLabels].ToString()][j]);
                // SET THE COLUMN VARIABLE
                Entries[allTopLabels[numOfTopLabels].ToString()][j].SetColumn(allTopLabels[numOfTopLabels]);
                // ADD THE ENTRY TO THE CLEAR ALL METHOD
                ClearAll += Entries[allTopLabels[numOfTopLabels].ToString()][j].ClearAndUnfocus;
            }

            //$"{allTopLabels[numOfTopLabels]}"
            // INSERT THE NEXT LABEL BEFORE THE ADD COLUMN BUTTON
            TopLabels.Insert(TopLabels.Count - 1, new ColumnLabel(allTopLabels[numOfTopLabels]));

            // INCREMENT THE NUMBER OF TOP LABELS
            numOfTopLabels++;
        }

        private void AddRowClicked(object sender, EventArgs e)
        {
            // LOOP THROUGH THE COLUMNS AND ADD AN ADDITIONAL ENTRY AT THE BOTTOM OF EACH
            for (int i = 0; i < numOfTopLabels; i++)
            {
                // ADD ENTRY
                Entries[allTopLabels[i].ToString()].Add(new MyEntry(numOfLeftLabels, handleCellChanged, cellClickedOn));
                // ADD ENTRY TO ROW
                Columns[i].Add(Entries[allTopLabels[i].ToString()][numOfLeftLabels]);
                // SET NEW ENTRY COLUMN VARIABLE
                Entries[allTopLabels[i].ToString()][numOfLeftLabels].SetColumn(allTopLabels[i]);
                // ADD NEW ENTRY TO CLEAR ALL METHOD
                ClearAll += Entries[allTopLabels[i].ToString()][numOfLeftLabels].ClearAndUnfocus;
            }

            // INSERT THE NEXT LABEL BEFORE THE ADD ROW BUTTON
            LeftLabels.Insert(LeftLabels.Count - 1, new RowLabel(numOfLeftLabels++));
        }

        /// <summary>
        ///   This method will be called by the individual Entry elements when Enter
        ///   is pressed in them.
        ///   
        ///   The idea is to move to the next cell in the list.
        /// </summary>
        /// <param name="col"> e.g., The 'A' in A5 </param>
        /// <param name="row"> e.g., The  5  in A5 </param>
        private void handleCellChanged(char col, int row)
        {
            if (row == numOfLeftLabels - 1)
            {
                Entries[col.ToString()][0].Focus();
            }
            else
            {
                Entries[col.ToString()][row + 1].Focus();
            }
        }

        private void cellClickedOn(char col, int row)
        {
            selectedCellValue.Text = col.ToString();
            selectedCellContents.Text = row.ToString();
            currentlySelectedCell = "" + col.ToString().ToUpper() + row;
            selectedCellName.Text = currentlySelectedCell;
        }

        /// <summary>
        ///   Shows how the single "event" method ClearAll can apply to many listeners.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void ClearButtonClicked(object sender, EventArgs e)
        {
            ClearAll();
            Entries[initialTopLabels[0].ToString()][0].Focus();
        }

        private void AddInitialEntriesToGrid(int numOfColumns, int numOfRows)
        {
            // LOOP THROUGH THE COLUMNS & ADD ENTRIES
            for (int i = 0; i < numOfColumns; i++)
            {
                // ADD A NEW LIST TO THE ENTRIES DICTIONARY
                Entries.Add(initialTopLabels[i].ToString(), new List<MyEntry>());
                // ADD A NEW COLUMN TO THE COLUMNS LIST
                VerticalStackLayout column = new VerticalStackLayout();
                Columns.Add(column);
                // ADD THE NEW COLUMN TO THE SPREADSHEET LAYOUT
                Grid.Add(column);

                // LOOP THROUGH THE ROWS & ADD ENTRIES
                for (int j = 0; j < numOfRows; j++)
                {
                    // ADD THE ENTRIES TO THE RELATIVE LIST IN THE ENTRIES DICTIONARY
                    Entries[initialTopLabels[i].ToString()].Add(new MyEntry(j, handleCellChanged, cellClickedOn));
                    // ADD THE CURRENT ENTRY TO THE SPREADSHEET
                    column.Add(Entries[initialTopLabels[i].ToString()][j]);
                    // SET THE COLUMN VARIABLE OF THE CURRENT ENTRY
                    Entries[initialTopLabels[i].ToString()][j].SetColumn(initialTopLabels[i]);
                    // ADD THE CURRENT ENTRY TO THE CLEAR ALL METHOD
                    ClearAll += Entries[initialTopLabels[i].ToString()][j].ClearAndUnfocus;
                }
            }
        }

        private void AddInitialTopLabels(int numOfColumns)
        {
            // ADD ALL TOP LABELS (A B C D E...)
            for (int i = 0; i < numOfColumns; i++)
            {
                TopLabels.Add(new ColumnLabel(initialTopLabels[i]));
            }
        }

        private void CreateNewColumnButton()
        {
            // CREATE BUTTON
            Button addColumn = new Button()
            {
                // SIZING
                HeightRequest = 30,
                WidthRequest = 75,
                BorderWidth = 0,
                // TEXT
                Text = "+",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 25,
                FontAttributes = FontAttributes.Bold,
                // BLEND IN BACKGROUND
                BackgroundColor = Color.FromRgb(28, 28, 28),
            };

            // ADD FUNCTION TO THE CLICKED EVENT
            addColumn.Clicked += AddColumnClicked;

            // ADD BUTTON TO THE SPREADSHEET
            TopLabels.Add(addColumn);
        }

        private void AddInitialLeftLabels(int numOfRows)
        {
            // ADD ALL LEFT LABELS (1 2 3 4 5...)
            for (int i = 0; i < numOfLeftLabels; i++)
            {
                LeftLabels.Add(new RowLabel(i));
            }
        }

        private void CreateNewRowButton()
        {
            // CREATE BUTTON
            Button addRow = new Button()
            {
                // SIZING
                HeightRequest = 30,
                WidthRequest = 75,
                BorderWidth = 0,
                // TEXT
                Text = "+",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 25,
                FontAttributes = FontAttributes.Bold,
                // BACKGROUND SAME COLOR AS WINDOW BACKGROUND
                BackgroundColor = Color.FromRgb(28, 28, 28),
            };

            // ADD FUNCTION FOR THE CLICKED EVENT
            addRow.Clicked += AddRowClicked;

            // ADD BUTTON TO THE SPREADSHEET
            LeftLabels.Add(addRow);
        }
    } 
}