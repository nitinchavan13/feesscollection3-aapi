using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.AuthModels;
using FeesCollection.ResponseModel.Utility;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.AuthService
{
    public interface IAuthService
    {
        Task<UserAuthModel> Login(AuthModel model);
        Task<StudentAuthResponseModel> StudentLogin(StudentAuthModel model);

        Task<List<AcademicYearModel>> FetchAcademicYears();
    }

    public class AuthService : IAuthService
    {
        #region Constructor
        DBHelper _dBHelper;
        public AuthService()
        {
            _dBHelper = new DBHelper(ConfigurationManager.AppSettings["mysqlConnectionString"]);
        }
		#endregion

		#region Methods
		public async Task<UserAuthModel> Login(AuthModel model)
        {
            UserAuthModel userInfo = new UserAuthModel();
            MySqlParameter[] parameters = new MySqlParameter[] {
                new MySqlParameter("@p_username", model.MobileNumber),
                new MySqlParameter("@p_password", model.Password),
                new MySqlParameter("@p_academicYearId", model.AcademicYearId)
            };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_admin_login", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    var row = result.Rows[0];
                    if (Convert.ToBoolean(row["isactive"]))
                    {
                        userInfo.Id = Convert.ToInt32(row["id"]);
                        userInfo.UserName = row["username"].ToString();
                        userInfo.AcademicYearId = Convert.ToInt32(row["academicYearId"]);

                        return userInfo;
                    }
                    else
                    {
                        throw new Exception("Sorry, your account is inactive. Please contact administrator");
                    }
                }
                else
                {
                    throw new Exception("User name or password is miss matched.");
                }
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<StudentAuthResponseModel> StudentLogin(StudentAuthModel model)
        {
            StudentAuthResponseModel userInfo = new StudentAuthResponseModel();
            MySqlParameter[] parameters = new MySqlParameter[] {
                new MySqlParameter("@p_username", model.UserEmail),
                new MySqlParameter("@p_password", model.Password)
            };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_admin_login", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    var row = result.Rows[0];
                    if (Convert.ToBoolean(row["isactive"]))
                    {
                        userInfo.Id = Convert.ToInt32(row["id"]);
                        userInfo.Name = row["name"].ToString();
                        userInfo.Email = row["emailid"].ToString();
                        userInfo.AcademicYearId = Convert.ToInt32(row["academicyearid"]);

                        return userInfo;
                    }
                    else
                    {
                        throw new Exception("Sorry, your account is inactive. Please contact administrator");
                    }
                }
                else
                {
                    throw new Exception("User name or password is miss matched.");
                }
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<List<AcademicYearModel>> FetchAcademicYears()
        {
            List<AcademicYearModel> academicYearModel = new List<AcademicYearModel>();
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_admin_get_academicyears", parameters: null);
            try
            {
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        academicYearModel.Add(new AcademicYearModel()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            AcademicYear = row["academic_year"].ToString()
                        });
                    }
                    return academicYearModel;
                }
                else
                {
                    throw new Exception("Sorry. Something went wrong. Please contact administrator.");
                }
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }
        #endregion
    }
}