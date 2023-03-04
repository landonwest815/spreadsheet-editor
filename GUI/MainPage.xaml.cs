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
    /// 
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
    /// 
    /// This Project makes use of the Spreadsheet class in a GUI via .NET Maui
    /// The goal of this project is make the GUI as user-friendly and error-free as possible.
    /// Cells can be clicked on and passed in information.
    /// This information can be a number, string, or formula.
    /// Anything else will be caught by the program.
    /// 
    /// </summary>
    public partial class MainPage : ContentPage
    {
        // SETUP
        private const string allTopLabels = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string initialTopLabels = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private int numOfTopLabels;
        private int numOfLeftLabels = 99;
        private EnhancedSpreadsheet spreadsheet;

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
        ///     Holds all entries that contain formulas
        ///     This helps update cells when a dependency is changed later on in the program
        /// </summary>
        private List<MyEntry> FormulaEntries = new List<MyEntry>();

        /// <summary>
        ///     Holds all the columns within the spreadsheet
        /// </summary>
        private List<VerticalStackLayout> Columns = new List<VerticalStackLayout>();

        /// <summary>
        ///     Button for adding additional rows
        /// </summary>
        private Button addRow;

        /// <summary>
        ///     Widget at the top of the spreadsheet for changing the contents of a cell
        /// </summary>
        private ContentsEntry contentsWidget;

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
        ///     Definition of what information (method signature must be sent
        ///     by the ContentsEntry when it is modified.
        /// </summary>
        /// <param name="col"> col (char) in grid, e.g., A5 </param>
        /// <param name="row"> row (int) in grid,  e.g., A5 </param>
        public delegate void ActionOnContentsWidgetCompleted(char col, int row, bool fromContentsWidget);

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
        ///     Extension of the Spreadsheet class
        ///     This adds functionality for saving a spreadsheet that has already been saved.
        ///     The user does not have to enter the name again.
        ///     If they wish to save the spreadsheet under a new name they can select 'Save As' under the File Menu
        /// </summary>
        public class EnhancedSpreadsheet : Spreadsheet
        {
            // data
            private string savePath = "";
            private string saveName = "";

            /// <summary>
            ///     Contructor that takes in a filepath, two delegate functions, and a version
            /// </summary>
            /// <param name="filepath"> filepath to load spreadsheet from </param>
            /// <param name="isValid"> delegate passed in to check the validity of variable names </param>
            /// <param name="normalize"> delegate passed in to normalize variable names </param>
            /// <param name="version"> version of the spreadsheet</param>
            public EnhancedSpreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(filepath, isValid, normalize, version) {}
           
            /// <summary>
            ///     Constructor that takes in two delegate functions and a version
            /// </summary>
            /// <param name="isValid"> delegate passed in to check the validity of the names </param>
            /// <param name="normalize"> delegate passed in to normalize the names </param>
            /// <param name="version"> version information </param>
            public EnhancedSpreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version) {}

            /// <summary>
            ///     Setter for the save path of the spreadsheet
            /// </summary>
            /// <param name="path"> save path </param>
            public void SetSavePath(string path)
            {
                savePath = path;
            }

            /// <summary>
            ///     Getter for the save path of the spreadsheet
            /// </summary>
            /// <returns> the spreadsheet's save path </returns>
            public string GetSavePath()
            {
                return savePath;
            }

            /// <summary>
            ///     Setter for the save name of the spreadsheet
            /// </summary>
            /// <param name="name"> the save name </param>
            public void SetSaveName(string name)
            {
                saveName = name;
            }

            /// <summary>
            ///     Getter for the save name of the spreadsheet
            /// </summary>
            /// <returns> the save name </returns>
            public string GetSaveName()
            {
                return saveName;
            }

        }

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
        ///     Getters and Setters:
        ///         This Entry class contains getter and setters for the column data and contains formula data
        ///         
        ///     *Additional Details mentioned in header comments throughout the class*
        /// </summary>
        public class MyEntry : Entry
        {
            // data
            private int row = 0;
            private char column;
            private bool containsFormula = false;
            private bool containsFormulaError = false;

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
            ///     Set the bool of whether this entry contains a formula error or not
            /// </summary>
            /// <param name="containsFormulaError"> true or false </param>
            public void SetContainsFormulaError(bool containsFormulaError)
            {
                if (containsFormulaError)
                    this.containsFormulaError = true;
                else
                    this.containsFormulaError = false;
            }

            /// <summary>
            ///     Gets the bool of whether this entry contains a formula error or not
            /// </summary>
            /// <returns> true or false </returns>
            public bool GetContainsFormulaError()
            {
                return this.containsFormulaError;
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
        ///     Extension of the Entry class that specifies functionality for the Contents Widget located at the
        ///     top of the spreadsheet
        /// </summary>
        public class ContentsEntry : Entry
        {
            // data
            char currentColumn;
            int currentRow;

            /// <summary>
            ///   Function provided by "outside world" to be called whenever
            ///   this entry is modified
            /// </summary>
            private ActionOnContentsWidgetCompleted onChangeContents;

            /// <summary>
            ///     Contstructor that takes in a delegate function for when this entry is 'completed'
            /// </summary>
            /// <param name="changeAction"> delegate passed in </param>
            public ContentsEntry(ActionOnContentsWidgetCompleted changeAction) : base()
            {
                this.Completed += CellChangedValue;
                onChangeContents = changeAction;
            }

            /// <summary>
            ///     Method to call when the function is completed
            ///     Updates necessary information and cells in the spreadsheet
            /// </summary>
            /// <param name="sender"> ignored </param>
            /// <param name="e"> ignored </param>
            private void CellChangedValue(object sender, EventArgs e)
            {
                Unfocus();

                // Inform the outside world that we have changed
                onChangeContents(currentColumn, currentRow - 1, true);
            }

            /// <summary>
            ///     Setter for the current row and column that the contents widget is referencing
            /// </summary>
            /// <param name="col"> current column </param>
            /// <param name="row"> current row </param>
            public void SetColumnAndRow(char col, int row)
            {
                currentColumn = col;
                currentRow = row;
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
            /// <param name="row"> the name of the row label </param>
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

            // create the initial spreadsheet
            CreateNewSpreadsheet();

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

            // set the widget for contents changing at the top of the spreadsheet
            AddContentsWidget();
        }

        /// <summary>
        ///     Sets the existing spreadsheet to a new spreadsheet with parameters:
        ///         s => true, s => s.ToUpper(), "six"
        /// </summary>
        private void CreateNewSpreadsheet()
        {
            spreadsheet = new EnhancedSpreadsheet(s => true, s => s.ToUpper(), "six");
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
            Entries["A"][0].Focus(); // Focuses the top left entry (will always be in the column A and in the row 0)
            ClearAll();
            CreateNewSpreadsheet();
            await DisplayAlert("", "A new spreadsheet has been created.", "Ok");
        }

        /// <summary>
        ///     Event called when the save file drop down option is clicked.
        ///     Asks the user for a name and saves it to the Desktop
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void FileMenuSave(object sender, EventArgs e)
        {
            if (spreadsheet.GetSavePath() != "") // if it has been saved before -> save to same place with same same
                saveFile(spreadsheet.GetSavePath(), spreadsheet.GetSaveName());
            else
                FileMenuSaveAs(null, null); // both params can be ignored
        }

        /// <summary>
        ///     Event called for two different circumstances:
        ///         1. Spreadsheet has never been saved before
        ///         2. User chooses 'Save As' in the File Menu
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private async void FileMenuSaveAs(object sender, EventArgs e)
        {
            // default path to save to is set to the users desktop
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // retrieve user input for file name
            string fileName = await DisplayPromptAsync("Save Spreadsheet", "Name of file:", "Save");

            // save it
            if (fileName != null)
            {
                string filePath = path + "\\" + fileName + ".sprd";
                saveFile(filePath, fileName);
            }
        }

        /// <summary>
        ///     Event for saving a spreadsheet file
        ///     Naming is taken care of before reaching this method
        /// </summary>
        /// <param name="path"> file path to save to </param>
        /// <param name="name"> file name to save with </param>
        private async void saveFile(string path, string name)
        {
            try
            {
                spreadsheet.Save(path);
                await DisplayAlert("", "Successfully saved '" + name + "' to the Desktop", "Ok");

                // if the user saves this spreadsheet again it will automatically save using these variables
                spreadsheet.SetSavePath(path);
                spreadsheet.SetSaveName(name);
            }
            catch (Exception)
            {
                await DisplayAlert("", "Failed to save '" + name + "' to the Desktop", "Ok");
            }
        }

        /// <summary>
        ///     Event called when the open file drop down option is clicked.
        ///     If there is unsaved data -> call the DataLoss method before invoking openFile
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void FileMenuOpenAsync(object sender, EventArgs e)
        {
            // check if user will lose data
            if (spreadsheet.Changed)
                DataLoss(openFile);
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
            var fileResult = await FilePicker.Default.PickAsync();

            if (fileResult != null)
            {
                ClearAll();
                OpenFilePath(fileResult);
            }
        }

        /// <summary>
        ///     Opens a user selected file path into the spreadsheet editor
        /// </summary>
        /// <param name="filePath"> the file path to open from </param>
        private async void OpenFilePath(FileResult filePath)
        {
            try
            {
                // set the existing spreadsheet to the opened spreadsheet
                spreadsheet = new EnhancedSpreadsheet(filePath.FullPath, s => true, s => s.ToUpper(), "six");

                MyEntry currentCell;
                string currentCellColumn;
                string currentCellRow;

                // load all of the cells
                foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    // retrieve row and column information
                    currentCellColumn = "";
                    currentCellRow = "";
                    foreach (Char c in cell)
                    {
                        if (Char.IsLetter(c))
                            currentCellColumn += c;
                        else
                            currentCellRow += c;
                    }

                    // load the current cell
                    try
                    {
                        currentCell = Entries[currentCellColumn][int.Parse(currentCellRow) - 1];

                        // if it is a formula
                        try
                        {
                            Formula formula = (Formula)spreadsheet.GetCellContents(cell);
                            currentCell.SetContainsFormula(true);
                            FormulaEntries.Add(currentCell);
                            currentCell.Text = spreadsheet.GetCellValue(cell).ToString();
                        }

                        // if it is a number or string
                        catch (InvalidCastException)
                        {
                            currentCell.Text = spreadsheet.GetCellValue(cell).ToString();
                        }

                    }

                    // if the spreadsheet does not contain enough cells required display an alert
                    catch (KeyNotFoundException)
                    {
                        await DisplayAlert("Load Failure", "Spreadsheet editor does not contain required number of rows/columns. \nPlease add rows/columns needed for your desired spreadsheet and then try opening again!", "Ok");
                        return;
                    }
                }

                // if everything goes well...
                await DisplayAlert("", "Successfully opened '" + filePath.FileName + "'", "Ok");
            }

            // if everything does not go well...
            catch (SpreadsheetReadWriteException)
            {
                await DisplayAlert("Load Failure", "Invalid Filepath", "Ok");
            }
            catch (FileNotFoundException)
            {
                await DisplayAlert("Load Failure", "File does not exist", "Ok");
            }
        }

        /// <summary>
        ///     Event for when the user selects 'Exit' under the File Menu
        ///     Notifies the user if there will be data loss on exit
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void FileMenuExit(object sender, EventArgs e)
        {
            DataLoss(exitProgram);
        }

        /// <summary>
        ///     Called after checking for data loss in the FileMenuExit Event
        /// </summary>
        private void exitProgram()
        {
            // this piece of code is from the piazza forums
            Application.Current?.CloseWindow(Application.Current.MainPage.Window);
        }

        /// <summary>
        ///     Keeps the user from losing data.
        ///     Asks them if they want to follow through with an event.
        /// </summary>
        /// <param name="afterWarning"> called after the method finishes its job </param>
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
        ///     Can only add up to 'Z' as of now.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void AddColumnClicked(object sender, EventArgs e)
        {
            // adds a new list to the entries dictionary (holds a column of cells)
            Entries.Add(allTopLabels[numOfTopLabels].ToString(), new List<MyEntry>());

            // add a new column to the GUI
            VerticalStackLayout column = new VerticalStackLayout();
            Columns.Add(column);
            Grid.Add(column);

            // add entry to the end of each row
            char newColumnName = allTopLabels[numOfTopLabels];
            List<MyEntry> newColumn = Entries[newColumnName.ToString()];
            for (int currentRow = 0; currentRow < numOfLeftLabels; currentRow++)
            {
                // add current entry to the Entries dictionary
                newColumn.Add(new MyEntry(currentRow, pressedEnter, cellClickedOn, cellUnfocused));
                // add the entry above to GUI structure 
                column.Add(newColumn[currentRow]);
                // set the column name
                newColumn[currentRow].SetColumn(newColumnName);
                // add the entry to the clear all event
                ClearAll += newColumn[currentRow].ClearAndUnfocus;
            }

            // insert the newColumnName before the Add Column Button
            TopLabels.Insert(TopLabels.Count - 1, new ColumnLabel(newColumnName));

            // increment the number of top labels
            numOfTopLabels++;


            // turn the button off if the columns have reached 'Z'
            if (numOfTopLabels == 26)
                addColumnButton.IsVisible = false;
        }

        /// <summary>
        ///     Event called when the add row button is pressed.
        ///     Simply adds an entire row with the next up row number.
        ///     Can add as many rows as desired (will affect how fast the program runs)
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void AddRowClicked(object sender, EventArgs e)
        {
            // add entry to the end of each column
            int newRow = numOfLeftLabels;
            List<MyEntry> currentColumn;
            for (int i = 0; i < numOfTopLabels; i++)
            {
                // set the current column
                currentColumn = Entries[allTopLabels[i].ToString()];
                // add current entry to the Entries dictionary
                currentColumn.Add(new MyEntry(newRow, pressedEnter, cellClickedOn, cellUnfocused));
                // add to the GUI
                Columns[i].Add(currentColumn[newRow]);
                // set the column name of each entry
                currentColumn[newRow].SetColumn(allTopLabels[i]);
                // add the entry to the clear all event
                ClearAll += currentColumn[newRow].ClearAndUnfocus;
            }

            // insert the new row label above the add row button (increments left label counter)
            RowLabel newLabel = new RowLabel(numOfLeftLabels++);
            LeftLabels.Add(newLabel);

            // adjust the color to the currently selected theme
            newLabel.Content.BackgroundColor = Color.FromRgba(GUIColorTheme);
        }

        /// <summary>
        ///     Event called when the user pressed enter while focused on an entry.
        ///     If the entry is the last in the column the focus moves to the top entry.
        ///     This ends up triggering the Unfocused event for an entry and updates all information.
        /// </summary>
        /// <param name="col"> the column of the entry </param>
        /// <param name="row"> the row of the entry </param>
        private void pressedEnter(char col, int row)
        {
            if (row == numOfLeftLabels - 1)
                Entries[col.ToString()][0].Focus();
            else
                Entries[col.ToString()][row + 1].Focus();
        }

        /// <summary>
        ///   This method will be called by the individual Entry elements when it is unfocused.
        ///   All information will be updated accordingly
        /// </summary>
        /// <param name="col"> e.g., The 'A' in A5 </param>
        /// <param name="row"> e.g., The  5  in A5 </param>
        private async void handleCellChanged(char col, int row, bool fromContentsWidget)
        {
            IList<string> toRecalculate = new List<string>();
            MyEntry alteredCell = Entries[col.ToString()][row];
            string alteredCellName = "" + col.ToString().ToUpper() + (row + 1);

            if (fromContentsWidget)
            {
                // set the text and focus on the cell
                alteredCell.Text = contentsWidget.Text;
                alteredCell.Focus();

                // set the contents and refocus on the cell
                pressedEnter(col, row);
                alteredCell.Focus();

                return;
            }

            // set the contents of the altered cell
            if (alteredCell.Text != null)
            {
                try
                {
                    toRecalculate = spreadsheet.SetContentsOfCell(alteredCellName, alteredCell.Text);
                }
                catch (Exception ex)
                {
                    // remove the cell from the spreadsheet (entry is now completely fresh)
                    alteredCell.Text = "";
                    spreadsheet.SetContentsOfCell(alteredCellName, "");
                    alteredCell.SetContainsFormula(false);
                    FormulaEntries.Remove(alteredCell);

                    // display the error to the user
                    if (ex is CircularException)
                        await DisplayAlert("Error", "Circular Dependencies are not allowed", "Ok");
                    if (ex is FormulaFormatException)
                        await DisplayAlert("Error", "Invalid Formula Format entered", "Ok");

                    return;
                }
            }

            if (alteredCell.Text != "" && alteredCell.Text != null)
            {
                // if the user entered a formula
                if (alteredCell.Text[0] == '=')
                {
                    alteredCell.SetContainsFormula(true);
                    FormulaEntries.Add(alteredCell);
                }

                // if the user entered a number/string
                else
                {
                    // takes care of the case where the previous contents contained a formula
                    alteredCell.SetContainsFormula(false); 
                    FormulaEntries.Remove(alteredCell);
                }
            }

            // if the user entered an empty entry
            else
            {
                // takes care of the case where the previous contents contained a formula
                Entries[col.ToString()][row].SetContainsFormula(false);
                FormulaEntries.Remove(Entries[col.ToString()][row]);
            }

            // set the cell's text to its value
            if (alteredCell.Text != "" && alteredCell.Text != null)
            {
                // try to retrieve the value of the entered cell
                if (double.TryParse(spreadsheet.GetCellValue(alteredCellName).ToString(), out double value) || alteredCell.Text[0] != '=')
                {
                    alteredCell.Text = value.ToString();

                    if (fromContentsWidget)
                        selectedCellValue.Text = value.ToString();

                    // takes care of the case where the previous contents contained a formula error
                    alteredCell.TextColor = Color.FromArgb("#ffffff");
                   
                    // takes care of the case where the previous contents contained a formula
                    alteredCell.SetContainsFormulaError(false);
                }

                // if the value is not retrieved correctly...
                else
                {
                    // this if statement makes it so the user only gets notified on creation of the formula error
                    // and not every time they focus -> unfocus it
                    if (!alteredCell.GetContainsFormulaError())
                    {
                        await DisplayAlert("", "Formula Error was detected.", "Ok");
                        alteredCell.SetContainsFormulaError(true);
                        alteredCell.TextColor = Color.FromArgb("#ff0000");
                    }
                    return;
                }
            }

            // if the user entered an empty entry
            else
                alteredCell.Text = "";

            // update all dependent cells
            if (alteredCell.Text != "")
            {
                // loop through each dependent cell
                foreach (string cell in toRecalculate)
                {
                    // retrieve the row and column information of the current dependent cell
                    string cellColumn = "";
                    string cellRow = "";
                    foreach (Char c in cell)
                    {
                        if (Char.IsLetter(c))
                            cellColumn += c;
                        else
                            cellRow += c;
                    }

                    // if the current dependent cell still contains a formula error don't update it
                    Object value = spreadsheet.GetCellValue(cell);
                    if (!value.GetType().Equals(typeof(FormulaError)))
                    {
                        Entries[cellColumn][int.Parse(cellRow) - 1].Text = value.ToString();

                        // takes care of the case where the previous contents contained a formula error
                        Entries[cellColumn][int.Parse(cellRow) - 1].TextColor = Color.FromArgb("#ffffff");
                        Entries[col.ToString()][row].SetContainsFormulaError(false);
                    }
                }
            }
            
            // if the user entered an empty entry
            else
            {
                // loop through the dependent cells and turn them into formula errors (however, don't update itself)
                for (int i = 1; i < toRecalculate.Count; i++)
                {
                    Entries[toRecalculate[i][0].ToString()][int.Parse(toRecalculate[i][1].ToString()) - 1].Text = "=" + spreadsheet.GetCellContents(toRecalculate[i]).ToString();
                    Entries[toRecalculate[i][0].ToString()][int.Parse(toRecalculate[i][1].ToString()) - 1].TextColor = Color.FromArgb("#ff0000");
                }
            }
        }

        /// <summary>
        ///     Event called when the user clicks on a cell.
        ///     Display all the proper information in the widgets at the top of the spreadsheet.
        ///     Checks for formula errors and deals with them accordingly.
        /// </summary>
        /// <param name="col"> the column of the selected cell </param>
        /// <param name="row"> the row of the selected cell </param>
        private void cellClickedOn(char col, int row)
        {
            MyEntry currentCell = Entries[col.ToString()][row - 1];
            string currentCellName = "" + col.ToString().ToUpper() + (row);

            // set the displayed contents at the top of the spreadsheet
            if (currentCell.GetContainsFormula() == true)
            {
                // prepend a '=' sign if the content is a formula
                contentsWidget.Text = "=" + spreadsheet.GetCellContents(currentCellName);
                currentCell.Text = contentsWidget.Text;
            }
            else
                contentsWidget.Text = spreadsheet.GetCellContents("" + col.ToString().ToUpper() + (row)).ToString();

            contentsWidget.SetColumnAndRow(col, row);

            // set the displayed value at the top of the spreadsheet
            if (!spreadsheet.GetCellValue(currentCellName).GetType().Equals(typeof(FormulaError)))
            {
                selectedCellValue.Text = spreadsheet.GetCellValue(currentCellName).ToString();
                currentCell.TextColor = Color.FromArgb("ffffff");
            }

            // display no value if the current cell contains a formula error
            else
                selectedCellValue.Text = "";

            // set the displayed cell name at the top of the spreadsheet
            selectedCellName.Text = currentCellName;
        }

        /// <summary>
        ///     Event called when the user clicks out of a cell.
        ///     Displays the proper information in the cell that was clicked out of.
        ///     Checks for formula errors and deals with them accordingly.
        /// </summary>
        /// <param name="col"> the column of the cell that was clicked out of </param>
        /// <param name="row"> the row of the cell that was clicked out of </param>
        private void cellUnfocused(char col, int row)
        {
            // update the spreadsheet information
            handleCellChanged(col, row, false);

            // checks if the cell contained a formula error and displays the contents of it if so
            if (spreadsheet.GetCellValue("" + col.ToString().ToUpper() + (row + 1)).GetType().Equals(typeof(FormulaError)))
                Entries[col.ToString()][row].Text = "=" + spreadsheet.GetCellContents("" + col.ToString().ToUpper() + (row + 1)).ToString();
            
            // if it didn't then display the value
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
            // ensures that no entries contain a formula anymore
            foreach (MyEntry entry in FormulaEntries)
            {
                entry.SetContainsFormula(false);
            }

            // clear everything, set the spreadsheet to a fresh one, and focus the first cell
            ClearAll();
            CreateNewSpreadsheet();
            Entries[initialTopLabels[0].ToString()][0].Focus();
        }

        /// <summary>
        ///     Adds all the initial entries to the grid.
        /// </summary>
        /// <param name="numOfColumns"> # of columns to start with </param>
        /// <param name="numOfRows"> # of rows to start with </param>
        private void AddInitialEntriesToGrid(int numOfColumns, int numOfRows)
        {
            // loop through each column and add the appropiate number of entries ( 5 rows -> 5 entries )
            string currentColumnName;
            for (int i = 0; i < numOfColumns; i++)
            {
                // set the current column's name
                currentColumnName = initialTopLabels[i].ToString();
                // add a new list to the column
                Entries.Add(currentColumnName, new List<MyEntry>());
                // add neccesary GUI structures
                VerticalStackLayout column = new VerticalStackLayout();
                Columns.Add(column);
                Grid.Add(column);

                // loop through the current column and add the entries
                List<MyEntry> currentColumn;
                for (int j = 0; j < numOfRows; j++)
                {
                    // set the current column
                    currentColumn = Entries[currentColumnName];
                    // add an entry to the current column
                    currentColumn.Add(new MyEntry(j, pressedEnter, cellClickedOn, cellUnfocused));
                    // add to the columns layout
                    column.Add(currentColumn[j]);
                    // set the column's label
                    currentColumn[j].SetColumn(initialTopLabels[i]);
                    // add entry to the clear all method
                    ClearAll += currentColumn[j].ClearAndUnfocus;
                }
            }
        }

        /// <summary>
        ///     Adds all the initial column labels to the spreadsheet.
        /// </summary>
        /// <param name="numOfColumns"> # of column labels to start with </param>
        private void AddInitialTopLabels(int numOfColumns)
        {
            // add all the column labels ( A, B, C, D, E, etc...)
            for (int i = 0; i < numOfColumns; i++)
            {
                TopLabels.Add(new ColumnLabel(initialTopLabels[i]));
            }
        }

        /// <summary>
        ///     Creates the add column button.
        ///     Allows user to add additional columns after creation.
        /// </summary>
        private void CreateNewColumnButton()
        {
            // If the initial columns contain all 26 letters there cannot be any additional columns
            if (initialTopLabels.Length == 26)
                return;

            // initialize the button
            Button addColumn = new Button()
            {
                // graphical settings
                HeightRequest = 30,
                WidthRequest = 75,
                BorderWidth = 0,
                Text = "+",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 25,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromRgb(28, 28, 28),
                TextColor = Color.FromRgb(212, 212, 210)
            };

            // add an method for the clicked event
            addColumn.Clicked += AddColumnClicked;

            // add button to the end of the column labels
            TopLabels.Add(addColumn);
        }

        /// <summary>
        ///     Adds all the initial row labels to the spreadsheet.
        /// </summary>
        /// <param name="numOfRows"> # of rows to start with </param>
        private void AddInitialLeftLabels(int numOfRows)
        {
            // add all the row labels (1, 2, 3, 4, 5, etc...)
            for (int i = 0; i < numOfLeftLabels; i++)
            {
                LeftLabels.Add(new RowLabel(i));
            }
        }

        /// <summary>
        ///     Creates the add row button.
        ///     Allows the user to add additional rows after creation
        /// </summary>
        private void CreateNewRowButton()
        {
            // initialize the button
            addRow = new Button()
            {
                // graphical settings
                HeightRequest = 30,
                WidthRequest = 75,
                BorderWidth = 0,
                Text = "+",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 25,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromRgb(28, 28, 28),
                TextColor = Color.FromRgb(255, 149, 0)
            };

            // add method to the clicked event
            addRow.Clicked += AddRowClicked;

            // add button to the end of the row labels
            LeftSide.Add(addRow);
        }

        /// <summary>
        ///     Creates the contents widget.
        ///     Allows the user to alter a cells contents from the top of the spreadsheet.
        /// </summary>
        private void AddContentsWidget()
        {
            // initialize the widget
            contentsWidget = new ContentsEntry(handleCellChanged)
            {
                // graphical settings
                Text = "",
                FontAttributes = FontAttributes.Bold,
                WidthRequest = 112.5,
                HeightRequest = 40,
                FontSize = 15,
                VerticalOptions = LayoutOptions.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Background = Color.FromArgb("#1C1C1C")
            };

            // add the widget to the top of the spreadsheet
            Widgets.Add(contentsWidget);
        }

        /// <summary>
        ///     This method changes the theme of the spreadsheet to Red.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void ChangeColorToRed(object sender, EventArgs e)
        {
            ChangeColor("#A7361C");
        }

        /// <summary>
        ///     This method changes the theme of the spreadsheet to Orange.
        ///     This theme is the default theme.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void ChangeColorToOrange(object sender, EventArgs e)
        {
            ChangeColor("#d1603d");
        }

        /// <summary>
        ///     This method changes the theme of the spreadsheet to Green.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void ChangeColorToGreen(object sender, EventArgs e)
        {
            ChangeColor("#377868");

        }

        /// <summary>
        ///     This method changes the theme of the spreadsheet to Blue.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private void ChangeColorToBlue(object sender, EventArgs e)
        {
            ChangeColor("#436d8f");
        }

        /// <summary>
        ///     This method changes the theme of the spreadsheet to Purple.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        private void ChangeColorToPurple(object sender, EventArgs e)
        {
            ChangeColor("#745987");
        }

        /// <summary>
        ///     All the color changing methods above pass in the desired color into this method and sets it to the theme.
        /// </summary>
        /// <param name="color"> the user selected color </param>
        private void ChangeColor(string color)
        {
            // sets the color of each row label
            foreach (RowLabel label in LeftLabels)
            {
                label.Content.BackgroundColor = Color.FromArgb(color);
            }

            // sets the color of the button
            addRow.TextColor = Color.FromArgb(color);

            // sets the color of the theme
            GUIColorTheme = color;
        }

        /// <summary>
        ///     This method displays help information for text inputs within the spreadsheet.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private async void HelpMenuText(object sender, EventArgs e)
        {
            await DisplayAlert("Text Inputs", "Simply enter any text into a cell, " +
                "and its Contents and Value will be set to that text. Input text " +
                "can be any combination of letters or numbers as long as it doesn't " +
                "start with an '='", "Ok");
        }

        /// <summary>
        ///     This method displays help information for number inputs within the spreadsheet.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private async void HelpMenuNumber(object sender, EventArgs e)
        {
            await DisplayAlert("Number Inputs", "Simply enter any number (whole or decimal) " +
                "into a cell, and its Contents and Value will be set to that input. " +
                "Input numbers can be any combination of numbers. (However, large enough " +
                "numbers will cause problems)", "Ok");
        }

        /// <summary>
        ///     This method displays help information for formula inputs within the spreadsheet.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private async void HelpMenuFormula(object sender, EventArgs e)
        {
            await DisplayAlert("Formula Inputs", "To enter a Formula, start the expression " +
                "with an '=' sign and continue with an arithmetic sequence of cell names. " +
                "Examples can include the following: \n \t '=A1+B1' '=G1' '=3*F2' \n Formulas " +
                "that create a circular dependency are not allowed. Formulas that result in a " +
                "Formula Error will not change anything and will be highlighted red. Fixing the " +
                "formula will result in a correct cell.", "Ok");
        }

        /// <summary>
        ///     This method displays help information for saving a spreadsheet.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private async void HelpMenuSaving(object sender, EventArgs e)
        {
            await DisplayAlert("Saving Spreadsheets", "To save your spreadsheet, select 'Save' " +
                "under the File Menu. The program will then prompt you to enter a name for your " +
                "spreadsheet. Do not provide a path or extension. Your spreadsheet will be saved " +
                "to your Desktop folder by default with the extension '.sprd'. If you wish to " +
                "move this from your Desktop, you may do so after it has been saved.", "Ok");
        }

        /// <summary>
        ///     This method displays help information for opening a spreadsheet.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private async void HelpMenuOpening(object sender, EventArgs e)
        {
            await DisplayAlert("Opening Spreadsheets", "To open an existing spreadsheet, " +
                "select 'Open' under the File Menu. The program will then allow you to select " +
                "a file from your file system. If there were unsaved changes within your current " +
                "spreadsheet you will be notified. If a valid file is selected it will then be " +
                "opened into the spreadsheet editor.", "Ok");
        }

        /// <summary>
        ///     This method displays help information for exiting a spreadsheet.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e">ignored </param>
        private async void HelpMenuExiting(object sender, EventArgs e)
        {
            await DisplayAlert("Exiting Spreadsheets", "Due to the limitations of the program " +
                "this spreadsheet editor is built on you will not be notified of unsaved changes " +
                "when exiting the program via the 'x button'. Instead, if you select 'Exit' under " +
                "the File Menu you will be notified. Also be aware that due to the limitiations of " +
                "the program the exit process is lengthy.", "Ok");
        }
    }
}