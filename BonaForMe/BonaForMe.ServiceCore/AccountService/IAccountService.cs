using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;

namespace BonaForMe.ServiceCore.AccountService
{
    public interface IAccountService
    {
        Result<UserDto> Login(AccountDto accountDto);
        Result<ResetPasswordDto> ResetPassword(ResetPasswordDto resetPasswordDto);
        Result<ResetPasswordDto> ResetPasswordByEmail(ResetPasswordDto resetPasswordDto);
    }
}
