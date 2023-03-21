using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace my_app_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public IWebDriver Driver_gen()
        {

            ChromeOptions options = new ChromeOptions();
            var chrome_service = ChromeDriverService.CreateDefaultService();
            chrome_service.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(options: options, service: chrome_service);

            return driver;
        }

        public MainWindow()
        {
            IWebDriver driver = Driver_gen();

            Method(driver);
            InitializeComponent();
        }

        public string Find_name_of_dialog(IWebElement element)
        {
            string out_txt = "";

            element = element.FindElement(By.CssSelector("div[class*='nim-dialog--content']"));
            element = element.FindElement(By.CssSelector("div[class*='nim-dialog--cw']"));
            element = element.FindElement(By.CssSelector("div[class*='_im_dialog_title']"));
            element = element.FindElement(By.CssSelector("div[class*='nim-dialog--name']"));
            element = element.FindElement(By.TagName("span"));
            element = element.FindElement(By.CssSelector("span[class*='_im_dialog_link']"));

            out_txt += element.Text;

            return out_txt;
        }

        public void Add_border(int i, int j, (int, int, int, int) rounder)
        {
            Border new_border = new Border();
            new_border.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            new_border.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            new_border.Width = 10;
            new_border.Height = 10;
            new_border.Margin = new System.Windows.Thickness(i * 10 + 30, j * 10 + 30, 0, 0);
            new_border.CornerRadius = new System.Windows.CornerRadius(rounder.Item1, rounder.Item2, rounder.Item3, rounder.Item4);
            new_border.Background = System.Windows.Media.Brushes.Black;

            qr.Children.Add(new_border);
        }

        public static List<double> Transform_parse(string input_str)
        {
            List<double> doubles = new List<double>();
            List<double> current = new List<double>();
            for (int i = 1; i < input_str.Length; i++)
            {
                if (char.IsLetter(input_str[i]) == false && char.IsDigit(input_str[i]))
                {
                    current.Add(char.GetNumericValue(input_str[i]));
                }
                else if (char.IsDigit(input_str[i - 1]))
                {
                    doubles.Add(Convert.ToDouble(string.Join("", current)));
                    current.Clear();
                }
            }

            if (current.Count != 0)
            {
                doubles.Add(Convert.ToDouble(string.Join("", current)));
                current.Clear();
            }

            return doubles;
        }

        public static bool[,] Rectangle(bool[,] matrix, int start_x, int start_y, int len, bool to)
        {
            for (int i = start_x; i < start_x + len; i++)
                for (int j = start_y; j < start_y + len; j++)
                {
                    matrix[i, j] = to;
                }

            return matrix;
        }
        public static bool[,] Qr_Rectangles()
        {
            bool[,] matrix = new bool[25, 25];

            matrix = Rectangle(matrix: matrix, start_x: 0, start_y: 0, len: 7, to: true);
            matrix = Rectangle(matrix: matrix, start_x: 1, start_y: 1, len: 5, to: false);
            matrix = Rectangle(matrix: matrix, start_x: 2, start_y: 2, len: 3, to: true);

            matrix = Rectangle(matrix: matrix, start_x: matrix.GetLength(0) - 7, start_y: 0, len: 7, to: true);
            matrix = Rectangle(matrix: matrix, start_x: matrix.GetLength(0) - 6, start_y: 1, len: 5, to: false);
            matrix = Rectangle(matrix: matrix, start_x: matrix.GetLength(0) - 5, start_y: 2, len: 3, to: true);

            matrix = Rectangle(matrix: matrix, start_x: 0, start_y: matrix.GetLength(0) - 7, len: 7, to: true);
            matrix = Rectangle(matrix: matrix, start_x: 1, start_y: matrix.GetLength(0) - 6, len: 5, to: false);
            matrix = Rectangle(matrix: matrix, start_x: 2, start_y: matrix.GetLength(0) - 5, len: 3, to: true);

            return matrix;
        }

        async public void Qr_code_out(bool[,] input_matrix)
        {
            bool[,] matrix = new bool[input_matrix.GetLength(0) + 2, input_matrix.GetLength(1) + 2];

            for (int i = 0; i <  input_matrix.GetLength(0); i++)
                for (int j = 0; j <  input_matrix.GetLength(1); j++)
                    matrix[i + 1, j + 1] = input_matrix[i, j];

            Border brd_of_qr = new Border();
            brd_of_qr.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            brd_of_qr.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            brd_of_qr.Margin = new System.Windows.Thickness(20);
            brd_of_qr.Width = 270;
            brd_of_qr.Height = 270;
            brd_of_qr.CornerRadius = new System.Windows.CornerRadius(10);
            brd_of_qr.Background = System.Windows.Media.Brushes.White;

            qr.Children.Add(brd_of_qr);
            int rnd_num = 3;

            for (int i = 1; i < matrix.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < matrix.GetLength(1) - 1; j++)
                {
                    await Task.Delay(10);
                    if (matrix[i, j])
                    {
                        (int, int, int, int) tuple_rnd = (rnd_num, rnd_num, rnd_num, rnd_num);

                        if (matrix[i, j - 1] || matrix[i - 1, j])   // Left n Top
                            tuple_rnd.Item1 = 0;
                        
                        if (matrix[i - 1, j] || matrix[i, j + 1])   // Top n right
                            tuple_rnd.Item4 = 0;
                        
                        if (matrix[i, j + 1] || matrix[i + 1, j])   // Right n Bottom
                            tuple_rnd.Item3 = 0;
                        
                        if (matrix[i + 1, j] || matrix[i, j - 1])   //Left n Bottom
                            tuple_rnd.Item2 = 0;

                        Add_border(i - 1, j - 1, tuple_rnd);
                    }
                }
            }
        }

        async public void Method(IWebDriver driver)
        {
            int max_len_of_dialogs = 10;
            await Task.Delay(50);
            try
            {
                driver.Navigate().GoToUrl("https://vk.com/im");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                await Task.Delay(50);
                win.Visibility = System.Windows.Visibility.Visible;
                string current_url = driver.Url.ToString();
                var el = driver.FindElement(By.CssSelector("button[class*='FlatButton FlatButton--accent-outline FlatButton--size-l FlatButton--wide VkIdForm__button VkIdForm__signInQRButton']"));

                if (el != null)
                {
                    el.Click();

                    qr.Visibility = System.Windows.Visibility.Visible;

                    IWebElement g = wait.Until(e => e.FindElement(By.CssSelector("g[transform*='translate(0,0)']")));
                    var inside_g = g.FindElements(By.TagName("g"));
                    bool[,] matrix_g = Qr_Rectangles();
                    string url = driver.Url.ToString();
                    for (int i = 0; i < inside_g.Count; i++)
                    {
                        if (inside_g[i].FindElement(By.TagName("use")).GetAttribute("xlink:href") != "#n_rb-0")
                        {
                            List<double> dbl_lst = Transform_parse(inside_g[i].GetAttribute("transform"));

                            int x = Convert.ToInt32(dbl_lst[0] / 97);
                            int y = Convert.ToInt32(dbl_lst[1] / 97);

                            matrix_g[x, y] = true;
                        }
                    }

                    Qr_code_out(matrix_g);

                    while (driver.Url.ToString() == url)
                    {
                        await Task.Delay(10);
                    }
                    qr.Visibility = System.Windows.Visibility.Hidden;
                    win.UpdateLayout();
                    Thread.Sleep(100);
                }

                while (true)
                {
                    while (driver.Url == "https://vk.com/im" || driver.Url == "https://vk.com/al_im.php")
                    {
                        string out_txt = "";
                        List<string> list_of_string = new List<string>();
                        var dialog_list = driver.FindElements(By.CssSelector("ul[id*='im_dialogs'] li"));
                        for (int num_of_dialog = 0; num_of_dialog < max_len_of_dialogs && num_of_dialog < dialog_list.Count; num_of_dialog++)
                        {
                            list_of_string.Add(Find_name_of_dialog(dialog_list[num_of_dialog]));
                            await Task.Delay(100);
                        }

                        out_txt = string.Join("\n", list_of_string);

                        Lb1.Content = out_txt;

                        await Task.Delay(500);
                    }
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                for (int i = 0; i < qr.Children.Count; i++)
                    qr.Children.Clear();
                Lb1.Content = ex.Message;
                await Task.Delay(10000);
                driver.Quit();
                this.Close();
                throw new Exception();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();

            if (e.RightButton == MouseButtonState.Pressed)
            {
                this.Close();
                throw new Exception();
            }
        }
    }
}