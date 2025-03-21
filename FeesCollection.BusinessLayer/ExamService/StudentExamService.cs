using FeesCollection.BusinessLayer.Utility;
using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.ExamModels;
using FeesCollection.ResponseModel.StudentExamModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.ExamService
{
    public interface IStudentExamService
    {
         List<StudentExamModel> FetchMyTodaysExams(BaseModel model);
        int UpdateExamAttendance(int studentId, int examId);
        List<StudentQuestionModel> FetchQuestions(int examId);
        bool SaveExamAnswers(StudentExamResponse model);
    }

    public class StudentExamService : IStudentExamService
    {
        #region Constructor
        DBHelper _dBHelper;
        private readonly Random _rand;
        public StudentExamService()
        {
            _rand = new Random();
            _dBHelper = new DBHelper();
            _dBHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }
        #endregion

        public List<StudentQuestionModel> GenerateRandomQuestionSequence(List<StudentQuestionModel> listToShuffle)
        {
            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = _rand.Next(i + 1);
                var value = listToShuffle[k];
                listToShuffle[k] = listToShuffle[i];
                listToShuffle[i] = value;
            }
            return listToShuffle;
        }

        public List<StudentExamModel> FetchMyTodaysExams(BaseModel model)
        {
            List<StudentExamModel> examList = new List<StudentExamModel>();
            _dBHelper.AddParameter("p_academicId", model.AcademicYearId);
            _dBHelper.AddParameter("p_studentId", model.UserId);
            _dBHelper.AddParameter("p_todaysDate", TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow));
            var reader = _dBHelper.ExecuteReader("sp_get_student_exams", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        StudentExamModel examDetails = new StudentExamModel();
                        examDetails.Id = Convert.ToInt32(reader["id"]);
                        examDetails.Title = reader["title"].ToString();
                        examDetails.ExamDate = Convert.ToDateTime(reader["examDate"]);
                        examDetails.IsAllDayEvent = Convert.ToBoolean(reader["isAllDayEvent"]);
                        examDetails.StartTime = Convert.ToBoolean(reader["isAllDayEvent"]) ? "" : reader["startTime"].ToString();
                        examDetails.EndTime = Convert.ToBoolean(reader["isAllDayEvent"]) ? "" : reader["endTime"].ToString();
                        examDetails.TotalMarks = Convert.ToInt32(reader["totalMarks"]);
                        examDetails.MinPassingMarks = Convert.ToInt32(reader["minPassingMarks"]);
                        examDetails.Duration = Convert.ToInt32(reader["durationInMinutes"]);
                        examDetails.IsAttempted = reader["isattempted"] == DBNull.Value ? false : Convert.ToBoolean(reader["isattempted"]);
                        examDetails.IsComplete = reader["iscompleted"] == DBNull.Value ? false : Convert.ToBoolean(reader["iscompleted"]);
                        examList.Add(examDetails);
                        examDetails.CanStartExam = CanStartExam(examDetails.StartTime, examDetails.EndTime);
                    }
                    return examList;
                }
                else
                {
                    return examList;
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

        private bool CanStartExam(string startTime, string endTime)
        {
            DateTime startTimeDate;
            bool res = DateTime.TryParse(startTime, out startTimeDate);
            DateTime endTimeDate;
            bool res1 = DateTime.TryParse(endTime, out endTimeDate);
            DateTime currentDateTime = DateTime.Now;
            if (currentDateTime.Ticks > startTimeDate.Ticks && currentDateTime.Ticks < endTimeDate.Ticks) return true;
            else return false;
        }

        public List<StudentQuestionModel> FetchQuestions(int examId)
        {
            List<StudentQuestionModel> questions = new List<StudentQuestionModel>();
            _dBHelper.AddParameter("p_examid", examId);
            var reader = _dBHelper.ExecuteReader("sp_get_student_exam_questions", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        questions.Add(new StudentQuestionModel()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            QuestionText = reader["questiontext"].ToString(),
                            Option1 = reader["option1"].ToString(),
                            Option2 = reader["option2"].ToString(),
                            Option3 = reader["option3"].ToString(),
                            Option4 = reader["option4"].ToString()
                        });
                    }
                    return GenerateRandomQuestionSequence(questions);
                }
                else
                {
                    return questions;
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

        public int UpdateExamAttendance(int studentId, int examId)
        {
            bool isAlreadyAttempted = CheckExamAttempt(studentId, examId);
            if(!isAlreadyAttempted)
            {
                _dBHelper.AddParameter("p_examid", examId);
                _dBHelper.AddParameter("p_studentid", studentId);
                _dBHelper.AddParameter("p_examstartdate", TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow));
                try
                {
                    var id = _dBHelper.ExecuteScaler("sp_add_student_attempt", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
                    return Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                throw new Exception("It looks like you already attempted this exam. If you have any issues, please contact administrator.");
            }
        }

        public bool SaveExamAnswers(StudentExamResponse model)
        {
            ExamModel examDetails = GetExamDetails(model.ExamId);
            List<QuestionModel> examQuestions = GetAllExamQuestion(model.ExamId);
            List<StudentBulkResponse> studentResponse = new List<StudentBulkResponse>();
            var totalMarks = 0;
            foreach (var answer in model.Answers)
            {
                StudentBulkResponse response = new StudentBulkResponse();
                response.Id = model.Id;
                response.QuestionId = answer.Id;
                response.SelectedOption = answer.SelectedOption;
                response.IsAnswerCorrect = false;
                var question = examQuestions.FirstOrDefault(x => x.Id == answer.Id);
                if(question != null && answer.SelectedOption.ToLower() == question.CorrectOption.ToLower())
                {
                    totalMarks = totalMarks + 2;
                    response.IsAnswerCorrect = true;
                }
                studentResponse.Add(response);
            }

            bool passingStatus = totalMarks >= examDetails.MinPassingMarks ? true : false;
            SaveFinalResult(model, totalMarks, passingStatus);

            _dBHelper.SaveBulkStudentResponse(studentResponse);

            return true;
        }

        private void SaveFinalResult(StudentExamResponse model, int totalMarks, bool passingStatus)
        {
            _dBHelper.AddParameter("p_id", model.Id);
            _dBHelper.AddParameter("p_passingstatus", passingStatus);
            _dBHelper.AddParameter("p_totalmarks", totalMarks);
            _dBHelper.AddParameter("p_examsubmitteddate", TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow));
            try
            {
                _dBHelper.ExecuteNonQuery("sp_add_student_exam_response", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private ExamModel GetExamDetails(int examId)
        {
            ExamModel examDetails = new ExamModel();
            _dBHelper.AddParameter("p_examId", examId);
            var reader = _dBHelper.ExecuteReader("sp_exam_get_details", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        examDetails.Id = Convert.ToInt32(reader["id"]);
                        examDetails.Title = reader["title"].ToString();
                        examDetails.ExamDate = Convert.ToDateTime(reader["examDate"]);
                        examDetails.IsAllDayEvent = Convert.ToBoolean(reader["isAllDayEvent"]);
                        examDetails.StartTime = Convert.ToBoolean(reader["isAllDayEvent"]) ? "" : reader["startTime"].ToString();
                        examDetails.EndTime = Convert.ToBoolean(reader["isAllDayEvent"]) ? "" : reader["endTime"].ToString();
                        examDetails.TotalMarks = Convert.ToInt32(reader["totalMarks"]);
                        examDetails.MinPassingMarks = Convert.ToInt32(reader["minPassingMarks"]);
                        examDetails.Duration = Convert.ToInt32(reader["durationInMinutes"]);

                    }
                    return examDetails;
                }
                else
                {
                    return examDetails;
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

        private List<QuestionModel> GetAllExamQuestion(int examId)
        {
            List<QuestionModel> examQuestions = new List<QuestionModel>();
            _dBHelper.AddParameter("p_examId", examId);
            var reader = _dBHelper.ExecuteReader("sp_get_exam_questions_for_admin", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        examQuestions.Add(new QuestionModel()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            QuestionText = reader["questiontext"].ToString(),
                            Option1 = reader["option1"].ToString(),
                            Option2 = reader["option2"].ToString(),
                            Option3 = reader["option3"].ToString(),
                            Option4 = reader["option4"].ToString(),
                            CorrectOption = reader["correctOption"].ToString(),
                            MarkPerQuestion = Convert.ToInt32(reader["markPerQuestion"])
                        });
                    }
                    return examQuestions;
                }
                else
                {
                    return examQuestions;
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

        private bool CheckExamAttempt(int studentId, int examId)
        {
            _dBHelper.AddParameter("p_examid", examId);
            _dBHelper.AddParameter("p_studentid", studentId);
            try
            {
                int attemptCount = Convert.ToInt32(_dBHelper.ExecuteScaler("sp_student_check_exam_already_attempted", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open));
                if (attemptCount > 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (_dBHelper.connection.State == System.Data.ConnectionState.Open)
                {
                    _dBHelper.connection.Close();
                }
            }
        }
    }
}
