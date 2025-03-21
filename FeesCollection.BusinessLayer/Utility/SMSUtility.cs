using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace FeesCollection.BusinessLayer.Utility
{
    public class SMSUtility
    {
        public static bool SendSMS(string smsBody, string receiverMobileNo)
        {
            String message = HttpUtility.UrlEncode(smsBody);
            using (var wb = new WebClient())
            {
                //string SMS_API_KEY = ConfigurationManager.AppSettings["SMS_API_KEY_TXTLOCAL"];
                //string URL = ConfigurationManager.AppSettings["SMS_TRANSACTIONAL_API_TXTLOCAL"];
                //string SMS_SENDER_NAME = ConfigurationManager.AppSettings["SMS_TRANSACTIONAL_SENDER_NAME_TXTLOCAL"];

                string sendSMSUri = "http://roundsms.com/api/sendhttp.php?authkey=MTVlOWYwODY0Yzk&mobiles=" + receiverMobileNo +"&message=" + smsBody + "&sender=MFNSCR&type=1&route=2";

                var result = false;
                try
                {
                    HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                    UTF8Encoding encoding = new UTF8Encoding();
                    httpWReq.Method = "GET";
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

                //byte[] response = wb.UploadValues(URL, new NameValueCollection() {
                //    {"apikey" , SMS_API_KEY},
                //    {"numbers" , receiverMobileNo},
                //    {"message" , message},
                //    {"sender" , SMS_SENDER_NAME}
                //});
                //string result = System.Text.Encoding.UTF8.GetString(response);
                //return result;
            }
        }
    }
}
