using Bogus.DataSets;
using Newtonsoft.Json;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Services.Interfaces;
using System.Net.Http;
using System.Net.Mime;
using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace StudentPortal.Services.Implementations;

public class LibraryService : ILibraryService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(HttpClient httpClient, ILogger<LibraryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CreateAccount(AccountDto request)
    {

        string accountUrl = $"api/register";

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
}