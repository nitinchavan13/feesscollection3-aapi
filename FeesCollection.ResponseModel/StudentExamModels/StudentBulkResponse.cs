using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.StudentExamModels
{
    public class StudentBulkResponse
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string SelectedOption { get; set; }
        public bool IsAnswerCorrect { get; set; }
    }
}
