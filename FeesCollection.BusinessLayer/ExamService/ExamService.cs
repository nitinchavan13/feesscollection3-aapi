using FeesCollection.BusinessLayer.Utility;
using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.ExamModels;
using FeesCollection.ResponseModel.Utility;
using Google.Protobuf.WellKnownTypes;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.ExamService
{
    public interface IExamService
    {
        Task<ExamListModel> GetAllExams(int academicId, int userId);
        Task<ExamModel> GetExamDetails(int examId);
        Task<int> CreateNewExam(ExamModel model);
        Task<List<QuestionModel>> GetAllExamQuestion(int examId);
        Task<bool> CreateQuestion(QuestionModel model, int examId = 0);
    }

    public class ExamService : IExamService
    {
        #region Constructor
        DBHelper _dBHelper;
        public ExamService()
        {
            _dBHelper = new DBHelper(ConfigurationManager.AppSettings["mysqlConnectionString"]);
        }
        #endregion

        //#region Methods
        public async Task<ExamListModel> GetAllExams(int academicId, int userId)
        {
            List<ExamModel> exams = new List<ExamModel>();
            ExamListModel examList = new ExamListModel();
            MySqlParameter[] parameters = new MySqlParameter[] {
                new MySqlParameter("@p_userId", userId),
                new MySqlParameter("@p_academicId", academicId)
            };
            DataTable result = await _dBHelper.ExecuteStoredProcedureDataTableAsync("sp_exam_get", parameters);
            try
            {
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        exams.Add(new ExamModel()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Title = row["title"].ToString(),
                            ExamDate = Convert.ToDateTime(row["examDate"]),
                            IsAllDayEvent = Convert.ToBoolean(row["isAllDayEvent"]),
                            StartTime = Convert.ToBoolean(row["isAllDayEvent"]) ? "" : row["startTime"].ToString(),
                            EndTime = Convert.ToBoolean(row["isAllDayEvent"]) ? "" : row["endTime"].ToString(),
                            TotalMarks = Convert.ToInt32(row["totalMarks"]),
                            MinPassingMarks = Convert.ToInt32(row["minPassingMarks"]),
                            Duration = Convert.ToInt32(row["durationInMinutes"])
                        });
                    }
                    return GetSortedExamList(exams);
                }
                else
                {
                    return examList;
                }
            }
            catch (Exception)
            {
                throw new Exception(AppConstants.GENERIC_ERROR_MSG);
            }
        }

        public async Task<ExamModel> GetExamDetails(int examId)
        {
            ExamModel examDetails = new ExamModel();
            MySqlParameter[] parameters = new MySqlParameter[] {
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

        public async Task<int> CreateNewExam(ExamModel model)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_title", model.Title),
                new MySqlParameter("@p_examDate", TimezoneHelper.ConvertLocalToUTCwithTimeZone(model.ExamDate)),
                new MySqlParameter("@p_isAllDayEvent", model.IsAllDayEvent),
                new MySqlParameter("@p_startTime", model.IsAllDayEvent ? "" : model.StartTime),
                new MySqlParameter("@p_endTime", model.IsAllDayEvent ? "" : model.EndTime),
                new MySqlParameter("@p_totalMarks", model.TotalMarks),
                new MySqlParameter("@p_minPassingMarks", model.MinPassingMarks),
                new MySqlParameter("@p_duration", model.Duration),
                new MySqlParameter("@p_academicId", model.AcademicYearId),
                new MySqlParameter("@p_userId", model.UserId)
            };
            try
            {
                int newId = await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_exam_add", parameters);
                return newId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CreateQuestion(QuestionModel model, int examId = 0)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_questionText", model.QuestionText),
                new MySqlParameter("@p_option1", model.Option1),
                new MySqlParameter("@p_option2", model.Option2),
                new MySqlParameter("@p_option3", model.Option3),
                new MySqlParameter("@p_option4", model.Option4),
                new MySqlParameter("@p_correctOption", model.CorrectOption),
                new MySqlParameter("@p_markPerQuestion", model.MarkPerQuestion),
                new MySqlParameter("@p_academicId", model.AcademicYearId),
                new MySqlParameter("@p_userId", model.UserId)
            };
            try
            {
                int newId = await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_question_add", parameters);
                if (examId != 0)
                {
                    return await this.MapQuestionWithExam(newId, examId);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<QuestionModel>> GetAllExamQuestion(int examId)
        {
            List<QuestionModel> examQuestions = new List<QuestionModel>();
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_examId", examId)
            };

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
        //#endregion

        //#region Private Methods
        private ExamListModel GetSortedExamList(List<ExamModel> exams)
        {
            ExamListModel examList = new ExamListModel();
            if (exams.Count > 0)
            {
                foreach (var item in exams)
                {
                    if (item.ExamDate.Date == DateTime.Today)
                    {
                        examList.TodayExams.Add(item);
                    }
                    else if (item.ExamDate.Date < DateTime.Today)
                    {
                        examList.PastExams.Add(item);
                    }
                    else
                    {
                        examList.UpcomingExams.Add(item);
                    }
                }
            }
            return examList;
        }

        private async Task<bool> MapQuestionWithExam(int questionId, int examId)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_examId", examId),
                new MySqlParameter("@p_questionId", questionId)
            };
            try
            {
                await _dBHelper.ExecuteStoredProcedureNonQueryAsync("sp_question_exam_mapping", parameters);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //#endregion
    }
}
