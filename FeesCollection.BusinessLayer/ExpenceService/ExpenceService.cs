using FeesCollection.BusinessLayer.Utility;
using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.ExpenceModels;
using FeesCollection.ResponseModel.Utility;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.ExpenceService
{
    public interface IExpenceService
    {
        Task<List<ExpenceModel>> GetExpences(BaseModel model);
        Task<List<ExpenceModel>> AddExpence(ExpenceModel model);
        Task<List<ExpenceModel>> EditExpence(ExpenceModel model);
        Task<List<ExpenceModel>> DeleteExpence(ExpenceModel model);
    }

    public class ExpenceService : IExpenceService
    {
        #region Constructor
        DBHelper _dBHelper;
        public ExpenceService()
        {
            _dBHelper = new DBHelper(ConfigurationManager.AppSettings["mysqlConnectionString"]);
        }
        #endregion

        //#region Methods
        public async Task<List<ExpenceModel>> GetExpences(BaseModel model)
        {
            List<ExpenceModel> expences = new List<ExpenceModel>();
            MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_academicid", model.AcademicYearId)
                };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_expence_get", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        expences.Add(new ExpenceModel()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            ExpenceDate = TimezoneHelper.GetLocaltimeFromUniversal(Convert.ToDateTime(row["expencedate"])),
                            ExpenceAmount = Convert.ToDecimal(row["expenceamount"]),
                            ExpenceNote = row["expencenote"].ToString(),
                            ExpenceType = row["expencetype"].ToString()
                        });
                    }
                    return expences.OrderByDescending(x => x.ExpenceDate).ToList();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }
        public async Task<List<ExpenceModel>> AddExpence(ExpenceModel model)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_expencedate", TimezoneHelper.ConvertLocalToUTCwithTimeZone(model.ExpenceDate).Date),
                new MySqlParameter("@p_expenceamount", model.ExpenceAmount),
                new MySqlParameter("@p_expencenote", model.ExpenceNote),
                new MySqlParameter("@p_expencetype", model.ExpenceType),
                new MySqlParameter("@p_academicid", model.AcademicYearId)
            };
            try
            {
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_expence_add", parameters);

                return await this.GetExpences(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ExpenceModel>> EditExpence(ExpenceModel model)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_expencedate", TimezoneHelper.ConvertLocalToUTCwithTimeZone(model.ExpenceDate).Date),
                new MySqlParameter("@p_expenceamount", model.ExpenceAmount),
                new MySqlParameter("@p_expencenote", model.ExpenceNote),
                new MySqlParameter("@p_expencetype", model.ExpenceType),
                new MySqlParameter("@p_academicid", model.AcademicYearId),
                new MySqlParameter("@p_id", model.Id),
                new MySqlParameter("@p_expencestatus", true)
            };
            try
            {
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_expence_update", parameters);

                return await this.GetExpences(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ExpenceModel>> DeleteExpence(ExpenceModel model)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] {
                    new MySqlParameter("@p_id", model.Id),
                    new MySqlParameter("@p_academicid", model.AcademicYearId)
                };
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_expence_delete", parameters);

                return await this.GetExpences(model);
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }
        //#endregion
    }
}
