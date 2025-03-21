using FeesCollection.ResponseModel.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.StudentModels
{
    public class StudentFeeModel : BaseModel
    {
        public int Id { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime CollectionDate { get; set; }
        public string Note { get; set; }
    }
}
