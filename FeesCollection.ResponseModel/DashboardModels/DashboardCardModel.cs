using FeesCollection.ResponseModel.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.DashboardModels
{
    public class DashboardCardModel : BaseModel
    {
        public int TotalStudents { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
    }
}
