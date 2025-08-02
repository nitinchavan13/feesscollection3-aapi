using FeesCollection.BusinessLayer.Utility;
using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.BaseModels;
using FeesCollection.ResponseModel.ExamModels;
using FeesCollection.ResponseModel.StudentExamModels;
using FeesCollection.ResponseModel.Utility;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.ExamService
{
    public interface IStudentExamService
    {
        Task<List<StudentExamModel>> FetchMyTodaysExams(BaseModel model);
        Task<int> UpdateExamAttendance(int studentId, int examId);
        Task<List<StudentQuestionModel>> FetchQuestions(int examId);
        Task<bool> SaveExamAnswers(StudentExamResponse model);
    }

    public class StudentExamService : IStudentExamService
    {
        #region Constructor
        DBHelper _dBHelper;
        private readonly Random _rand;
        public StudentExamService()
        {
            _rand = new Random();
            _dBHelper = new DBHelper(ConfigurationManager.AppSettings["mysqlConnectionString"]);
        }
        #endregion

        #region Public methods
        public async Task<List<StudentExamModel>> FetchMyTodaysExams(BaseModel model)
        {
            List<StudentExamModel> examList = new List<StudentExamModel>();
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_academicId", model.AcademicYearId),
                new MySqlParameter("p_studentId", model.UserId),
                new MySqlParameter("p_todaysDate", TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow))
            };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_get_student_exams", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        StudentExamModel examDetails = new StudentExamModel
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Title = row["title"].ToString(),
                            ExamDate = Convert.ToDateTime(row["examDate"]),
                            IsAllDayEvent = Convert.ToBoolean(row["isAllDayEvent"]),
                            StartTime = Convert.ToBoolean(row["isAllDayEvent"]) ? "" : row["startTime"].ToString(),
                            EndTime = Convert.ToBoolean(row["isAllDayEvent"]) ? "" : row["endTime"].ToString(),
                            TotalMarks = Convert.ToInt32(row["totalMarks"]),
                            MinPassingMarks = Convert.ToInt32(row["minPassingMarks"]),
                            Duration = Convert.ToInt32(row["durationInMinutes"]),
                            IsAttempted = row["isattempted"] == DBNull.Value ? false : Convert.ToBoolean(row["isattempted"]),
                            IsComplete = row["iscompleted"] == DBNull.Value ? false : Convert.ToBoolean(row["iscompleted"])
                        };
                        examDetails.CanStartExam = CanStartExam(examDetails.StartTime, examDetails.EndTime);
                        examList.Add(examDetails);

                    }
                }
                return examList;
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<List<StudentQuestionModel>> FetchQuestions(int examId)
        {
            List<StudentQuestionModel> questions = new List<StudentQuestionModel>();
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_examid", examId)
            };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_get_student_exam_questions", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        questions.Add(new StudentQuestionModel()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            QuestionText = row["questiontext"].ToString(),
                            Option1 = row["option1"].ToString(),
                            Option2 = row["option2"].ToString(),
                            Option3 = row["option3"].ToString(),
                            Option4 = row["option4"].ToString()
                        });
                    }
                }
                return GenerateRandomQuestionSequence(questions);
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<int> UpdateExamAttendance(int studentId, int examId)
        {
            bool isAlreadyAttempted = CheckExamAttempt(studentId, examId);
            if (!isAlreadyAttempted)
            {
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("p_examid", examId),
                    new MySqlParameter("p_studentid", studentId),
                    new MySqlParameter("p_examstartdate", TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow))
                };
                try
                {
                    var id = await _dBHelper.ExecuteScalarAsync("sp_add_student_attempt", parameters);
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

        public async Task<bool> SaveExamAnswers(StudentExamResponse model)
        {
            ExamModel examDetails = await GetExamDetailsAsync(model.ExamId);
            List<QuestionModel> examQuestions = await GetAllExamQuestionAsync(model.ExamId);
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
                if (question != null && answer.SelectedOption.ToLower() == question.CorrectOption.ToLower())
                {
                    totalMarks = totalMarks + 2;
                    response.IsAnswerCorrect = true;
                }
                studentResponse.Add(response);
            }

            bool passingStatus = totalMarks >= examDetails.MinPassingMarks ? true : false;
            await SaveFinalResult(model, totalMarks, passingStatus);

            await SaveBulkStudentResponse(studentResponse);

            return true;
        }
        #endregion

        #region Private methods

        private List<StudentQuestionModel> GenerateRandomQuestionSequence(List<StudentQuestionModel> listToShuffle)
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

        private bool CanStartExam(string startTime, string endTime)
        {
            DateTime startTimeDate = TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow);
            bool res = DateTime.TryParse(startTime, out startTimeDate);
            DateTime endTimeDate = TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow);
            bool res1 = DateTime.TryParse(endTime, out endTimeDate);
            DateTime currentDateTime = TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow);
            if (currentDateTime.Ticks > startTimeDate.Ticks && currentDateTime.Ticks < endTimeDate.Ticks) return true;
            else return false;
        }

        private async Task SaveBulkStudentResponse(List<StudentBulkResponse> studentResponse)
        {
            StringBuilder sCommand = new StringBuilder("INSERT INTO `tblstudentexamattemptdetails`(`examattemptid`,`questionid`,`selectedoption`,`isanswercorrect`) VALUES ");
            List<string> Rows = new List<string>();
            foreach (var item in studentResponse)
            {
                Rows.Add(string.Format("('{0}','{1}', '{2}', '{3}')",
                    MySqlHelper.EscapeString(item.Id.ToString()), MySqlHelper.EscapeString(item.QuestionId.ToString()),
                    MySqlHelper.EscapeString(item.SelectedOption), MySqlHelper.EscapeString(item.IsAnswerCorrect ? "1" : "")));
            }
            sCommand.Append(string.Join(",", Rows));
            sCommand.Append(";");
            try
            {
                await _dBHelper.ExecuteNonQueryAsync(sCommand.ToString());
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        private async Task SaveFinalResult(StudentExamResponse model, int totalMarks, bool passingStatus)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_id", model.Id),
                new MySqlParameter("@p_passingstatus", passingStatus),
                new MySqlParameter("@p_totalmarks", totalMarks),
                new MySqlParameter("@p_examsubmitteddate", TimezoneHelper.getLocaltimeFromUniversal(DateTime.UtcNow))
            };
            try
            {
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_add_student_exam_response", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<ExamModel> GetExamDetailsAsync(int examId)
        {
            ExamModel examDetails = new ExamModel();
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_examId", examId)
            };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_exam_get_details", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        examDetails.Id = Convert.ToInt32(row["id"]);
                        examDetails.Title = row["title"].ToString();
                        examDetails.ExamDate = Convert.ToDateTime(row["examDate"]);
                        examDetails.IsAllDayEvent = Convert.ToBoolean(row["isAllDayEvent"]);
                        examDetails.StartTime = Convert.ToBoolean(row["isAllDayEvent"]) ? "" : row["startTime"].ToString();
                        examDetails.EndTime = Convert.ToBoolean(row["isAllDayEvent"]) ? "" : row["endTime"].ToString();
                        examDetails.TotalMarks = Convert.ToInt32(row["totalMarks"]);
                        examDetails.MinPassingMarks = Convert.ToInt32(row["minPassingMarks"]);
                        examDetails.Duration = Convert.ToInt32(row["durationInMinutes"]);
                    }
                }
                return examDetails;
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        private async Task<List<QuestionModel>> GetAllExamQuestionAsync(int examId)
        {
            List<QuestionModel> examQuestions = new List<QuestionModel>();
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@p_examId", examId) };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_get_exam_questions_for_admin", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        examQuestions.Add(new QuestionModel()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            QuestionText = row["questiontext"].ToString(),
                            Option1 = row["option1"].ToString(),
                            Option2 = row["option2"].ToString(),
                            Option3 = row["option3"].ToString(),
                            Option4 = row["option4"].ToString(),
                            CorrectOption = row["correctOption"].ToString(),
                            MarkPerQuestion = Convert.ToInt32(row["markPerQuestion"])
                        });
                    }
                }
                return examQuestions;
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        private bool CheckExamAttempt(int studentId, int examId)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_examid", examId),
                new MySqlParameter("p_studentid", studentId)
            };
            try
            {
                int attemptCount = Convert.ToInt32(_dBHelper.ExecuteScalarAsync("sp_student_check_exam_already_attempted", parameters));
                if (attemptCount > 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
        #endregion
    }
}
