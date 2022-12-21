using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonaForMe.DomainCore.DTO
{
    public class ResetPasswordDto
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string VerificationNewPassword { get; set; }

    }
}
