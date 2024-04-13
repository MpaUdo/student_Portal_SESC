using StudentPortal.Models.Dtos.Requests;

namespace StudentPortal.Services.Interfaces;

public interface ILibraryService
{
    Task<bool> CreateAccount(AccountDto request);
}