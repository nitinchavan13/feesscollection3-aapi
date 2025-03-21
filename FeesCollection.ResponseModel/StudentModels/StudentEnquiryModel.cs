using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.StudentModels
{
    public class StudentEnquiryModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string AadharNumber { get; set; }
        public string Address { get; set; }
        public decimal TenthMarks { get; set; }
        public decimal TwelthMarks { get; set; }
        public string OtherEduName { get; set; }
        public decimal OtherEduMarks { get; set; }
        public DateTime EnquiryDate { get; set; }
        public bool IsValid { get; set; }
    }
}
