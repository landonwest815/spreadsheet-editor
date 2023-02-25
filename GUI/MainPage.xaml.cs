namespace GUI
{
    public partial class MainPage : ContentPage
    { 
        public MainPage()
        {
            InitializeComponent();

            TopLabels.Add(
                new Border
                {
                    StrokeThickness = 0,
                    HeightRequest = 35,
                    WidthRequest = 70,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                        }
                }
            );

            TopLabels.Add(
                new Border
                {
                    StrokeThickness = 0,
                    HeightRequest = 35,
                    WidthRequest = 70,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = $"A",
                            BackgroundColor = Color.FromRgb(212, 212, 210),
                            TextColor = Color.FromRgb(28, 28, 28),
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                }
            );

            TopLabels.Add(
                new Border
                {
                    StrokeThickness = 0,
                    HeightRequest = 35,
                    WidthRequest = 70,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = $"B",
                            BackgroundColor = Color.FromRgb(212, 212, 210),
                            TextColor = Color.FromRgb(28, 28, 28),
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                }
            );

            TopLabels.Add(
                new Border
                {
                    StrokeThickness = 0,
                    HeightRequest = 35,
                    WidthRequest = 70,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = $"C",
                            BackgroundColor = Color.FromRgb(212, 212, 210),
                            TextColor = Color.FromRgb(28, 28, 28),
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                }
            );

            LeftLabels.Add(
                new Border
                {
                    BackgroundColor = Color.FromRgb(255, 149, 0),
                    StrokeThickness = 0,
                    HeightRequest = 35,
                    WidthRequest = 70,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = $"1",
                            BackgroundColor = Color.FromRgb(255, 149, 0),
                            TextColor = Color.FromRgb(255, 255, 255),
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                }
            );

            LeftLabels.Add(
                new Border
                {
                    BackgroundColor = Color.FromRgb(255, 149, 0),
                    StrokeThickness = 0,
                    HeightRequest = 35,
                    WidthRequest = 70,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = $"2",
                            BackgroundColor = Color.FromRgb(255, 149, 0),
                            TextColor = Color.FromRgb(255, 255, 255),
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                }
            );

            LeftLabels.Add(
                new Border
                {
                    BackgroundColor = Color.FromRgb(255, 149, 0),
                    StrokeThickness = 0,
                    HeightRequest = 35,
                    WidthRequest = 70,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = $"3",
                            BackgroundColor = Color.FromRgb(255, 149, 0),
                            TextColor = Color.FromRgb(255, 255, 255),
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                }
            );
        }

        private void FileMenuNew(object sender, EventArgs e)
        {
           
        }

        private void FileMenuOpenAsync(object sender, EventArgs e)
        {

        }


    }
}