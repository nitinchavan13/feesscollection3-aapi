using FeesCollection.ResponseModel.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.StudentExamModels
{
    public class StudentExamModel : BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ExamDate { get; set; }
        public bool IsAllDayEvent { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int TotalMarks { get; set; }
        public int MinPassingMarks { get; set; }
        public int Duration { get; set; }
        public bool IsAttempted { get; set; }
        public bool IsComplete { get; set; }
        public bool CanStartExam { get; set; }
    }
}
