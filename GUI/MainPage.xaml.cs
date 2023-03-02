using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Shapes;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SS;
using System.Runtime.Serialization;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace GUI
{
    /// <summary>
    /// Author:    Landon West
    /// Partner:   None
    /// Date:      24-Feb-2023
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Landon West - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Landon West, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private const string allTopLabels = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string initialTopLabels = "ABCDEFGHIJKL";
        private int numOfTopLabels;
        private int numOfLeftLabels = 25;
        private AbstractSpreadsheet spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "six");

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

        private Button addRow;

        private string GUIColorTheme = "#d1603d";

        /// <summary>
        ///    Definition of what information (method signature) must be sent
        ///    by the Entry when it is modified.
        /// </summary>
        /// <param name="col"> col (char) in grid, e.g., A5 </param>
        /// <param name="row"> row (int) in grid,  e.g., A5 </param>
        public delegate void ActionOnCompleted(char col, int row);

        public delegate void ActionOnFocused(char col, int row);

        public delegate void ActionOnUnfocused(char col, int row);

        public delegate void OnDisplayWarning();

        public class MyEntry : Entry
        {
            // ROW & COLUMN VARIABLES
            int row = 0;
            char column;
            bool containsFormula = false;

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

            private ActionOnUnfocused onUnfocus;

            /// <summary>
            ///   build an Entry element with the row "remembered"
            /// </summary>
            /// <param name="row"> unique identifier for this item </param>
            /// <param name="changeAction"> outside action that should be invoked after this cell is modified </param>
            public MyEntry(int row, ActionOnCompleted changeAction, ActionOnFocused focusAction, ActionOnUnfocused unfocusAction) : base()
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

                this.Unfocused += CellChangedFocus;

                // "remember" outside worlds request about what to do when we change.
                onChange = changeAction;
                // "remember" outside worlds request about what to do when we focus.
                onFocus = focusAction;

                onUnfocus = unfocusAction;
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

            public void SetContainsFormula(bool containsFormula)
            {
                if (containsFormula)
                    this.containsFormula = true;
                else
                    this.containsFormula = false;
            }

            public bool GetContainsFormula()
            {
                return this.containsFormula;
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

            private void CellChangedFocus(object sender, EventArgs e)
            {
                onUnfocus(column, row);
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
                this.Stroke = Color.FromArgb("#d1603d");
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
                        BackgroundColor = Color.FromArgb("#d1603d")

            };
               
            }
        }

        public MainPage()
        {
            InitializeComponent();

            ClearAll += CreateNewSpreadsheet;

            // GET THE NUMBER OF TOP LABELS
            numOfTopLabels = initialTopLabels.Length;

            AddInitialEntriesToGrid(numOfTopLabels, numOfLeftLabels);

            AddInitialTopLabels(numOfTopLabels);

            CreateNewColumnButton();

            AddInitialLeftLabels(numOfLeftLabels);

            CreateNewRowButton();
        }

        private void CreateNewSpreadsheet()
        {
            spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "six");
        }

        private void FileMenuNew(object sender, EventArgs e)
        {
            if (spreadsheet.Changed == true)
                DataLoss(newFile);
            else
                newFile();
        }

        private async void newFile()
        {
            Entries["A"][0].Focus();
            ClearAll();
            CreateNewSpreadsheet();
            await DisplayAlert("", "A new spreadsheet has been created.", "Ok");
        }

        private async void FileMenuSave(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fileName = await DisplayPromptAsync("Save Spreadsheet", "Name of file:", "Save");

            if (fileName != null)
            {
                try
                {
                    spreadsheet.Save(path + "\\" + fileName + ".sprd");
                    await DisplayAlert("", "Successfully saved '" + fileName + "' to the Desktop", "Ok");
                }
                catch (Exception)
                {
                    await DisplayAlert("", "Failed to save '" + fileName + "' to the Desktop", "Ok");
                }
            }
        }

        private void FileMenuOpenAsync(object sender, EventArgs e)
        {
            if (spreadsheet.Changed == true)
                DataLoss(openFile);
            else
                openFile();
        }

        private async void openFile()
        {
            FileResult? fileResult = await FilePicker.Default.PickAsync();
            if (fileResult != null)
            {
                ClearAll();
                OpenFilePath(fileResult);
            }
        }

        private async void OpenFilePath(FileResult filePath)
        {
            try
            {
                spreadsheet = new Spreadsheet(filePath.FullPath, s => true, s => s.ToUpper(), "six");
                foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    try
                    {
                        Formula formula = (Formula)spreadsheet.GetCellContents(cell);
                        Entries[cell[0].ToString()][int.Parse(cell[1].ToString()) - 1].SetContainsFormula(true);
                        Entries[cell[0].ToString()][int.Parse(cell[1].ToString()) - 1].Text = spreadsheet.GetCellValue(cell).ToString();
                    }
                    catch (InvalidCastException)
                    {
                        Entries[cell[0].ToString()][int.Parse(cell[1].ToString()) - 1].Text = spreadsheet.GetCellValue(cell).ToString();
                    }
                }
                await DisplayAlert("", "Successfully opened '" + filePath.FileName + "'", "Ok");
            }
            catch (SpreadsheetReadWriteException)
            {
                await DisplayAlert("", "Invalid Filepath", "Ok");
            }
            catch (FileNotFoundException)
            {
                await DisplayAlert("", "File does not exist", "Ok");
            }
        }

        private async void DataLoss(OnDisplayWarning afterWarning)
        {
            OnDisplayWarning action = afterWarning;

            bool overwrite = await DisplayAlert("Warning", "There are unsaved Changes. \n Do you want to contiue?", "Yes", "No");

            if (overwrite)
                afterWarning();
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
                Entries[allTopLabels[numOfTopLabels].ToString()].Add(new MyEntry(j, handleCellChanged, cellClickedOn, cellUnfocused));
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
                Entries[allTopLabels[i].ToString()].Add(new MyEntry(numOfLeftLabels, handleCellChanged, cellClickedOn, cellUnfocused));
                // ADD ENTRY TO ROW
                Columns[i].Add(Entries[allTopLabels[i].ToString()][numOfLeftLabels]);
                // SET NEW ENTRY COLUMN VARIABLE
                Entries[allTopLabels[i].ToString()][numOfLeftLabels].SetColumn(allTopLabels[i]);
                // ADD NEW ENTRY TO CLEAR ALL METHOD
                ClearAll += Entries[allTopLabels[i].ToString()][numOfLeftLabels].ClearAndUnfocus;
            }

            // INSERT THE NEXT LABEL BEFORE THE ADD ROW BUTTON
            RowLabel newLabel = new RowLabel(numOfLeftLabels++);
            newLabel.Content.BackgroundColor = Color.FromRgba(GUIColorTheme);
            LeftLabels.Add(newLabel);
        }

        /// <summary>
        ///   This method will be called by the individual Entry elements when Enter
        ///   is pressed in them.
        ///   
        ///   The idea is to move to the next cell in the list.
        /// </summary>
        /// <param name="col"> e.g., The 'A' in A5 </param>
        /// <param name="row"> e.g., The  5  in A5 </param>
        private async void handleCellChanged(char col, int row)
        {
            IList<string> toRecalculate = new List<string>();

            // SET CONTENTS OF EDITED CELL
            if (Entries[col.ToString()][row].Text != null)
                toRecalculate = spreadsheet.SetContentsOfCell("" + col.ToString().ToUpper() + (row + 1), Entries[col.ToString()][row].Text);

            if (Entries[col.ToString()][row].Text != "" && Entries[col.ToString()][row].Text != null)
            {
                if (Entries[col.ToString()][row].Text[0] == '=')
                    Entries[col.ToString()][row].SetContainsFormula(true);
                else
                    Entries[col.ToString()][row].SetContainsFormula(false);
            } 
            else
            {
                Entries[col.ToString()][row].SetContainsFormula(false);
            }

            // SET ENTRY TEXT AS THE VALUE

            if (Entries[col.ToString()][row].Text != "" && Entries[col.ToString()][row].Text != null)
            {
                if (double.TryParse(spreadsheet.GetCellValue("" + col.ToString().ToUpper() + (row + 1)).ToString(), out double value))
                {
                    Entries[col.ToString()][row].Text = value.ToString();
                    Entries[col.ToString()][row].TextColor = Color.FromArgb("#ffffff");
                }
                else
                {
                    await DisplayAlert("", "Formula Error was detected.", "Ok");
                    return;
                }
            }
            else
            {
                Entries[col.ToString()][row].Text = "";
            }

            // UPDATE ALL ENTRIES
            if (Entries[col.ToString()][row].Text != "") {
                foreach (string cell in toRecalculate)
                {
                    Entries[cell[0].ToString()][int.Parse(cell[1].ToString()) - 1].Text = spreadsheet.GetCellValue(cell).ToString();
                    Entries[cell[0].ToString()][int.Parse(cell[1].ToString()) - 1].TextColor = Color.FromArgb("#ffffff");
                }
            } 
            else
            {
                for (int i = 1; i < toRecalculate.Count; i++)
                {
                    Entries[toRecalculate[i][0].ToString()][int.Parse(toRecalculate[i][1].ToString()) - 1].Text = "=" + spreadsheet.GetCellContents(toRecalculate[i]).ToString();
                    Entries[toRecalculate[i][0].ToString()][int.Parse(toRecalculate[i][1].ToString()) - 1].TextColor = Color.FromArgb("#ff0000");
                }
            }
            

            // MOVE CURSOR TO CELL BENEATH (OR TOP)
            if (row == numOfLeftLabels - 1)
                Entries[col.ToString()][0].Focus();
            else
                Entries[col.ToString()][row + 1].Focus();
        }

        private void cellClickedOn(char col, int row)
        {
            // SET DISPLAYED CONTENTS
            if (Entries[col.ToString()][row - 1].GetContainsFormula() == true)
            {
                selectedCellContents.Text = "=" + spreadsheet.GetCellContents("" + col.ToString().ToUpper() + (row)).ToString();
                Entries[col.ToString()][row - 1].Text = selectedCellContents.Text;
            }
            else
                selectedCellContents.Text = spreadsheet.GetCellContents("" + col.ToString().ToUpper() + (row)).ToString();

            // SET DISPLAYED VALUE
            Type type = spreadsheet.GetCellValue("" + col.ToString().ToUpper() + (row)).GetType();

            if (type.Equals(typeof(SpreadsheetUtilities.FormulaError)))
                selectedCellValue.Text = "";
            else
                selectedCellValue.Text = spreadsheet.GetCellValue("" + col.ToString().ToUpper() + (row)).ToString();

            // SET DISPLAYED CELL NAME
            selectedCellName.Text = "" + col.ToString().ToUpper() + row;
        }

        private void cellUnfocused(char col, int row)
        {
            Type type = spreadsheet.GetCellValue("" + col.ToString().ToUpper() + (row + 1)).GetType();

            if (type.Equals(typeof(SpreadsheetUtilities.FormulaError)))
                Entries[col.ToString()][row].Text = "=" + spreadsheet.GetCellContents("" + col.ToString().ToUpper() + (row + 1)).ToString();
            else
                Entries[col.ToString()][row].Text = spreadsheet.GetCellValue("" + col.ToString().ToUpper() + (row + 1)).ToString();  
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
                    Entries[initialTopLabels[i].ToString()].Add(new MyEntry(j, handleCellChanged, cellClickedOn, cellUnfocused));
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
                TextColor = Color.FromRgb(212, 212, 210)
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
            addRow = new Button()
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
                TextColor = Color.FromRgb(255, 149, 0)
            };

            // ADD FUNCTION FOR THE CLICKED EVENT
            addRow.Clicked += AddRowClicked;

            // ADD BUTTON TO THE SPREADSHEET
            LeftSide.Add(addRow);
        }

        // COLOR CHANGING METHODS
        private void ChangeColorToRed(object sender, EventArgs e)
        {
            foreach (RowLabel label in LeftLabels)
            {
                label.Content.BackgroundColor = Color.FromArgb("#A7361C");
            }

            addRow.TextColor = Color.FromArgb("#A7361C");
            GUIColorTheme = "#A7361C";
        }

        private void ChangeColorToOrange(object sender, EventArgs e)
        {
            foreach (RowLabel label in LeftLabels)
            {
                label.Content.BackgroundColor = Color.FromArgb("#d1603d");
            }

            addRow.TextColor = Color.FromArgb("#d1603d");
            GUIColorTheme = "#d1603d";
        }

        private void ChangeColorToGreen(object sender, EventArgs e)
        {
            foreach (RowLabel label in LeftLabels)
            {
                label.Content.BackgroundColor = Color.FromArgb("#377868");
            }

            addRow.TextColor = Color.FromArgb("#377868");
            GUIColorTheme = "#377868";

        }

        private void ChangeColorToBlue(object sender, EventArgs e)
        {
            foreach (RowLabel label in LeftLabels)
            {
                label.Content.BackgroundColor = Color.FromArgb("#436d8f");
            }

            addRow.TextColor = Color.FromArgb("#436d8f");
            GUIColorTheme = "#436d8f";
        }

        private void ChangeColorToPurple(object sender, EventArgs e)
        {
            foreach (RowLabel label in LeftLabels)
            {
                label.Content.BackgroundColor = Color.FromArgb("#745987");
            }

            addRow.TextColor = Color.FromArgb("#745987");
            GUIColorTheme = "#745987";
        }
    } 
}