using System;
using System.Windows;
using System.Windows.Media;

namespace TriangleCheckOnWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TriangleCheck_Click(object sender, RoutedEventArgs e)
        {
            // Считываем значения из TextBox'ов и преобразуем их в числа
            if (double.TryParse(FLine_Width.Text, out double a) &&
                double.TryParse(SLine_Width.Text, out double b) &&
                double.TryParse(TLine_Width.Text, out double c))
            {
                // Проверяем, можно ли из данных отрезков построить треугольник
                if (CanFormTriangle(a, b, c))
                {
                    Result.Text = "Из данных отрезков можно построить треугольник.";
                    Result.Foreground = new SolidColorBrush(Colors.Green); // Зелёный цвет текста для положительного результата
                }
                else
                {
                    Result.Text = "Из данных отрезков нельзя построить треугольник.";
                    Result.Foreground = new SolidColorBrush(Colors.Red); // Красный цвет текста для отрицательного результата
                }
            }
            else
            {
                Result.Text = "Пожалуйста, введите корректные числовые значения.";
                Result.Foreground = new SolidColorBrush(Colors.Orange); // Оранжевый цвет текста для ошибки
            }
        }

        // Метод проверки условий для построения треугольника
        private bool CanFormTriangle(double a, double b, double c)
        {
            return (a + b > c) && (a + c > b) && (b + c > a);
        }
    }
}
