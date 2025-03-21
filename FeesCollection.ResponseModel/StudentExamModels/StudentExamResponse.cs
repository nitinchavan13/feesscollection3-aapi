using FeesCollection.ResponseModel.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.StudentExamModels
{
    public class StudentExamResponse : BaseModel
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public List<Answer> Answers { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }
        public string SelectedOption { get; set; }
    }
}
