using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.CourseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.StudentModels
{
    public class StudentModel : BaseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Address { get; set; }
        public int courseId { get; set; }
        public string CourseName { get; set; }
        public decimal TotalPaidFees { get; set; }
        public string Race { get; set; }
        public string Cast { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Qualification { get; set; }
        public string AadharNumber { get; set; }
        public string PanNumber { get; set; }
        public bool IsHavingHeavyLicence { get; set; }

        public string ProfilePic { get; set; }
    }

    public class StudentDetailsModel
    {
        public StudentModel StudentInfo { get; set; }
        public List<StudentFeeModel> StudentFees { get; set; }
    }
}
