using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.StudentModels;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FeesCollection.ResponseModel.Utility;
using System.Configuration;

namespace FeesCollection.BusinessLayer.StudentService
{
    public interface IStudentService
    {
        Task<List<StudentModel>> GetStudents(int academicYearId);
        Task<StudentDetailsModel> GetStudentDetails(int id);
        Task<List<StudentModel>> AddStudent(StudentModel studentModel);
        Task<List<StudentModel>> EditStudent(StudentModel studentModel);
        Task<StudentDetailsModel> AddStudentFees(int studentId, StudentFeeModel model);
        Task<StudentDetailsModel> EditStudentFees(int studentId, StudentFeeModel model);
        Task<List<StudentModel>> DeleteStudent(int id, int academicYearId);
    }

    public class StudentService : IStudentService
    {
        #region Constructor
        DBHelper _dBHelper;
        public StudentService()
        {
            _dBHelper = new DBHelper(ConfigurationManager.AppSettings["mysqlConnectionString"]);
        }
        #endregion

        #region Methods
        public async Task<List<StudentModel>> AddStudent(StudentModel studentModel)
        {
            List<StudentModel> students = new List<StudentModel>();
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_firstname", studentModel.FirstName),
                    new MySqlParameter("@p_lastname", studentModel.LastName),
                    new MySqlParameter("@p_middlename", studentModel.MiddleName),
                    new MySqlParameter("@p_mobilenumber", studentModel.MobileNumber),
                    new MySqlParameter("@p_emailid", studentModel.EmailId),
                    new MySqlParameter("@p_address", studentModel.Address),
                    new MySqlParameter("@p_academicid", studentModel.AcademicYearId),
                    new MySqlParameter("@p_race", studentModel.Race),
                    new MySqlParameter("@p_cast", studentModel.Cast),
                    new MySqlParameter("@p_gender", studentModel.Gender),
                    new MySqlParameter("@p_birthdate", studentModel.Birthdate),
                    new MySqlParameter("@p_qualification", studentModel.Qualification),
                    new MySqlParameter("@p_aadharno", studentModel.AadharNumber),
                    new MySqlParameter("@p_panno", studentModel.PanNumber),
                    new MySqlParameter("@p_heavylicence", studentModel.IsHavingHeavyLicence),
                    new MySqlParameter("@p_courseid", studentModel.CourseId),
                    new MySqlParameter("@p_profilepic", studentModel.ProfilePic)
                };
                // Insert student
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_student_add", parameters);
                //SendSMS.SMSSend("", studentModel.MobileNumber);
                
                // Fetch updated students list
                students = await GetStudents(studentModel.AcademicYearId);
                return students;
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }


        public async Task<List<StudentModel>> GetStudents(int academicYearId)
        {
            List<StudentModel> students = new List<StudentModel>();
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_academicid", academicYearId)
                };
                DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_student_get", parameters);
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        students.Add(new StudentModel()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            FirstName = row["firstname"].ToString(),
                            LastName = row["lastname"].ToString(),
                            MiddleName = row["middlename"].ToString(),
                            MobileNumber = row["mobilenumber"].ToString(),
                            EmailId = row["emailid"].ToString(),
                            Address = row["address"].ToString(),
                            CourseName = row["coursename"].ToString(),
                            TotalPaidFees = Convert.ToDecimal(row["paidFees"]),
                            Race = row["race"].ToString(),
                            Cast = row["cast"].ToString(),
                            Gender = row["gender"].ToString(),
                            Birthdate = row["birthdate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["birthdate"]),
                            Qualification = row["qualification"].ToString(),
                            AadharNumber = row["aadharnumber"].ToString(),
                            PanNumber = row["pannumber"].ToString(),
                            IsHavingHeavyLicence = row["heavylicence"] == DBNull.Value ? false : Convert.ToBoolean(row["heavylicence"]),
                            ProfilePic = row["profilepic"] == DBNull.Value ? null : row["profilepic"].ToString()
                        });
                    }
                }
                return students.OrderBy(x => x.FirstName).ToList();
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<StudentDetailsModel> GetStudentDetails(int id)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_id", id)
                };
                StudentDetailsModel details = new StudentDetailsModel
                {
                    StudentInfo = await GetStudentInfo(id),
                    // Fetch fees details
                    StudentFees = await GetStudentFeesDetails(id)
                };
                return details;
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        private async Task<StudentModel> GetStudentInfo(int studentId)
        {
            StudentModel student = new StudentModel();
            MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_id", studentId)
                };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_student_details", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    var row = result.Rows[0];
                    student.Id = Convert.ToInt32(row["id"]);
                    student.FirstName = row["firstname"].ToString();
                    student.LastName = row["lastname"].ToString();
                    student.MiddleName = row["middlename"].ToString();
                    student.MobileNumber = row["mobilenumber"].ToString();
                    student.EmailId = row["emailid"].ToString();
                    student.Address = row["address"].ToString();
                    student.CourseName = row["coursename"].ToString();
                    return student;
                }
                return null;
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        private async Task<List<StudentFeeModel>> GetStudentFeesDetails(int studentId)
        {
            List<StudentFeeModel> studentFees = new List<StudentFeeModel>();
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_studentid", studentId)
                };
                DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_student_fees_get", parameters);
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        studentFees.Add(new StudentFeeModel()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            PaidAmount = Convert.ToDecimal(row["feesamount"]),
                            CollectionDate = Convert.ToDateTime(row["collectiondate"]),
                            Note = row["feesnote"].ToString()
                        });
                    }
                }
                return studentFees.OrderByDescending(x => x.CollectionDate).ToList();
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<List<StudentModel>> EditStudent(StudentModel studentModel)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_firstname", studentModel.FirstName),
                    new MySqlParameter("@p_lastname", studentModel.LastName),
                    new MySqlParameter("@p_middlename", studentModel.MiddleName),
                    new MySqlParameter("@p_mobilenumber", studentModel.MobileNumber),
                    new MySqlParameter("@p_emailid", studentModel.EmailId),
                    new MySqlParameter("@p_address", studentModel.Address),
                    new MySqlParameter("@p_race", studentModel.Race),
                    new MySqlParameter("@p_cast", studentModel.Cast),
                    new MySqlParameter("@p_gender", studentModel.Gender),
                    new MySqlParameter("@p_birthdate", studentModel.Birthdate),
                    new MySqlParameter("@p_qualification", studentModel.Qualification),
                    new MySqlParameter("@p_aadharno", studentModel.AadharNumber),
                    new MySqlParameter("@p_panno", studentModel.PanNumber),
                    new MySqlParameter("@p_heavylicence", studentModel.IsHavingHeavyLicence),
                    new MySqlParameter("@p_id", studentModel.Id)
                };
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_student_update", parameters);
                return await GetStudents(studentModel.AcademicYearId);
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<StudentDetailsModel> AddStudentFees(int studentId, StudentFeeModel model)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_studentId", studentId),
                    new MySqlParameter("@p_feesAmount", model.PaidAmount),
                    new MySqlParameter("@p_collectionDate", model.CollectionDate),
                    new MySqlParameter("@p_feesNote", model.Note)
                };
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_student_fees_add", parameters);
                return await GetStudentDetails(studentId);
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<StudentDetailsModel> EditStudentFees(int studentId, StudentFeeModel model)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_studentId", studentId),
                    new MySqlParameter("@p_feesAmount", model.PaidAmount),
                    new MySqlParameter("@p_collectionDate", model.CollectionDate),
                    new MySqlParameter("@p_feesNote", model.Note),
                    new MySqlParameter("@p_feeStatus", true),
                    new MySqlParameter("@p_id", model.Id)
                };
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_student_fees_update", parameters);
                return await GetStudentDetails(studentId);
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<List<StudentModel>> DeleteStudent(int id, int academicYearId)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_id", id)
                };
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_student_delete", parameters);
                return await GetStudents(academicYearId);
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }
        #endregion
    }
}
