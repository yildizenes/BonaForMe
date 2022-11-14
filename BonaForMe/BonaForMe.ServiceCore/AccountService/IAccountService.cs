using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;

namespace BonaForMe.ServiceCore.AccountService
{
    public interface IAccountService
    {
        Result<UserDto> Login(AccountDto accountDto);
    }
}
