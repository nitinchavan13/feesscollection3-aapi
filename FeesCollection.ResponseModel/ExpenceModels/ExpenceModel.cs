using FeesCollection.ResponseModel.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.ExpenceModels
{
    public class ExpenceModel : BaseModel
    {
        public int Id { get; set; }
        public DateTime ExpenceDate { get; set; }
        public decimal ExpenceAmount { get; set; }
        public string ExpenceType { get; set; }
        public string ExpenceNote { get; set; }
    }
}
