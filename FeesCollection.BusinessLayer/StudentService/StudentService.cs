using FeesCollection.BusinessLayer.Utility;
using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.StudentModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FeesCollection.BusinessLayer.StudentService
{
    public interface IStudentService
    {
        List<StudentModel> GetStudents(int academicYearId);
        StudentDetailsModel GetStudentDetails(int id);
        List<StudentModel> AddStudent(StudentModel studentModel);
        List<StudentModel> EditStudent(StudentModel studentModel);
        StudentDetailsModel AddStudentFees(int studentId, StudentFeeModel model);
        StudentDetailsModel EditStudentFees(int studentId, StudentFeeModel model);
        List<StudentModel> DeleteStudent(int id, int academicYearId);
    }

    public class StudentService : IStudentService
    {
        #region Constructor
        DBHelper _dBHelper;
        public StudentService()
        {
            _dBHelper = new DBHelper();
            _dBHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }
        #endregion

        #region Methods
        public List<StudentModel> AddStudent(StudentModel studentModel)
        {
            _dBHelper.AddParameter("p_firstname", studentModel.FirstName);
            _dBHelper.AddParameter("p_lastname", studentModel.LastName);
            _dBHelper.AddParameter("p_middlename", studentModel.MiddleName);
            _dBHelper.AddParameter("p_mobilenumber", studentModel.MobileNumber);
            _dBHelper.AddParameter("p_emailid", studentModel.EmailId);
            _dBHelper.AddParameter("p_address", studentModel.Address);
            _dBHelper.AddParameter("p_academicid", studentModel.AcademicYearId);
            _dBHelper.AddParameter("p_race", studentModel.Race);
            _dBHelper.AddParameter("p_cast", studentModel.Cast);
            _dBHelper.AddParameter("p_gender", studentModel.Gender);
            _dBHelper.AddParameter("p_birthdate", studentModel.Birthdate);
            _dBHelper.AddParameter("p_qualification", studentModel.Qualification);
            _dBHelper.AddParameter("p_aadharno", studentModel.AadharNumber);
            _dBHelper.AddParameter("p_panno", studentModel.PanNumber);
            _dBHelper.AddParameter("p_heavylicence", studentModel.IsHavingHeavyLicence);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_student_add", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);

                int newStudentId = this.GetLastInsertedId();

                if(newStudentId != 0)
                {
                    _dBHelper.connection.Close();
                    this.AssignCourse(newStudentId, studentModel.courseId);
                    //SendSMS.SMSSend("", studentModel.MobileNumber);
                }

                return this.GetStudents(studentModel.AcademicYearId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public List<StudentModel> EditStudent(StudentModel studentModel)
        {
            _dBHelper.AddParameter("p_firstname", studentModel.FirstName);
            _dBHelper.AddParameter("p_lastname", studentModel.LastName);
            _dBHelper.AddParameter("p_middlename", studentModel.MiddleName);
            _dBHelper.AddParameter("p_mobilenumber", studentModel.MobileNumber);
            _dBHelper.AddParameter("p_emailid", studentModel.EmailId);
            _dBHelper.AddParameter("p_address", studentModel.Address);
            _dBHelper.AddParameter("p_race", studentModel.Race);
            _dBHelper.AddParameter("p_cast", studentModel.Cast);
            _dBHelper.AddParameter("p_gender", studentModel.Gender);
            _dBHelper.AddParameter("p_birthdate", studentModel.Birthdate);
            _dBHelper.AddParameter("p_qualification", studentModel.Qualification);
            _dBHelper.AddParameter("p_aadharno", studentModel.AadharNumber);
            _dBHelper.AddParameter("p_panno", studentModel.PanNumber);
            _dBHelper.AddParameter("p_heavylicence", studentModel.IsHavingHeavyLicence);
            _dBHelper.AddParameter("p_id", studentModel.Id);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_student_update", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);

                return this.GetStudents(studentModel.AcademicYearId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public StudentDetailsModel GetStudentDetails(int id)
        {
            StudentDetailsModel studentDetails = new StudentDetailsModel();
            studentDetails.StudentInfo = this.GetStudentInfo(id);
            studentDetails.StudentFees = this.GetStudentFeesDetails(id);
            return studentDetails;
        }

        public List<StudentModel> GetStudents(int academicYearId)
        {
            List<StudentModel> students = new List<StudentModel>();
            _dBHelper.AddParameter("p_academicid", academicYearId);
            var reader = _dBHelper.ExecuteReader("sp_student_get", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        students.Add(new StudentModel()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            FirstName = reader["firstname"].ToString(),
                            LastName = reader["lastname"].ToString(),
                            MiddleName = reader["middlename"].ToString(),
                            MobileNumber = reader["mobilenumber"].ToString(),
                            EmailId = reader["emailid"].ToString(),
                            Address = reader["address"].ToString(),
                            CourseName = reader["coursename"].ToString(),
                            TotalPaidFees = Convert.ToDecimal(reader["paidFees"]),
                            Race = reader["race"].ToString(),
                            Cast = reader["cast"].ToString(),
                            Gender = reader["gender"].ToString(),
                            Birthdate = reader["birthdate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["birthdate"]),
                            Qualification = reader["qualification"].ToString(),
                            AadharNumber = reader["aadharnumber"].ToString(),
                            PanNumber = reader["pannumber"].ToString(),
                            IsHavingHeavyLicence = reader["heavylicence"] == DBNull.Value ? false : Convert.ToBoolean(reader["heavylicence"]),
                        });
                    }
                    return students.OrderBy(x => x.FirstName).ToList();
                }
                else
                {
                    return students;
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

        public StudentDetailsModel AddStudentFees(int studentId, StudentFeeModel model)
        {
            _dBHelper.AddParameter("p_studentId", studentId);
            _dBHelper.AddParameter("p_feesAmount", model.PaidAmount);
            _dBHelper.AddParameter("p_collectionDate", model.CollectionDate);
            _dBHelper.AddParameter("p_feesNote", model.Note);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_student_fees_add", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
                return this.GetStudentDetails(studentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public StudentDetailsModel EditStudentFees(int studentId, StudentFeeModel model)
        {
            _dBHelper.AddParameter("p_studentId", studentId);
            _dBHelper.AddParameter("p_feesAmount", model.PaidAmount);
            _dBHelper.AddParameter("p_collectionDate", model.CollectionDate);
            _dBHelper.AddParameter("p_feesNote", model.Note);
            _dBHelper.AddParameter("p_feeStatus", true);
            _dBHelper.AddParameter("p_id", model.Id);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_student_fees_update", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
                return this.GetStudentDetails(studentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<StudentModel> DeleteStudent(int id, int academicYearId)
        {
            _dBHelper.AddParameter("p_id", id);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_student_delete", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);

                return this.GetStudents(academicYearId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Private Methods
        private int GetLastInsertedId()
        {
            var id = 0;
            var reader = _dBHelper.ExecuteReader("sp_student_get_last_inserted_id", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader["id"]);
                    }
                    return id;
                }
                else
                {
                    return id;
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

        private StudentModel GetStudentInfo(int studentId)
        {
            StudentModel student = new StudentModel();
            _dBHelper.AddParameter("p_id", studentId);
            var reader = _dBHelper.ExecuteReader("sp_student_details", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        student.Id = Convert.ToInt32(reader["id"]);
                        student.FirstName = reader["firstname"].ToString();
                        student.LastName = reader["lastname"].ToString();
                        student.MiddleName = reader["middlename"].ToString();
                        student.MobileNumber = reader["mobilenumber"].ToString();
                        student.EmailId = reader["emailid"].ToString();
                        student.Address = reader["address"].ToString();
                        student.CourseName = reader["coursename"].ToString();
                    }
                    return student;
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

        private List<StudentFeeModel> GetStudentFeesDetails(int studentId)
        {
            List<StudentFeeModel> studentFees = new List<StudentFeeModel>();
            _dBHelper.AddParameter("p_studentid", studentId);
            var reader = _dBHelper.ExecuteReader("sp_student_fees_get", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        studentFees.Add(new StudentFeeModel()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            PaidAmount = Convert.ToDecimal(reader["feesamount"]),
                            CollectionDate = Convert.ToDateTime(reader["collectiondate"]),
                            Note = reader["feesnote"].ToString()
                        });
                    }
                    return studentFees.OrderByDescending(x => x.CollectionDate).ToList();
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

        private bool AssignCourse(int studentId, int courseId)
        {
            _dBHelper.AddParameter("p_studentid", studentId);
            _dBHelper.AddParameter("p_courseid", courseId);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_student_course_assign", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
