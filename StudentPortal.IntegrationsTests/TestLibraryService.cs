using StudentPortal.Models.Dtos;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.IntegrationsTests;

public class TestLibraryService : ILibraryService
{
    public async Task<bool> CreateAccount(AccountDto request)
    {
        return true;
    }
}