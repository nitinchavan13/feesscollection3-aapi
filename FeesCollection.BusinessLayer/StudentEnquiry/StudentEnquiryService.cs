using FeesCollection.BusinessLayer.Utility;
using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.ExamModels;
using FeesCollection.ResponseModel.StudentModels;
using FeesCollection.ResponseModel.Utility;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.StudentEnquiry
{
    public interface IStudentEnquiryService
    {
        Task<bool> CreateNewEnquiry(StudentEnquiryModel model);
        Task<List<StudentEnquiryModel>> GetAllEnquiries(BaseModel model);
    }

    public class StudentEnquiryService : IStudentEnquiryService
    {
        #region Constructor
        DBHelper _dBHelper;
        public StudentEnquiryService()
        {
            _dBHelper = new DBHelper(ConfigurationManager.AppSettings["mysqlConnectionString"]);
        }
        #endregion

        public async Task<bool> CreateNewEnquiry(StudentEnquiryModel model)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_firstname", model.FirstName),
                new MySqlParameter("p_lastname", model.LastName),
                new MySqlParameter("p_middlename", model.MiddleName),
                new MySqlParameter("p_mobilenumber", model.MobileNumber),
                new MySqlParameter("p_emailid", model.EmailId),
                new MySqlParameter("p_address", model.Address),
                new MySqlParameter("p_aadharnumber", model.AadharNumber),
                new MySqlParameter("p_tenthmarks", model.TenthMarks),
                new MySqlParameter("p_twelththmarks", model.TwelthMarks),
                new MySqlParameter("p_othereduname", model.OtherEduName),
                new MySqlParameter("p_otheredumarks", model.OtherEduMarks),
                new MySqlParameter("p_enquirydate", TimezoneHelper.ConvertLocalToUTCwithTimeZone(DateTime.UtcNow)),
                new MySqlParameter("p_isvalid", true)
            };
            try
            {
                await _dBHelper.ExecuteNonQueryAsync("sp_student_enquiry_add", parameters);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<StudentEnquiryModel>> GetAllEnquiries(BaseModel model)
        {
            List<StudentEnquiryModel> enquiries = new List<StudentEnquiryModel>();
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_student_enquiry_get");
            try
            {
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        enquiries.Add(new StudentEnquiryModel()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            FirstName = row["firstname"].ToString(),
                            LastName = row["lastname"].ToString(),
                            MiddleName = row["middlename"].ToString(),
                            MobileNumber = row["mobilenumber"].ToString(),
                            EmailId = row["emailid"].ToString(),
                            AadharNumber = row["aadharnumber"].ToString(),
                            Address = row["address"].ToString(),
                            TenthMarks = Convert.ToDecimal(row["tenthmarks"]),
                            TwelthMarks = Convert.ToDecimal(row["twelthmarks"]),
                            OtherEduName = row["tenthmarks"].ToString(),
                            OtherEduMarks = Convert.ToDecimal(row["otheredumarks"]),
                            EnquiryDate = Convert.ToDateTime(row["enquirydate"]),
                            IsValid = Convert.ToBoolean(row["isvalid"])

                        });
                    }
                }
                return enquiries.OrderByDescending(x => x.EnquiryDate).ToList();
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }
    }
}
