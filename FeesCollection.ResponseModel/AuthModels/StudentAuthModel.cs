using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.AuthModels
{
    public class StudentAuthModel
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }

    public class StudentAuthResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int AcademicYearId { get; set; }
    }
}
