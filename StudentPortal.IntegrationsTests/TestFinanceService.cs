using StudentPortal.Models.Dtos;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.IntegrationsTests;

public class TestFinanceService : IFinanceService
{
    public async Task<bool> CreateAccount(AccountDto request)
    {
        return true;
    }

    public async Task<string> GenerateInvoice(GenerateInvoiceDto request)
    {
        return "#1234";
    }

    public async Task<FinanceAccountDto> GetAccountOutstanding(string studentId)
    {
        throw new NotImplementedException();
    }
}