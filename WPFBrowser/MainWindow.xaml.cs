using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace WPFBrowser
{
    public partial class MainWindow : Window
    {
        // URL стартовой страницы
        private readonly string StartPageUrl = "file:///" + Path.GetFullPath("HomePage/index.html");

        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                await MyWebView.EnsureCoreWebView2Async(null);
                MyWebView.CoreWebView2.Navigate(StartPageUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации WebView2: {ex.Message}");
            }
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrlOrSearch();
        }

        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            MyWebView.CoreWebView2.Navigate(StartPageUrl);
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (MyWebView.CoreWebView2.CanGoBack)
            {
                MyWebView.CoreWebView2.GoBack();
            }
        }

        private void UpdatePage_Click(object sender, RoutedEventArgs e)
        {
            MyWebView.CoreWebView2.Reload();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (MyWebView.CoreWebView2.CanGoForward)
            {
                MyWebView.CoreWebView2.GoForward();
            }
        }

        private void NavigateToUrlOrSearch()
        {
            string input = AddressBar.Text.Trim();

            if (string.IsNullOrEmpty(input))
                return;

            if (IsUrl(input))
            {
                MyWebView.CoreWebView2.Navigate(input);
            }
            else
            {
                // Если ввод не является URL, выполняем поиск в Google
                string searchQuery = Uri.EscapeDataString(input);
                string googleSearchUrl = $"https://www.google.com/search?q={searchQuery}";
                MyWebView.CoreWebView2.Navigate(googleSearchUrl);
            }
        }

        private bool IsUrl(string input)
        {
            return input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   input.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrlOrSearch();
            }
        }
    }
}
