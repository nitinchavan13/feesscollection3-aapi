using FeesCollection.ResponseModel.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.ExamModels
{
    public class QuestionModel: BaseModel
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string CorrectOption { get; set; }
        public int MarkPerQuestion { get; set; }
    }
}
