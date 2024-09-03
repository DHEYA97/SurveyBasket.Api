namespace SurveyBasket.Api.Helpers
{
    public static class EmailBodyBuilder
    {
        public static string GenerateEmailBody(string template,Dictionary<string,string> templateModel)
        {
            var path = $"{Directory.GetCurrentDirectory()}/Templates/{template}.html";
            var streamReader = new StreamReader(path);
            var body = streamReader.ReadToEnd();
            foreach (var item in templateModel)
                body = body.Replace(item.Key, item.Value);
            
            return body;
        }
    }
}