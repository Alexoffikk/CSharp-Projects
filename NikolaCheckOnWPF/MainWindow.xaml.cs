using System;
using System.Windows;

namespace NikolaCheckOnWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NikolaCheck_Click(object sender, RoutedEventArgs e)
        {
            // Считываем имя и возраст из полей
            string name = Name_Entry.Text;
            if (int.TryParse(Age_Entry.Text, out int age))
            {
                // Создаем экземпляр класса Nikola
                Nikola nikolaInstance = new Nikola(name, age);

                // Отображаем результат
                Result_Block.Text = $"Ответ: {nikolaInstance.Name}, Возраст: {nikolaInstance.Age}";
            }
            else
            {
                MessageBox.Show("Введите корректное значение возраста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Создаем класс Nikola
    public sealed class Nikola
    {
        public string Name { get; private set; }
        public int Age { get; private set; }

        public Nikola(string name, int age)
        {
            // Преобразуем имя согласно условию
            Name = name == "Николай" ? name : $"Я не {name}, а Николай";
            Age = age;
        }
    }
}
