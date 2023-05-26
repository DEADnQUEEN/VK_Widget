using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenCvLogicWarp;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace my_app_1
{
    public partial class MainWindow : Window
    {
        private readonly Warper WRP = new Warper();
        private List<Button> Buttons;
        public double TranceparentOpacity = 0.6;
        private readonly double Step = 0.03;
        private IWebDriver Driver { get; set; } = null;
        private ChromeOptions ChromeOptions { get; set; } = new ChromeOptions();
        private ChromeDriverService ChromeDriverService { get; set; } = ChromeDriverService.CreateDefaultService();
        private readonly VKStruct VK = new VKStruct();
        private readonly List<Grid> Grids = new List<Grid>();
        private List<string> Dialogs = new List<string>();
        public void DriverSettingsSetter()
        {
            ChromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions.AddArgument("--headless");
            Driver = new ChromeDriver(options: ChromeOptions, service: ChromeDriverService);
        }
        public void CloseApp()
        {
            Driver.Quit();
            Close();

            foreach (var proc in System.Diagnostics.Process.GetProcessesByName("chromedriver"))
                proc.Kill();
            foreach (var proc in System.Diagnostics.Process.GetProcessesByName("chrome"))
                proc.Kill();
        }
        public void CloseApp(Exception ex)
        {
            Label Lb1 = new Label()
            {
                Content = ex.ToString(),
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                FontSize = 16,
            };

            MainView.Children.Clear();
            Grid.SetColumn(Lb1 , 1);
            Grid.SetRow(Lb1 , 1);
            MainView.Children.Add(Lb1);
        }
        private void Cleaner(Grid grid, int colomnDefinitions, int rowDefinitions)
        {
            Cleaner(grid);
            int min = Math.Min(rowDefinitions, colomnDefinitions);
            for (int i = 0; i < min; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            }
            for (int i = 0; i < (colomnDefinitions - min); i++) grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            for (int i = 0; i < (rowDefinitions - min); i++) grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        }
        private void Cleaner(Grid grid)
        {
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }
        private void VkUIset()
        {
            Cleaner(MainView, 2, 1);

            Button adder = new Button()
            {
                Width = 30,
                Height = 30,
                Style = (Style)win.Resources["Adder"],
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri("uiicons/plus_white.png", UriKind.Relative)),
                },
                Margin = new Thickness(5),
                Padding = new Thickness(3),
            };

            ComboBox comboBox = new ComboBox()
            {
                Style = (Style)win.Resources["ComboBoxDialogsStyle"],
                ItemContainerStyle = (Style)win.Resources["itemsStyle"],
                Width = 250,
                Height = adder.Height,
                ItemsSource = VK.Dialogs,
                MaxDropDownHeight = 400,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5),
            };

            Grid.SetColumn(adder, 0);
            Grid.SetRow(adder, int.MaxValue);
            MainView.Children.Add(adder);
            
            Grid.SetColumn(comboBox, 1);
            Grid.SetRow(comboBox, int.MaxValue);
            MainView.Children.Add(comboBox);

            adder.Click += (s, e) =>
            {
                if (comboBox.Text == "") return;

                Grid textGrid = new Grid()
                {
                    Name = 'g' + Grids.Count.ToString(),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition() { Width = new GridLength(30), },
                        new ColumnDefinition() { Width = GridLength.Auto, },
                        new ColumnDefinition() { Width = GridLength.Auto, }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition() { Height = new GridLength(adder.Height) },
                    },
                    Margin = new Thickness(5),
                };
                    
                Grids.Add(textGrid);

                Button delete = new Button()
                {
                    Name= 'b' + textGrid.Name,
                    Width = 30,
                    Height = 30,
                    Style = (Style)win.Resources["Adder"],
                    Content = new Image()
                    {
                        Source = new BitmapImage(new Uri("uiicons/plus45_white.png", UriKind.Relative)),
                    },
                };

                Label lbl = new Label()
                {
                    Name = 'l' + textGrid.Name,
                    Height = 30,
                    Content = comboBox.Text,
                    Width = comboBox.Width - 30,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                };

                Label brd = new Label()
                {
                    Name = 'n' + textGrid.Name,
                    Height = 30,
                    Content = VK.Numbers[VK.AllDialogs.IndexOf(comboBox.Text)],
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(7, 0, 5, 0),
                };

                delete.Click += async (ob, ev) =>
                {
                    int id = int.Parse(delete.Name.Substring(2));
                    MainView.Children.RemoveAt(id + 2);
                    Grids.RemoveAt(id);
                    MainView.RowDefinitions.RemoveAt(MainView.RowDefinitions.Count - 1);
                    for (int i = id; i < Grids.Count; i++)
                    {
                        Grids[i].Name = Grids[i].Name.Substring(0, 1) + id.ToString();
                        foreach (FrameworkElement child in Grids[i].Children)
                            child.Name = child.Name.Substring(0, 2) + id.ToString();
                        Grid.SetRow(Grids[i], i);
                    }
                    Dialogs.RemoveAt(id);
                    VK.Dialogs.Clear();
                    foreach (string d in VK.AllDialogs)
                    {
                        await Task.Delay(5);
                        if (!Dialogs.Contains(d))
                        {
                            VK.Dialogs.Add(d);
                        }
                    }
                };

                Grid.SetColumn(delete, 0);
                Grid.SetRow(delete, 0);
                textGrid.Children.Add(delete);

                Grid.SetRow(brd, 0);
                Grid.SetColumn(brd, 1);
                textGrid.Children.Add(brd);

                Grid.SetColumn(lbl, textGrid.ColumnDefinitions.Count - 1);
                Grid.SetRow(lbl, 0);
                textGrid.Children.Add(lbl);
                
                Grid.SetColumn(textGrid, 0);
                Grid.SetColumnSpan(textGrid, MainView.ColumnDefinitions.Count);
                Grid.SetRow(textGrid, MainView.RowDefinitions.Count - 1);

                MainView.RowDefinitions.Add(new RowDefinition());
                MainView.Children.Add(textGrid);
                Dialogs.Add(comboBox.Text);
                VK.Dialogs.Remove(comboBox.Text);
            };

            MainView.ColumnDefinitions[0].Width = new GridLength(adder.Width + adder.Margin.Right + adder.Margin.Left);
            MainView.ColumnDefinitions[1].Width = new GridLength(comboBox.Width + comboBox.Margin.Left + comboBox.Margin.Right);
            MainView.RowDefinitions[0].Height = new GridLength(adder.Height + adder.Margin.Top + adder.Margin.Bottom);
        }
        async public void Method()
        {
            Buttons = new List<Button>() 
            {
                pinButton,
                closeButton,
                minimizeButton,
                homeButton,
            };
            try
            {
                while (true)
                {
                    await Task.Delay(1);

                    if (Driver.Url == VK.AfterLogURL)
                    {
                        Driver.Navigate().GoToUrl(VK.MessagesURL);
                        VK.Login = true;
                        VkUIset();
                        ReadOnlyCollection<IWebElement> liWebElements = Driver.FindElement(By.CssSelector("#im_dialogs")).FindElements(By.XPath("li"));
                        VK.DialogNamesSetter(liWebElements);
                    }
                    while (VK.Login)
                    {
                        await Task.Delay(10000);
                        VK.ValOfDialogsCheck(Driver.FindElement(By.CssSelector("#im_dialogs")).FindElements(By.XPath("li")));
                    }
                }
            } catch (Exception ex) { CloseApp(ex); }
        }
        public MainWindow()
        {
            DriverSettingsSetter();
            InitializeComponent();

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!win.Topmost && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseApp();
        }

        async private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            if (win.Topmost)
            {
                win.Topmost = false;
                PinButtonImg.Source = new BitmapImage(new Uri("uiicons/unlock_white.png", UriKind.Relative));
                for (double i = TranceparentOpacity; i < 1; i += Step / 5)
                    foreach (Button button in Buttons)
                    {
                        button.Opacity += Step / 5; 
                        await Task.Delay(1);
                    }
            }
            else 
            {
                win.Topmost = true;
                PinButtonImg.Source = new BitmapImage(new Uri("uiicons/lock_white.png", UriKind.Relative));
                for (double i = 1; i > TranceparentOpacity; i -= Step)
                    foreach (Button btn in Buttons)
                    {
                        btn.Opacity -= Step;
                        await Task.Delay(1);
                    }
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            win.WindowState = WindowState.Minimized;
        }
        async private void VkClick(object sender, RoutedEventArgs e)
        {
            if (!VK.Login)
            {
                Driver.Navigate().GoToUrl(VK.URLLOG);

                byte[] ss = ((ITakesScreenshot)Driver).GetScreenshot().AsByteArray;
                OpenCvSharp.Point2f[] pts = WRP.Detect(ss);

                while (pts.Length == 0)
                {
                    await Task.Delay(50);
                    ss = ((ITakesScreenshot)Driver).GetScreenshot().AsByteArray;
                    await Task.Delay(50);
                    pts = WRP.Detect(ss);
                }

                vk.Style = (Style)win.Resources["VKupd"];
                vk.Padding = new Thickness(10);
                vk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffffff"));
                System.Drawing.Bitmap bit = WRP.QrFounder(ss);
                bit.MakeTransparent(System.Drawing.Color.White);
                vk.Content = new Image() { Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bit.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) };
                
                return;
            }
        }
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Cleaner(MainView, 1, 1);
            vk.Style = (Style)win.Resources["VK"];
            vk.Content = new Image() { Source = new BitmapImage(new Uri("uiicons/vkLogM.png", UriKind.Relative)), Name = "vkImg" };
            vk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#26ffffff"));
            MainView.Children.Add(vk);
        }
    }
}