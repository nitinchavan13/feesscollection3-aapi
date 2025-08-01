using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.DashboardModels;
using FeesCollection.ResponseModel.Utility;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.DashboardService
{
    public interface IDashboardService
    {
        Task<DashboardCardModel> GetDashboardCards(BaseModel model);
    }

    public class DashboardService : IDashboardService
    {
        #region Constructor
        DBHelper _dBHelper;
        public DashboardService()
        {
            _dBHelper = new DBHelper(ConfigurationManager.AppSettings["mysqlConnectionString"]);
        }
        #endregion

        #region Methods
        public async Task<DashboardCardModel> GetDashboardCards(BaseModel model)
        {
            MySqlParameter[] parameters = new MySqlParameter[] {
                new MySqlParameter("@p_userid", model.UserId),
                new MySqlParameter("@p_academicid", model.AcademicYearId)
            };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_dashboard_cards_get", parameters);
            try
            {

                if (result.Rows.Count > 0)
                {
                    DashboardCardModel cards = new DashboardCardModel();
                    var row = result.Rows[0];
                    cards.TotalStudents = Convert.ToInt32(row["totalstudents"]);
                    cards.TotalCredit = Convert.ToDecimal(row["totalcredit"]);
                    cards.TotalDebit = Convert.ToDecimal(row["totaldebit"]);

                    return cards;
                }
                return null;
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }
        #endregion
    }
}
