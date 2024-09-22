using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CalculatorOnWPF
{
    public partial class MainWindow : Window
    {
        private string currentInput = "";   // Текущее введённое число или выражение
        private string operation = "";      // Текущая операция
        private double firstNumber = 0;     // Первое число в операции
        private bool isFirstInput = true;   // Флаг для отслеживания первого ввода
        private bool isMore = false;        // Флаг для переключения дополнительных функций

        private DispatcherTimer timer;       // Таймер для управления видимостью

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.25); // Задержка 0.25 секунды
            timer.Tick += Timer_Tick;
        }

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Если это первое нажатие, очищаем текстовое поле
                if (isFirstInput)
                {
                    ResultTextBox.Text = "";
                    isFirstInput = false;
                }

                // Добавляем число или десятичную точку
                string value = button.Content.ToString();
                if (value == "." && currentInput.Contains("."))
                {
                    return; // Не добавляем вторую десятичную точку
                }

                currentInput += value;
                ResultTextBox.Text = currentInput;
            }
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (string.IsNullOrEmpty(currentInput) && button.Content.ToString() == "-")
                {
                    currentInput = "-";
                    ResultTextBox.Text = currentInput;
                    return;
                }

                if (double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out firstNumber))
                {
                    operation = button.Content.ToString();
                    currentInput = "";
                    ResultTextBox.Text = operation;
                }
            }
        }

        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double secondNumber))
            {
                double result = operation switch
                {
                    "+" => firstNumber + secondNumber,
                    "-" => firstNumber - secondNumber,
                    "*" => firstNumber * secondNumber,
                    "/" when secondNumber != 0 => firstNumber / secondNumber,
                    _ => double.NaN
                };

                if (double.IsNaN(result))
                {
                    MessageBox.Show("Деление на ноль невозможно");
                }
                else
                {
                    ResultTextBox.Text = result.ToString(CultureInfo.InvariantCulture);
                    currentInput = result.ToString(CultureInfo.InvariantCulture);
                    isFirstInput = true;
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            currentInput = "";
            operation = "";
            firstNumber = 0;
            isFirstInput = true;
            ResultTextBox.Text = "0";
        }

        private void ClearLast_Click(object sender, RoutedEventArgs e)
        {
            if (currentInput.Length > 0)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                ResultTextBox.Text = currentInput.Length > 0 ? currentInput : "0";
            }
        }

        private void MoreFunc_Click(object sender, RoutedEventArgs e)
        {
            // Отключаем кнопку
            var button = sender as Button;
            button.IsEnabled = false;

            // Запускаем таймер перед изменением видимости
            if (!timer.IsEnabled)
            {
                timer.Start();
            }

            // Создаем анимацию для ширины окна
            DoubleAnimation windowAnimation = new DoubleAnimation
            {
                From = this.Width,
                To = isMore ? this.Width - 100 : this.Width + 100, // Увеличение или уменьшение ширины
                Duration = new Duration(TimeSpan.FromSeconds(0.25)) // Длительность анимации
            };

            // Запускаем анимацию для ширины окна
            this.BeginAnimation(WidthProperty, windowAnimation);

            // Запускаем таймер для повторного включения кнопки
            DispatcherTimer buttonTimer = new DispatcherTimer();
            buttonTimer.Interval = TimeSpan.FromMilliseconds(300);
            buttonTimer.Tick += (s, args) =>
            {
                button.IsEnabled = true;
                buttonTimer.Stop();
            };
            buttonTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop(); // Остановить таймер

            // Изменяем видимость TrigFunctions с учетом isMore
            TrigFunctions.Visibility = isMore ? Visibility.Collapsed : Visibility.Visible;

            // Переключаем состояние isMore
            isMore = !isMore;
        }

        // Обработчик для тригонометрических функций
        private void TrigFunction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double angle))
                {
                    double result = button.Content switch
                    {
                        "sin" => Math.Sin(angle),
                        "cos" => Math.Cos(angle),
                        "tan" => Math.Tan(angle),
                        "cot" => 1 / Math.Tan(angle),
                        _ => 0
                    };
                    ResultTextBox.Text = result.ToString(CultureInfo.InvariantCulture);
                    currentInput = result.ToString(CultureInfo.InvariantCulture);
                    isFirstInput = true;
                }
            }
        }

        // Обработчик для логарифмов
        private void LogFunction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
                {
                    double result = button.Content switch
                    {
                        "log" => Math.Log10(number),
                        "ln" => Math.Log(number),
                        _ => 0
                    };
                    ResultTextBox.Text = result.ToString(CultureInfo.InvariantCulture);
                    currentInput = result.ToString(CultureInfo.InvariantCulture);
                    isFirstInput = true;
                }
            }
        }

        // Обработчик для логарифма с произвольным основанием
        private void LogBaseFunction_Click(object sender, RoutedEventArgs e)
        {
            string input = currentInput;
            string[] parts = input.Split(',');

            if (parts.Length == 2 && double.TryParse(parts[0], out double a) && double.TryParse(parts[1], out double b))
            {
                double result = Math.Log(b) / Math.Log(a);
                ResultTextBox.Text = result.ToString(CultureInfo.InvariantCulture);
                currentInput = result.ToString(CultureInfo.InvariantCulture);
                isFirstInput = true;
            }
            else
            {
                MessageBox.Show("Введите корректный формат: a,b");
            }
        }

        // Обработчик для антиклогарифма
        private void AntiLogFunction_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double input))
            {
                double result = Math.Pow(10, input);
                ResultTextBox.Text = result.ToString(CultureInfo.InvariantCulture);
                currentInput = result.ToString(CultureInfo.InvariantCulture);
                isFirstInput = true;
            }
        }

        // Обработчик для дополнительных функций
        private void AdditionalFunction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
                {
                    double result = button.Content switch
                    {
                        "√" => Math.Sqrt(number),
                        "exp" => Math.Exp(number),
                        "^" => 0, // Здесь можно добавить логику для возведения в степень
                        _ => 0
                    };
                    ResultTextBox.Text = result.ToString(CultureInfo.InvariantCulture);
                    currentInput = result.ToString(CultureInfo.InvariantCulture);
                    isFirstInput = true;
                }
            }
        }
    }
}