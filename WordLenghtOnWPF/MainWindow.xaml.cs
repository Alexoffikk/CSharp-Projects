using System;
using System.Windows;

namespace WordLenghtOnWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StrokeCheck.Content = "Сравнить"; // Устанавливаем текст для кнопки
        }

        private void StrokeCheck_Click(object sender, RoutedEventArgs e)
        {
            // Получаем строки из текстовых полей
            string firstStroke = First_Stroke.Text;
            string secondStroke = Second_Stroke.Text;

            // Создаем экземпляры класса RealString
            RealString realString1 = new RealString(firstStroke);
            RealString realString2 = new RealString(secondStroke);

            // Сравниваем длины строк и формируем результат
            string result;
            if (realString1.Length > realString2.Length)
            {
                result = "Первая строка длиннее.";
            }
            else if (realString1.Length < realString2.Length)
            {
                result = "Вторая строка длиннее.";
            }
            else
            {
                result = "Строки равны по длине.";
            }

            // Отображаем результат
            Result_Block.Text = result;
        }
    }

    // Класс RealString
    public class RealString
    {
        public string Value { get; }
        public int Length => Value.Length; // Свойство для получения длины строки

        public RealString(string value)
        {
            Value = value;
        }
    }
}
