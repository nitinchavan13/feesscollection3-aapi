using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.ExpenceModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.ExpenceService
{
    public interface IExpenceService
    {
        List<ExpenceModel> GetExpences(BaseModel model);
        List<ExpenceModel> AddExpence(ExpenceModel model);
        List<ExpenceModel> EditExpence(ExpenceModel model);
        List<ExpenceModel> DeleteExpence(ExpenceModel model);
    }

    public class ExpenceService : IExpenceService
    {
        #region Constructor
        DBHelper _dBHelper;
        public ExpenceService()
        {
            _dBHelper = new DBHelper();
            _dBHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }
        #endregion

        #region Methods
        public List<ExpenceModel> AddExpence(ExpenceModel model)
        {
            _dBHelper.AddParameter("p_expencedate", model.ExpenceDate);
            _dBHelper.AddParameter("p_expenceamount", model.ExpenceAmount);
            _dBHelper.AddParameter("p_expencenote", model.ExpenceNote);
            _dBHelper.AddParameter("p_expencetype", model.ExpenceType);
            _dBHelper.AddParameter("p_academicid", model.AcademicYearId);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_expence_add", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);

                return this.GetExpences(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ExpenceModel> DeleteExpence(ExpenceModel model)
        {
            _dBHelper.AddParameter("p_id", model.Id);
            _dBHelper.AddParameter("p_academicid", model.AcademicYearId);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_expence_delete", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);

                return this.GetExpences(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ExpenceModel> EditExpence(ExpenceModel model)
        {
            _dBHelper.AddParameter("p_expencedate", model.ExpenceDate);
            _dBHelper.AddParameter("p_expenceamount", model.ExpenceAmount);
            _dBHelper.AddParameter("p_expencenote", model.ExpenceNote);
            _dBHelper.AddParameter("p_expencetype", model.ExpenceType);
            _dBHelper.AddParameter("p_id", model.Id);
            _dBHelper.AddParameter("p_expencestatus", true);
            _dBHelper.AddParameter("p_academicid", model.AcademicYearId);
            try
            {
                _dBHelper.ExecuteNonQuery("sp_expence_update", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);

                return this.GetExpences(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ExpenceModel> GetExpences(BaseModel model)
        {
            List<ExpenceModel> expences = new List<ExpenceModel>();
            _dBHelper.AddParameter("p_academicid", model.AcademicYearId);
            var reader = _dBHelper.ExecuteReader("sp_expence_get", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        expences.Add(new ExpenceModel()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            ExpenceDate = Convert.ToDateTime(reader["expencedate"]),
                            ExpenceAmount = Convert.ToDecimal(reader["expenceamount"]),
                            ExpenceNote = reader["expencenote"].ToString(),
                            ExpenceType = reader["expencetype"].ToString()
                        });
                    }
                    return expences.OrderByDescending(x => x.ExpenceDate).ToList();
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
        #endregion
    }
}
