using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class SendEmailStatusFunction
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendEmailStatusFunction> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _sendEmailFunctionBaseUrl;
    private readonly string _functionCode;

    public SendEmailStatusFunction(IConfiguration configuration, HttpClient httpClient, ILogger<SendEmailStatusFunction> logger)
    {
        _configuration = configuration;
        _sendEmailFunctionBaseUrl = _configuration!["SendEmailFunctionBaseUrl"]!;
        _functionCode = _configuration["FunctionCode"]!;
        _httpClient = httpClient;
        _logger = logger;
    }

    public virtual async Task SendEmailStatusAsync(int userId, string status, string message)
    {
        try
        {
            var url = $"{_sendEmailFunctionBaseUrl}/{userId}?code={_functionCode}";
            var payload = new
            {
                UserId = userId,
                Status = status,
                Message = message
            };

            var json = JsonSerializer.Serialize(payload);

            _logger.LogInformation($"Sending email status to URL: {url} with payload: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                _logger.LogError(response.ToString());

            _logger.LogInformation(response.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email status");
        }
    }
}