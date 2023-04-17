using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonaForMe.DomainCore.DTO
{
    public class CheckStockDTO
    {

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int OrderCount { get; set; }
    }
}
