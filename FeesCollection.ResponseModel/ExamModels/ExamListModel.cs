using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.ExamModels
{
    public class ExamListModel
    {
        public ExamListModel()
        {
            TodayExams = new List<ExamModel>();
            UpcomingExams = new List<ExamModel>();
            PastExams = new List<ExamModel>();
        }
        public List<ExamModel> TodayExams { get; set; }
        public List<ExamModel> UpcomingExams { get; set; }
        public List<ExamModel> PastExams { get; set; }
    }
}
