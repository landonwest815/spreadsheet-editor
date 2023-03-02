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
        // SETUP
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

        /// <summary>
        ///     Holds all entries within the spreadsheet
        /// </summary>
        private Dictionary<string, List<MyEntry>> Entries = new Dictionary<string, List<MyEntry>>();

        /// <summary>
        ///     Holds all the columns within the spreadsheet
        /// </summary>
        private List<VerticalStackLayout> Columns = new List<VerticalStackLayout>();

        /// <summary>
        ///     Button for adding additional rows
        /// </summary>
        private Button addRow;

        /// <summary>
        ///     Initial color theme of the spreadsheet is set to Orange
        ///     Can be changed through the color menu to the right of the file menu
        /// </summary>
        private string GUIColorTheme = "#d1603d";

        /// <summary>
        ///    Definition of what information (method signature) must be sent
        ///    by the Entry when it is modified.
        /// </summary>
        /// <param name="col"> col (char) in grid, e.g., A5 </param>
        /// <param name="row"> row (int) in grid,  e.g., A5 </param>
        public delegate void ActionOnCompleted(char col, int row);

        /// <summary>
        ///     Definition of what information (method signature) must be sent
        ///     by the Entry when it is focused.
        /// </summary>
        /// <param name="col"> col (char) in grid, e.g., A5 </param>
        /// <param name="row"> row (int) in grid,  e.g., A5 </param>
        public delegate void ActionOnFocused(char col, int row);

        /// <summary>
        ///     Definition of what information (method signature) must be sent
        ///     by the Entry when it is unfocused.
        /// </summary>
        /// <param name="col"> col (char) in grid, e.g., A5 </param>
        /// <param name="row"> row (int) in grid,  e.g., A5 </param>
        public delegate void ActionOnUnfocused(char col, int row);

        /// <summary>
        ///     Delegate for when a display warning should be shown to the user
        /// </summary>
        public delegate void OnDisplayWarning();

        /// <summary>
        ///     Extension of the Entry class that allows the majority of the spreadsheet
        ///     functionality to work
        ///     
        ///     Data includes:
        ///         row # (int)
        ///         the entries' column (char)
        ///         whether the entry contains a formula (bool)
        ///         
        ///     Delegates include:
        ///         functionality for when an entry is altered
        ///         functionality for when an entry is focused
        ///         functionality for when an entry is unfocused
        ///         
        ///     On Creation:
        ///         The entry takes in a row # and the 3 delegates mentioned above
        ///         
        ///     Getters & Setters:
        ///         This Entry class contains getter and setters for the column data and contains formula data
        ///         
        ///     *Additional Details mentioned in header comments throughout the class*
        /// </summary>
        public class MyEntry : Entry
        {
            // data
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

            /// <summary>
            ///   Function provided by "outside world" to be called whenever
            ///   this entry is unfocused
            /// </summary>
            private ActionOnUnfocused onUnfocus;

            /// <summary>
            ///   build an Entry element with the row "remembered"
            /// </summary>
            /// <param name="row"> unique identifier for this item </param>
            /// <param name="changeAction"> outside action that should be invoked after this cell is modified </param>
            public MyEntry(int row, ActionOnCompleted changeAction, ActionOnFocused focusAction, ActionOnUnfocused unfocusAction) : base()
            {
                // data
                this.row = row;

                // graphical settings
                this.HeightRequest = 40;
                this.WidthRequest = 75;
                this.FontAttributes = FontAttributes.Bold;
                this.FontSize = 15;
                this.HorizontalTextAlignment = TextAlignment.End;
                this.VerticalTextAlignment = TextAlignment.Center;

                // action to take when the user presses enter on this entry
                this.Completed += CellChangedValue;
                // action to take when the user clicks on this entry
                this.Focused += IsFocus;
                // action to take when the user clicks out of this entry
                this.Unfocused += CellChangedFocus;

                // "remember" outside worlds request about what to do when we change.
                onChange = changeAction;
                // "remember" outside worlds request about what to do when we focus.
                onFocus = focusAction;
                // "remember" outside worlds request about what to do when we unfocus.
                onUnfocus = unfocusAction;
            }

            /// <summary>
            ///   Remove focus and text from this entry
            /// </summary>
            public void ClearAndUnfocus()
            {
                this.Unfocus();
                this.Text = "";
            }

            /// <summary>
            ///     Set the column that this entry is in
            /// </summary>
            /// <param name="cellColumn"> the column that this entry is in </param>
            public void SetColumn(char cellColumn)
            {
                this.column = cellColumn;
            }

            /// <summary>
            ///     Set the bool of whether this entry contains a formula or not
            /// </summary>
            /// <param name="containsFormula"> true or false </param>
            public void SetContainsFormula(bool containsFormula)
            {
                if (containsFormula)
                    this.containsFormula = true;
                else
                    this.containsFormula = false;
            }

            /// <summary>
            ///     Gets the bool of whether this entry contains a formula or not
            /// </summary>
            /// <returns> true or false </returns>
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

            /// <summary>
            ///     Action to take when this entry is focused on (clicked on)
            /// </summary>
            /// <param name="sender"> ignored, but should == this </param>
            /// <param name="e"> ignored </param>
            private void IsFocus(object sender, EventArgs e)
            {
                // Inform the outside world that we have focused
                onFocus(column, row + 1);
            }

            /// <summary>
            ///     Action to take when this entry is unfocused (clicked out of)
            /// </summary>
            /// <param name="sender"> ignored, but should == this </param>
            /// <param name="e"> ignored </param>
            private void CellChangedFocus(object sender, EventArgs e)
            {
                // Inform the outside world that we have unfocused the entry
                onUnfocus(column, row);
            }
        }

        /// <summary>
        ///     Extension of the Label class that specifies graphical settings for all
        ///     column labels e.g. A, B, C...
        /// </summary>
        public class ColumnLabel : Border
        {
            /// <summary>
            ///     Constructor that only takes in the name of the column label (char)
            /// </summary>
            /// <param name="name"> the name of the column label </param>
            public ColumnLabel(char name) : base()
            {
                // graphical settings for border
                this.Stroke = Color.FromRgb(212, 212, 210);
                this.StrokeThickness = 0;
                this.HeightRequest = 40;
                this.WidthRequest = 75;
                this.HorizontalOptions = LayoutOptions.Center;
                this.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5, 5, 5, 5) };
                    
                // graphical settings for label
                Content =
                    new Label
                    {
                        Text = $"{name}",
                        TextColor = Color.FromRgb(28, 28, 28),
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 15,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        BackgroundColor = Color.FromRgb(212, 212, 210)
                    };
            }
        }

        /// <summary>
        ///     Extension of the Label class that specifies graphical settings for all
        ///     row labels e.g. 1, 2, 3...
        /// </summary>
        public class RowLabel : Border
        {
            /// <summary>
            ///     Constructor that only takes in the name of the row label (char)
            /// </summary>
            /// <param name="name"> the name of the row label </param>
            public RowLabel(int row) : base() 
            {
                // graphical settings for the border
                this.Stroke = Color.FromArgb("#d1603d");
                this.StrokeThickness = 0;
                this.HeightRequest = 40;
                this.WidthRequest = 75;
                this.HorizontalOptions = LayoutOptions.Center;
                this.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5, 5, 5, 5) };
                
                // graphical settings for the label
                Content =
                    new Label
                    {
                        Text = $"{row + 1}",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 15,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        BackgroundColor = Color.FromArgb("#d1603d")
                    };
               
            }
        }

        /// <summary>
        ///     Sets everything up for the program to run
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // when clear is clicked -> set the spreadsheet to a fresh one
            ClearAll += CreateNewSpreadsheet;

            // set the # of top labels to start with
            numOfTopLabels = initialTopLabels.Length;

            // set the entries to start with
            AddInitialEntriesToGrid(numOfTopLabels, numOfLeftLabels);

            // setup the top labels
            AddInitialTopLabels(numOfTopLabels);

            // set the button for additional columns functionality
            CreateNewColumnButton();

            // setup the left labels
            AddInitialLeftLabels(numOfLeftLabels);

            // set the button for additional rows functionality
            CreateNewRowButton();
        }

        /// <summary>
        ///     Sets the existing spreadsheet to a new spreadsheet with parameters:
        ///         s => true, s => s.ToUpper(), "six"
        /// </summary>
        private void CreateNewSpreadsheet()
        {
            spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "six");
        }

        /// <summary>
        ///     Event called when the new file dropdown option is clicked
        ///     If there is unsaved data -> calls the DataLoss method before invoking newFile
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void FileMenuNew(object sender, EventArgs e)
        {
            if (spreadsheet.Changed)
                DataLoss(newFile); // calls method that keeps the user from losing data
            else
                newFile();
        }

        /// <summary>
        ///     Creates a new file after checking for unsaved changes in the FileMenuNew event.
        ///     Shows the user a popup confirming the creation of a new file.
        /// </summary>
        private async void newFile()
        {
            Entries["A"][0].Focus(); // Focuses the top left entry
            ClearAll();
            CreateNewSpreadsheet();
            await DisplayAlert("", "A new spreadsheet has been created.", "Ok");
        }

        /// <summary>
        ///     Event called when the save file drop down option is clicked.
        ///     Asks the user for a name and saves it to the Desktop
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private async void FileMenuSave(object sender, EventArgs e)
        {
            // default path to save to
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // user input for naming of file
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

        /// <summary>
        ///     Event called when the open file drop down option is clicked.
        ///     If there is unsaved data -> call the DataLoss method before invoking openFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileMenuOpenAsync(object sender, EventArgs e)
        {
            if (spreadsheet.Changed)
                DataLoss(openFile); // calls method that keeps the user from losing data
            else
                openFile();
        }

        /// <summary>
        ///     Opens an existing file after checking for unsaved changes in the FileMenuOpenAsync event.
        ///     Shows the user a popup confirming the opening of an existing file.
        /// </summary>
        private async void openFile()
        {
            // user selected file
            FileResult? fileResult = await FilePicker.Default.PickAsync();
            if (fileResult != null)
            {
                ClearAll();
                OpenFilePath(fileResult);
            }
        }

        /// <summary>
        ///     Opens a file path into the spreadsheet editor
        /// </summary>
        /// <param name="filePath"></param>
        private async void OpenFilePath(FileResult filePath)
        {
            try
            {
                // set the existing spreadsheet to the opened spreadsheet
                spreadsheet = new Spreadsheet(filePath.FullPath, s => true, s => s.ToUpper(), "six");
                
                // load all of the entries
                foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    try // it is a formula
                    {
                        Formula formula = (Formula)spreadsheet.GetCellContents(cell);
                        Entries[cell[0].ToString()][int.Parse(cell[1].ToString()) - 1].SetContainsFormula(true);
                        Entries[cell[0].ToString()][int.Parse(cell[1].ToString()) - 1].Text = spreadsheet.GetCellValue(cell).ToString();
                    }
                    catch (InvalidCastException) // it is a number or string
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

        /// <summary>
        ///     Keeps the user from losing data.
        ///     Asks them if they want to follow through with an event.
        /// </summary>
        /// <param name="afterWarning"></param>
        private async void DataLoss(OnDisplayWarning afterWarning)
        {
            // gets the users decision (lose data or not)
            bool overwrite = await DisplayAlert("Warning", "There are unsaved Changes. \n Do you want to contiue?", "Yes", "No");

            // if they want to lose the data -> go through with the process
            if (overwrite)
                afterWarning();
        }

        /// <summary>
        ///     Event called when the add column button is pressed.
        ///     Simply adds an entire column with the next up label.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private void AddColumnClicked(object sender, EventArgs e)
        {
            // adds a new column of entries
            Entries.Add(allTopLabels[numOfTopLabels].ToString(), new List<MyEntry>());

            // add a new column
            VerticalStackLayout column = new VerticalStackLayout();
            Columns.Add(column);
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
                try
                {
                    toRecalculate = spreadsheet.SetContentsOfCell("" + col.ToString().ToUpper() + (row + 1), Entries[col.ToString()][row].Text);
                }
                catch (CircularException)
                {
                    Entries[col.ToString()][row].Text = "";
                    spreadsheet.SetContentsOfCell("" + col.ToString().ToUpper() + (row + 1), "");
                    Entries[col.ToString()][row].SetContainsFormula(false);
                    await DisplayAlert("Error", "Circular Dependencies are not allowed", "Ok");
                    return;
                }

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
                if (double.TryParse(spreadsheet.GetCellValue("" + col.ToString().ToUpper() + (row + 1)).ToString(), out double value) || Entries[col.ToString()][row].Text[0] != '=')
                {
                    Entries[col.ToString()][row].Text = value.ToString();
                    Entries[col.ToString()][row].TextColor = Color.FromArgb("#ffffff");
                }
                else
                {
                    await DisplayAlert("", "Formula Error was detected.", "Ok");
                    Entries[col.ToString()][row].TextColor = Color.FromArgb("#ff0000");
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
            else{
                selectedCellValue.Text = spreadsheet.GetCellValue("" + col.ToString().ToUpper() + (row)).ToString();
                Entries[col.ToString()][row - 1].TextColor = Color.FromArgb("ffffff");

            }

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