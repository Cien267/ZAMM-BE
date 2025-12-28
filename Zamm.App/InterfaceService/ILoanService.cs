using Zamm.Application.Payloads.InputModels.Loan;
using Zamm.Application.Payloads.ResultModels.Loan;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService;

public interface ILoanService
{
    Task<PagedResult<LoanResult>> GetListLoanAsync(LoanQuery query);
    Task<LoanResult> GetLoanByIdAsync(Guid id);
    Task<LoanResult> CreateLoanAsync(CreateLoanInput request);
    Task<LoanResult> UpdateLoanAsync(Guid loanId, UpdateLoanInput request);
    Task DeleteLoanAsync(Guid id);
}