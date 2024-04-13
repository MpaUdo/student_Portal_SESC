using StudentPortal.Models.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentPortal.Models.Dtos;

namespace StudentPortal.Services.Interfaces
{
    public interface IFinanceService
    {
        Task<bool> CreateAccount(AccountDto request);
        Task<string> GenerateInvoice(GenerateInvoiceDto request);
        Task<FinanceAccountDto> GetAccountOutstanding(string studentId);

    }
}
