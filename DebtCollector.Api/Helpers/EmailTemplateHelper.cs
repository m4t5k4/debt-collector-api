namespace DebtCollector.Api.Helpers
{
    public static class EmailTemplateHelper
    {
        public static string GetTemplate(string filePath, Dictionary<string, string> placeholders)
        {
            var html = File.ReadAllText(filePath);
            foreach (var placeholder in placeholders)
            {
                html = html.Replace(placeholder.Key, placeholder.Value);
            }
            return html;
        }
    }
}
