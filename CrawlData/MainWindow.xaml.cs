using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrawlData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<MenuTreeItem> TreeItems;
        HttpClient httpClient;
        HttpClientHandler handler;
        CookieContainer cookie = new CookieContainer();
        public MainWindow()
        {
            InitializeComponent();
            InitHttpClient();
            TreeItems = new ObservableCollection<MenuTreeItem>();
        }
        void InitHttpClient()
        {
            handler = new HttpClientHandler
            {
                CookieContainer = cookie,
                ClientCertificateOptions = ClientCertificateOption.Automatic,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseDefaultCredentials = false
            };
            httpClient = new HttpClient(handler);
        }
        void AddItemIntoTreeViewItem(ObservableCollection<MenuTreeItem> root, MenuTreeItem node)
        {
            root.Add(node);
        }
        string CrawlDataFromURL(string url)
        {
            string html = "";
            html = httpClient.GetStringAsync(url).Result;
            return html;
        }
        void Crawl(string url)
        {
            string htmlLearn = CrawlDataFromURL(url);
            var CourseList = Regex.Matches(htmlLearn, @"<div class=""info-course(.*?)</div>", RegexOptions.Singleline);
            foreach (var course in CourseList)
            {
                string courseName = Regex.Match(course.ToString(), @"(?=<h5>).*?(?=</h5>)").Value.Replace("<h5>", "");
                string linkCourse = Regex.Match(course.ToString(), @"'(.*?)'", RegexOptions.Singleline).Value.Replace("'", "");

                MenuTreeItem item = new MenuTreeItem();
                item.Name = courseName;
                item.URL = linkCourse;

                AddItemIntoTreeViewItem(TreeItems, item);
            }
            treeMain.ItemsSource = TreeItems;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            wbMain.Text = CrawlDataFromURL("https://howkteam.vn/learn");
        }
    }
}
