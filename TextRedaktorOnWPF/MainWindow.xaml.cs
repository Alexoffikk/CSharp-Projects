using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;


namespace TextRedaktorOnWPF
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private const string LanguageToolUrl = "https://api.languagetool.org/v2/check";
        private const string ApiResponseLogFile = "api_response_log.txt";
        private const string FormattedResponseLogFile = "formatted_response_log.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnFixButtonClick(object sender, RoutedEventArgs e)
        {
            string text = InputTextBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                OutputTextBlock.Text = "Введите текст для проверки.";
                return;
            }

            // Цензурируем текст
            string censoredText = CensorProfanity(text);

            try
            {
                var response = await CheckTextAsync(censoredText);
                var (correctedText, formattedResponse) = FormatResponse(response, censoredText);

                // Цензурируем исправленный текст
                string finalCorrectedText = CensorProfanity(correctedText);

                InputTextBox.Text = finalCorrectedText; // Обновляем текст с исправлениями
                OutputTextBlock.Text = formattedResponse; // Печатаем исправленные слова и предложения во втором текстовом блоке
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = $"Ошибка: {ex.Message}";
            }
        }

        private async Task<JObject> CheckTextAsync(string text)
        {
            var content = new StringContent(
                $"text={Uri.EscapeDataString(text)}&language=ru",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            HttpResponseMessage response = await client.PostAsync(LanguageToolUrl, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            // Запись ответа от API в лог-файл
            File.AppendAllText(ApiResponseLogFile, "API Response:\n" + responseBody + "\n");

            return JObject.Parse(responseBody);
        }

        private (string correctedText, string formattedResponse) FormatResponse(JObject response, string text)
        {
            var correctedTextBuilder = new StringBuilder(text);
            var formattedResponseBuilder = new StringBuilder();
            var matches = response["matches"]?.ToObject<JArray>();

            if (matches == null || matches.Count == 0)
            {
                formattedResponseBuilder.AppendLine("Ошибок не найдено.");
                return (text, formattedResponseBuilder.ToString());
            }

            // Сортируем ошибки по убыванию смещения, чтобы не нарушать индексы при заменах
            var sortedMatches = matches.OrderByDescending(m => (int)m["offset"]).ToList();

            foreach (var match in sortedMatches)
            {
                var replacements = match["replacements"]?.ToObject<JArray>();
                var offset = match["offset"]?.ToString();
                var length = match["length"]?.ToString();
                var message = match["message"]?.ToString();

                if (int.TryParse(offset, out int startOffset) && int.TryParse(length, out int matchLength))
                {
                    if (replacements != null && replacements.Count > 0)
                    {
                        // Возьмем первое предложение исправления
                        var replacement = replacements.FirstOrDefault();
                        var replacementValue = replacement["value"]?.ToString();
                        var incorrectText = text.Substring(startOffset, matchLength);

                        if (!string.IsNullOrEmpty(replacementValue))
                        {
                            // Формируем исправленный текст
                            correctedTextBuilder.Remove(startOffset, matchLength);
                            correctedTextBuilder.Insert(startOffset, replacementValue);

                            // Формируем отформатированный ответ
                            formattedResponseBuilder.AppendLine($"{incorrectText} => {replacementValue}");
                        }
                    }
                    else if (!string.IsNullOrEmpty(message))
                    {
                        // Формируем отформатированный ответ для предложений
                        formattedResponseBuilder.AppendLine($"{message}");
                    }
                }
            }

            return (correctedTextBuilder.ToString(), formattedResponseBuilder.ToString());
        }

        private string CensorProfanity(string text)
        {
            // Пример списка неприличных слов
            var badWords = new[] { "плохоеСлово1", "плохоеСлово2" }; // Замените на реальные слова

            foreach (var badWord in badWords)
            {
                var regex = new System.Text.RegularExpressions.Regex(Regex.Escape(badWord), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                text = regex.Replace(text, new string('*', badWord.Length));
            }

            return text;
        }
    }
}
