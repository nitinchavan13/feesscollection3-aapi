using FeesCollection.ResponseModel.BaseModels;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.Utility
{
    public static class MySqlParametersHelper
    {
        public static MySqlParameter[] GetHeaderParameters(BaseModel model)
        {
            return new MySqlParameter[]
            {
                new MySqlParameter("@p_academicId", model.AcademicYearId),
                new MySqlParameter("@p_userId", model.UserId)
            };
        }
    }
}
