using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        private string topLabels = "ABCDE";
        private string leftLabels = "12345";

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
        ///   List of Entries to show how to "move around" via enter key
        /// </summary>
        private MyEntry[] EntryColumn = new MyEntry[3];

        /// <summary>
        ///    Definition of what information (method signature) must be sent
        ///    by the Entry when it is modified.
        /// </summary>
        /// <param name="col"> col (char) in grid, e.g., A5 </param>
        /// <param name="row"> row (int) in grid,  e.g., A5 </param>
        public delegate void ActionOnCompleted(char col, int row);

        public class MyEntry : Entry
        {
            //JIM: better?--> int row = 0;

            /// <summary>
            ///   Function provided by "outside world" to be called whenever
            ///   this entry is modified
            /// </summary>
            private ActionOnCompleted onChange;

            /// <summary>
            ///   build an Entry element with the row "remembered"
            /// </summary>
            /// <param name="row"> unique identifier for this item </param>
            /// <param name="changeAction"> outside action that should be invoked after this cell is modified </param>
            public MyEntry(int row, ActionOnCompleted changeAction) : base()
            {
                //JIM: better?-->        this.row = row;
                this.StyleId = $"{row}";

                // Action to take when the user presses enter on this cell
                this.Completed += CellChangedValue;

                // "remember" outside worlds request about what to do when we change.
                onChange = changeAction;
            }

            /// <summary>
            ///   Remove focus and text from this widget
            /// </summary>
            public void ClearAndUnfocus()
            {
                this.Unfocus();
                this.Text = "";
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
                onChange('A', Convert.ToInt32(this.StyleId));
                //Jim Better? --> onChange( 'A', row );
            }

        }

        public MainPage()
        {
            InitializeComponent();

            // MAKE THE BACKGROUND A GOOD COLOR
            Entire.BackgroundColor = Color.FromRgb(28, 28, 28);

            // ADD EMPTY CORNER TO THE TOP LEFT OF THE GRID
            TopLabels.Add(
                new Border
                {
                    StrokeThickness = 0,
                    HeightRequest = 40,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = ""
                        }
                }
                );

            // ADD ALL TOP LABELS (A B C D E...)
            for (int i = 0; i < topLabels.Length; i++)
            {
                TopLabels.Add(
                new Border
                {
                    Stroke = Color.FromRgb(212, 212, 210),
                    StrokeThickness = 0,
                    HeightRequest = 40,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Center,
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(5, 5, 5, 5)
                    },
                    Content =
                        new Label
                        {
                            Text = $"{topLabels[i]}",
                            TextColor = Color.FromRgb(28, 28, 28),
                            BackgroundColor = Color.FromRgb(212, 212, 210),
                            FontSize = 15,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center
                        }
                }
                );
            }

            // ADD ALL LEFT LABELS (1 2 3 4 5...)
            for (int i = 0; i < leftLabels.Length; i++)
            {
                LeftLabels.Add(
                new Border
                {
                    Stroke = Color.FromRgb(255, 149, 0),
                    StrokeThickness = 0,
                    HeightRequest = 40,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Center,
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(5, 5, 5, 5)
                    },
                    Content =
                        new Label
                        {
                            Text = $"{leftLabels[i]}",
                            FontAttributes = FontAttributes.Bold,
                            BackgroundColor = Color.FromRgb(255, 149, 0),
                            FontSize = 15,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center
                        }
                }
                );
            }
        }

        private void FileMenuNew(object sender, EventArgs e)
        {
           
        }

        private void FileMenuOpenAsync(object sender, EventArgs e)
        {

        }

        

    }
}