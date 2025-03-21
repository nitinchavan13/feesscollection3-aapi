using FeesCollection.BusinessLayer.Utility;
using FeesCollection.DatabaseLayer.Helpers;
using FeesCollection.ResponseModel.ExamModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FeesCollection.BusinessLayer.ExamService
{
    public interface IExamService
    {
        ExamListModel GetAllExams(int academicId, int userId);
        ExamModel GetExamDetails(int examId);
        int CreateNewExam(ExamModel model);
        List<QuestionModel> GetAllExamQuestion(int examId);
        bool CreateQuestion(QuestionModel model, int examId = 0);
    }

    public class ExamService : IExamService
    {
        #region Constructor
        DBHelper _dBHelper;
        public ExamService()
        {
            _dBHelper = new DBHelper();
            _dBHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }
        #endregion

        #region Methods
        public ExamListModel GetAllExams(int academicId, int userId)
        {
            List<ExamModel> exams = new List<ExamModel>();
            ExamListModel examList = new ExamListModel();
            _dBHelper.AddParameter("p_academicId", academicId);
            _dBHelper.AddParameter("p_userId", userId);
            var reader = _dBHelper.ExecuteReader("sp_exam_get", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        exams.Add(new ExamModel()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Title = reader["title"].ToString(),
                            ExamDate = Convert.ToDateTime(reader["examDate"]),
                            IsAllDayEvent = Convert.ToBoolean(reader["isAllDayEvent"]),
                            StartTime = Convert.ToBoolean(reader["isAllDayEvent"]) ? "" : reader["startTime"].ToString(),
                            EndTime = Convert.ToBoolean(reader["isAllDayEvent"]) ? "" : reader["endTime"].ToString(),
                            TotalMarks = Convert.ToInt32(reader["totalMarks"]),
                            MinPassingMarks = Convert.ToInt32(reader["minPassingMarks"]),
                            Duration = Convert.ToInt32(reader["durationInMinutes"])
                        });
                    }
                    return GetSortedExamList(exams);
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

        public ExamModel GetExamDetails(int examId)
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

        public int CreateNewExam(ExamModel model)
        {
            _dBHelper.AddParameter("p_title", model.Title);
            _dBHelper.AddParameter("p_examDate", TimezoneHelper.ConvertLocalToUTCwithTimeZone(model.ExamDate));
            _dBHelper.AddParameter("p_isAllDayEvent", model.IsAllDayEvent);
            _dBHelper.AddParameter("p_startTime", model.IsAllDayEvent ? "" : model.StartTime);
            _dBHelper.AddParameter("p_endTime", model.IsAllDayEvent ? "" : model.EndTime);
            _dBHelper.AddParameter("p_totalMarks", model.TotalMarks);
            _dBHelper.AddParameter("p_minPassingMarks", model.MinPassingMarks);
            _dBHelper.AddParameter("p_duration", model.Duration);
            _dBHelper.AddParameter("p_academicId", model.AcademicYearId);
            _dBHelper.AddParameter("p_userId", model.UserId);
            try
            {
                int newId = Convert.ToInt32(_dBHelper.ExecuteScaler("sp_exam_add", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open));
                return newId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool CreateQuestion(QuestionModel model, int examId = 0)
        {
            _dBHelper.AddParameter("p_questionText", model.QuestionText);
            _dBHelper.AddParameter("p_option1", model.Option1);
            _dBHelper.AddParameter("p_option2", model.Option2);
            _dBHelper.AddParameter("p_option3", model.Option3);
            _dBHelper.AddParameter("p_option4", model.Option4);
            _dBHelper.AddParameter("p_correctOption", model.CorrectOption);
            _dBHelper.AddParameter("p_markPerQuestion", model.MarkPerQuestion);
            _dBHelper.AddParameter("p_academicId", model.AcademicYearId);
            _dBHelper.AddParameter("p_userId", model.UserId);
            try
            {
                int newId = Convert.ToInt32(_dBHelper.ExecuteScaler("sp_question_add", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open));
                if(examId != 0)
                {
                    _dBHelper.connection.Close();
                    return this.MapQuestionWithExam(newId, examId);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<QuestionModel> GetAllExamQuestion(int examId)
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
        #endregion

        #region Private Methods
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

        private bool MapQuestionWithExam(int questionId, int examId)
        {
            _dBHelper.AddParameter("p_examId", examId);
            _dBHelper.AddParameter("p_questionId", questionId);
            try
            {
                _dBHelper.ExecuteScaler("sp_question_exam_mapping", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
    }
}
