using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.StudentModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.StudentEnquiry
{
    public interface IStudentEnquiryService
    {
        bool CreateNewEnquiry(StudentEnquiryModel model);
        List<StudentEnquiryModel> GetAllEnquiries(BaseModel model);
    }

    public class StudentEnquiryService : IStudentEnquiryService
    {
        #region Constructor
        DBHelper _dBHelper;
        public StudentEnquiryService()
        {
            _dBHelper = new DBHelper();
            _dBHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }
        #endregion

        public bool CreateNewEnquiry(StudentEnquiryModel model)
        {
            _dBHelper.AddParameter("p_firstname", model.FirstName);
            _dBHelper.AddParameter("p_lastname", model.LastName);
            _dBHelper.AddParameter("p_middlename", model.MiddleName);
            _dBHelper.AddParameter("p_mobilenumber", model.MobileNumber);
            _dBHelper.AddParameter("p_emailid", model.EmailId);
            _dBHelper.AddParameter("p_address", model.Address);
            _dBHelper.AddParameter("p_aadharnumber", model.AadharNumber);
            _dBHelper.AddParameter("p_tenthmarks", model.TenthMarks);
            _dBHelper.AddParameter("p_twelththmarks", model.TwelthMarks);
            _dBHelper.AddParameter("p_othereduname", model.OtherEduName);
            _dBHelper.AddParameter("p_otheredumarks", model.OtherEduMarks);
            _dBHelper.AddParameter("p_enquirydate", DateTime.Now);
            _dBHelper.AddParameter("p_isvalid", true);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_student_enquiry_add", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<StudentEnquiryModel> GetAllEnquiries(BaseModel model)
        {
            List<StudentEnquiryModel> enquiries = new List<StudentEnquiryModel>();
            var reader = _dBHelper.ExecuteReader("sp_student_enquiry_get", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        enquiries.Add(new StudentEnquiryModel()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            FirstName = reader["firstname"].ToString(),
                            LastName = reader["lastname"].ToString(),
                            MiddleName = reader["middlename"].ToString(),
                            MobileNumber = reader["mobilenumber"].ToString(),
                            EmailId = reader["emailid"].ToString(),
                            AadharNumber = reader["aadharnumber"].ToString(),
                            Address = reader["address"].ToString(),
                            TenthMarks = Convert.ToDecimal(reader["tenthmarks"]),
                            TwelthMarks = Convert.ToDecimal(reader["twelthmarks"]),
                            OtherEduName = reader["tenthmarks"].ToString(),
                            OtherEduMarks = Convert.ToDecimal(reader["otheredumarks"]),
                            EnquiryDate = Convert.ToDateTime(reader["enquirydate"]),
                            IsValid = Convert.ToBoolean(reader["isvalid"])
                           
                        });
                    }
                    return enquiries.OrderByDescending(x => x.EnquiryDate).ToList();
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                if (_dBHelper.connection.State == System.Data.ConnectionState.Open)
                {
                    _dBHelper.connection.Close();
                }
            }
        }
    }
}
