using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.AuthModels;
using System;
using System.Configuration;

namespace FeesCollection.BusinessLayer.AuthService
{
    public interface IAuthService
    {
        UserAuthModel Login(AuthModel model);
        StudentAuthResponseModel StudentLogin(StudentAuthModel model);
    }

    public class AuthService : IAuthService
    {
        #region Constructor
        DBHelper _dBHelper;
        public AuthService()
        {
            _dBHelper = new DBHelper();
            _dBHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }
        #endregion

        #region Methods
        public UserAuthModel Login(AuthModel model)
        {
            UserAuthModel userInfo = new UserAuthModel();
            _dBHelper.AddParameter("p_username", model.MobileNumber);
            _dBHelper.AddParameter("p_password", model.Password);
            _dBHelper.AddParameter("p_academicYearId", model.AcademicYearId);
            var reader = _dBHelper.ExecuteReader("sp_admin_login", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader["isactive"]) == true)
                        {
                            userInfo.Id = Convert.ToInt32(reader["id"]);
                            userInfo.UserName = reader["username"].ToString();
                            userInfo.AcademicYearId = Convert.ToInt32(reader["academicYearId"]);
                        }
                        else
                        {
                            throw new Exception("Sorry, your account is inactive. Please contact administrator");
                        }

                    }
                    return userInfo;
                }
                else
                {
                    throw new Exception("User name or password is miss matched.");
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

        public StudentAuthResponseModel StudentLogin(StudentAuthModel model)
        {
            StudentAuthResponseModel userInfo = new StudentAuthResponseModel();
            _dBHelper.AddParameter("p_username", model.UserEmail);
            _dBHelper.AddParameter("p_password", model.Password);
            var reader = _dBHelper.ExecuteReader("sp_student_login", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader["isactive"]) == true)
                        {
                            userInfo.Id = Convert.ToInt32(reader["id"]);
                            userInfo.Name = reader["name"].ToString();
                            userInfo.Email = reader["emailid"].ToString();
                            userInfo.AcademicYearId = Convert.ToInt32(reader["academicyearid"]);
                        }
                        else
                        {
                            throw new Exception("Sorry, your account is inactive. Please contact administrator");
                        }

                    }
                    return userInfo;
                }
                else
                {
                    throw new Exception("User name or password is miss matched.");
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
        #endregion
    }
}