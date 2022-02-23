using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class NameCheck
{


      private const String host = "https://eid.shumaidata.com";
      private const String path = "/eid/check";
      private const String method = "POST";
      private const String appcode = "f737944a42f64616a0170f9e566179e0";

      public static void Main(string name,string number,out string strData)
      {
            string str = string.Format("idcard={0}&name={1}", number, name);
            string querys =/* "idcard=350301********9422&name=张*";*/str;
            string bodys = "";
            string url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                  url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                  ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                  httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                  httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            if (0 < bodys.Length)
            {
                  byte[] data = Encoding.UTF8.GetBytes(bodys);
                  using (Stream stream = httpRequest.GetRequestStream())
                  {
                        stream.Write(data, 0, data.Length);
                  }
            }
            try
            {
                  httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                  httpResponse = (HttpWebResponse)ex.Response;
            }

            Console.WriteLine(httpResponse.StatusCode);
            Console.WriteLine(httpResponse.Method);
            Console.WriteLine(httpResponse.Headers);
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            strData = reader.ReadToEnd();
            Debug.Log("strData: "+strData);
            reader.Close();
      }

      public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
      {
            return true;
      }

}


/// <summary>  
/// 验证身份证号码类  
/// </summary>  
public class IDCardValidation
{
      /// <summary>  
      /// 验证身份证合理性  
      /// </summary>  
      /// <param name="Id"></param>  
      /// <returns></returns>  
      public static bool CheckIDCard(string idNumber)
      {
            if (idNumber.Length == 18)
            {
                  bool check = CheckIDCard18(idNumber);
                  return check;
            }
            else
            {
                  return false;
            }
      }


      /// <summary>  
      /// 18位身份证号码验证  
      /// </summary>  
      private static bool CheckIDCard18(string idNumber)
      {
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                  return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                  return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                  return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                  sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                  return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
      }

      public static Result SpriteResult(string resultSt)
      {

            //获取code值
            string[] spl = resultSt.Split('{');
            int code = int.Parse(spl[1].Substring(spl[1].IndexOf(':') + 2, 1));
            if (code != 0) return null;

            //分割result
            string[] res = spl[2].Split(',');
            string[] resSp = new string[res.Length];
            Result result = new Result();

            for (int i = 0; i < res.Length; i++)
            {
                  resSp[i] = res[i].Substring(res[i].IndexOf(':') + 2, res[i].LastIndexOf('"') - (res[i].IndexOf(':') + 2));
            }
            result.name = resSp[0];
            result.idcard = resSp[1];
            result.res = int.Parse(resSp[2]);
            result.sex = resSp[4];
            result.birthday = resSp[5];
            result.address = resSp[6];

            if (result.res == 2)
                  return null;
            Debug.Log(result.name + "\n" + result.idcard + "\n" + result.res + "\n" + result.sex + "\n" + result.birthday + "\n" + result.address);
            return result;
      }
}
[Serializable]
public class Result
{
      public string name;
      public string idcard;
      public int res;//1 一致 2 不一致 3 无记录
      public string sex;
      public string birthday;
      public string address;
}



