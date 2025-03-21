using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.DashboardModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.DashboardService
{
    public interface IDashboardService
    {
        DashboardCardModel GetDashboardCards(BaseModel model);
    }

    public class DashboardService : IDashboardService
    {
        #region Constructor
        DBHelper _dBHelper;
        public DashboardService()
        {
            _dBHelper = new DBHelper();
            _dBHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }
        #endregion

        #region Methods
        public DashboardCardModel GetDashboardCards(BaseModel model)
        {
            _dBHelper.AddParameter("p_userid", model.UserId);
            _dBHelper.AddParameter("p_academicid", model.AcademicYearId);
            var reader = _dBHelper.ExecuteReader("sp_dashboard_cards_get", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    DashboardCardModel cards = new DashboardCardModel();
                    while (reader.Read())
                    {
                        cards.TotalStudents = Convert.ToInt32(reader["totalstudents"]);
                        cards.TotalCredit = Convert.ToDecimal(reader["totalcredit"]);
                        cards.TotalDebit = Convert.ToDecimal(reader["totaldebit"]);
                    }
                    return cards;
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
