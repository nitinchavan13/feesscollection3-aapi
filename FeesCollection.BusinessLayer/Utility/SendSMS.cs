using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.BusinessLayer.Utility
{
    public class SendSMS
    {
        public static bool SMSSend(string body, string receiverMobileNo)
        {
            string SMS_API_KEY = ConfigurationManager.AppSettings["SMS_API_KEY"];
            string URL = ConfigurationManager.AppSettings["SMS_TRANSACTIONAL_API"];
            string SMS_SENDER_NAME = ConfigurationManager.AppSettings["SMS_TRANSACTIONAL_SENDER_NAME"];
            string mobileNumber = "91" + receiverMobileNo;
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", SMS_API_KEY);
            sbPostData.AppendFormat("&mobiles={0}", mobileNumber);
            sbPostData.AppendFormat("&message={0}", body);
            sbPostData.AppendFormat("&sender={0}", SMS_SENDER_NAME);
            sbPostData.AppendFormat("&route={0}", "4");
            sbPostData.AppendFormat("&country={0}", "91");
            URL = URL + "?" + sbPostData;
            return SendSms(URL, APIMethodTypes.GET.ToString());
        }

        public static bool MultiSMSSend(string body, List<string> receiverMobileNos)
        {
            string SMS_API_KEY = ConfigurationManager.AppSettings["SMS_API_KEY"];
            string URL = ConfigurationManager.AppSettings["SMS_TRANSACTIONAL_API"];
            string SMS_SENDER_NAME = ConfigurationManager.AppSettings["SMS_TRANSACTIONAL_SENDER_NAME"];
            string mobileNumbers = "";
            foreach (var item in receiverMobileNos)
            {
                mobileNumbers += "91" + item + ", ";
            }
            mobileNumbers = mobileNumbers.TrimEnd(' ').TrimEnd(',');
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", SMS_API_KEY);
            sbPostData.AppendFormat("&mobiles={0}", mobileNumbers);
            sbPostData.AppendFormat("&message={0}", body);
            sbPostData.AppendFormat("&sender={0}", SMS_SENDER_NAME);
            sbPostData.AppendFormat("&route={0}", "4");
            sbPostData.AppendFormat("&country={0}", "91");
            URL = URL + "?" + sbPostData;
            return SendSms(URL, APIMethodTypes.GET.ToString());
        }

        public static SMSResponse SendOtp(string receiverMobileNo, int otp)
        {
            string SMS_API_KEY = ConfigurationManager.AppSettings["SMS_API_KEY"];
            string URL = ConfigurationManager.AppSettings["SMS_OTP_API"];
            string SMS_SENDER_NAME = ConfigurationManager.AppSettings["SMS_OTP_SENDER_NAME"];
            string SMS_OTP_BODY = ConfigurationManager.AppSettings["SMS_OTP_BODY"];
            SMS_OTP_BODY = SMS_OTP_BODY.Replace("##OTP##", otp.ToString());
            string SMS_OTP_EXPIRY = ConfigurationManager.AppSettings["SMS_OTP_EXPIRY"];
            string mobileNumber = "91" + receiverMobileNo;
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", SMS_API_KEY);
            sbPostData.AppendFormat("&mobile={0}", mobileNumber);
            sbPostData.AppendFormat("&message={0}", SMS_OTP_BODY);
            sbPostData.AppendFormat("&sender={0}", SMS_SENDER_NAME);
            sbPostData.AppendFormat("&otp_expiry={0}", SMS_OTP_EXPIRY);
            sbPostData.AppendFormat("&otp={0}", otp);
            return SendSms(URL, APIMethodTypes.POST.ToString(), sbPostData);
        }

        public static SMSResponse ValidateOtp(string receiverMobileNo, string otp)
        {
            string SMS_API_KEY = ConfigurationManager.AppSettings["SMS_API_KEY"];
            string URL = ConfigurationManager.AppSettings["SMS_OTP_VERIFY_API"];
            string mobileNumber = "91" + receiverMobileNo;
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", SMS_API_KEY);
            sbPostData.AppendFormat("&mobile={0}", mobileNumber);
            sbPostData.AppendFormat("&otp={0}", otp);
            return SendSms(URL, APIMethodTypes.POST.ToString(), sbPostData);
        }

        public static SMSResponse ResendOtp(string receiverMobileNo)
        {
            string SMS_API_KEY = ConfigurationManager.AppSettings["SMS_API_KEY"];
            string URL = ConfigurationManager.AppSettings["SMS_OTP_RESEND_API"];
            string mobileNumber = "91" + receiverMobileNo;
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", SMS_API_KEY);
            sbPostData.AppendFormat("&mobile={0}", mobileNumber);
            sbPostData.AppendFormat("&retrytype={0}", "text");
            return SendSms(URL, APIMethodTypes.POST.ToString(), sbPostData);
        }

        private static SMSResponse SendSms(string URL, string methodType, StringBuilder sbPostData)
        {
            try
            {
                string sendSMSUri = URL;
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(sbPostData.ToString());
                httpWReq.Method = methodType;
                if (methodType == APIMethodTypes.POST.ToString())
                {
                    httpWReq.ContentType = "application/x-www-form-urlencoded";
                    httpWReq.ContentLength = data.Length;
                    using (Stream stream = httpWReq.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                else
                {
                    httpWReq.ContentType = "application/json";
                }
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();
                var responseData = JsonConvert.DeserializeObject<SMSResponse>(responseString);
                reader.Close();
                response.Close();
                return responseData;
            }
            catch (SystemException ex)
            {
                return new SMSResponse()
                {
                    Type = SmsReponseEnum.ERROR.ToString()
                };
            }
        }

        private static bool SendSms(string URL, string methodType)
        {
            var result = false;
            try
            {
                string sendSMSUri = URL;
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                UTF8Encoding encoding = new UTF8Encoding();
                httpWReq.Method = methodType;
                httpWReq.ContentType = "application/json";
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                response.Close();
                result = true; ;
            }
            catch (SystemException ex)
            {
                result = false;
            }
            return result;
        }
    }

    public class SMSResponse
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public int OTP { get; set; }
    }

    public enum SmsReponseEnum
    {
        SUCCESS,
        ERROR
    }

    public enum APIMethodTypes
    {
        GET,
        POST
    }
}
