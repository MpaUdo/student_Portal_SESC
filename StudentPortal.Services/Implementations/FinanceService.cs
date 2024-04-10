using Newtonsoft.Json;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Services.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using StudentPortal.Models.Dtos;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace StudentPortal.Services.Implementations;

public class FinanceService : IFinanceService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FinanceService> _logger;

    public FinanceService(HttpClient httpClient, ILogger<FinanceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CreateAccount(AccountDto request)
    {

        string accountUrl = "/accounts";

        try
        {
            var dataAsString = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            StringContent content = new StringContent(dataAsString, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(accountUrl, content);
            return response.IsSuccessStatusCode;

        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"An error occurred while creating account for studentId:{request.StudentId}\n{ex.Message}");
            return false;
        }
    }

    public async Task<string> GenerateInvoice(GenerateInvoiceDto request)
    {

        string invoiceUrl = "/invoices";

        try
        {
            var dataAsString = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            StringContent content = new StringContent(dataAsString, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(invoiceUrl, content);
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var invoice = JsonConvert.DeserializeObject<Invoice>(message, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                });

                return invoice.Reference;
            }

            throw new InvalidOperationException(message);


        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"An error occurred while creating invoice for studentId:{request.Account.StudentId}\n{ex.Message}");
            throw new InvalidOperationException($"An error occurred while creating invoice for studentId:{ request.Account.StudentId}");
        }
    }

    public async Task<FinanceAccountDto> GetAccountOutstanding(string studentId)
    {
        var url = $"accounts/student/{studentId}";
        var  response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<FinanceAccountDto>(await response.Content.ReadAsStringAsync(), new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
        }
        throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
    }

   
}