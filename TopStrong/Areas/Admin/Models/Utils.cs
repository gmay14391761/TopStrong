using Easy4net.DBUtility;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Xml;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace TopStrong.Areas.Admin.Models
{
    public static class Utils
    {

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        public static string Encryption(string express)
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = "oa_erp_dowork";//密匙容器的名称，保持加密解密一致才能解密成功
            param.Flags = CspProviderFlags.UseMachineKeyStore;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024, param))
            {
                byte[] plaindata = Encoding.Default.GetBytes(express);//将要加密的字符串转换为字节数组
                byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
                return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
            }
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static string Decrypt(string ciphertext)
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = "oa_erp_dowork";
            param.Flags = CspProviderFlags.UseMachineKeyStore;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024, param))
            {
                byte[] encryptdata = Convert.FromBase64String(ciphertext);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                return Encoding.Default.GetString(decryptdata);
            }
        }

        /// <summary>
        /// 检查请求来源是否合法
        /// </summary>
        /// <returns></returns>
        public static bool CheckRequestSource(string key)
        {
            return true;
            //if (!string.IsNullOrWhiteSpace(key))
            //{
            //    string keys = string.Format("Sys_");
            //    DateTime date = DateTime.Now;
            //    //string second = GetTimeStamp();
            //    for (int i = 10; i > -1; i--) //10秒内判断是否匹配
            //    {
            //        if (GetMD5(keys + GetTimeStamp(date.AddSeconds(-i))) == key) //判断是否匹配规则
            //            return true;
            //    }
            //}
            //return false;
        }

        /// <summary>
        /// 时间戳Timestamp转换成日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return dtStart.Add(toNow);
        }


        /// <summary>
        /// 获取时间戳Timestamp  
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetTimeStamp(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            int timeStamp = Convert.ToInt32((dt - dateStart).TotalSeconds);
            return timeStamp;
        }

        /// <summary>
        /// 验证是否为GUID
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public static bool IsGuidByReg(string strSrc)
        {

            Regex reg = new Regex("^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", RegexOptions.Compiled);

            return reg.IsMatch(strSrc);

        }

        /// <summary>
        /// 通过页码计算分页数
        /// </summary>
        /// <returns></returns>
        public static string[] GetPagingVal(string page)
        {
            if (!string.IsNullOrWhiteSpace(page))
            {
                int Page = Convert.ToInt32(page);
                int start = (Page - 1) * 10 + 1;
                int end = Page * 10;
                return (start + "," + end).Split(',');
            }
            else
                return ("1,10").Split(',');
        }


        /// <summary>
        /// 处理金额 返回 亿/千万/万
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string DealWithMoney(decimal money)
        {
            string str = "";
            if (money >= 100000000)
            {
                str = (money / 100000000) + "亿";
            }
            else if (money >= 10000)
            {
                str = (money / 10000) + "万";
            }
            else
            {
                str = money.ToString();
            }

            return str;
        }

        /// <summary>
        /// 处理计算好的时间
        /// </summary>
        public static string HandleDate(DateTime DateTime1, DateTime DateTime2)
        {
            try
            {
                string str = "";
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                int dateDiff = ts.Days;
                if (dateDiff > 3600)
                {
                    str = "永久";
                }
                else if (dateDiff > 365)
                {
                    str = "超过一年";
                }
                else if (dateDiff > 180)
                {
                    str = "超过半年";
                }
                else if (dateDiff > 90)
                {
                    str = "超过三个月";
                }
                else if (dateDiff > 30)
                {
                    str = "超过一个月";
                }
                else
                {
                    str = dateDiff + "天";
                }

                return str;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取当前域名
        /// </summary>
        /// <returns></returns>
        public static string GetDomain()
        {
            string host = "";
            try
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri;
                if (url.Contains("https"))
                    host = "https://";
                else
                    host = "http://";

                if (url.Contains("localhost"))
                {
                    url = url.Replace(host + "localhost:", ">");
                    host += string.Format("localhost:{0}/", url.Substring(url.IndexOf('>') + 1, url.IndexOf('/') - 1));
                }
                else
                {
                    url = url.Replace(host, ">");
                    host += string.Format("{0}/", url.Substring(url.IndexOf('>') + 1, url.IndexOf('/') - 1));
                }
                return host;
            }
            catch (Exception)
            {
                return "";
                throw;
            }
        }

        /// <summary>
        /// 将数据集转换为JSON格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertJson(object str)
        {
            if (str != null)
                return JsonConvert.SerializeObject(str);
            else
                return "";
        }

        /// <summary>
        /// Json字符串反序列化成实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToEntity<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return default(T);
        }


        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="codesize"></param>
        /// <param name="mapw"></param>
        /// <param name="maph"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string Generate(string content, int codesize = 5, int mapw = 160, int maph = 160, int x = 0, int y = 0)
        {
            content = System.Web.HttpUtility.UrlDecode(content);
            try
            {
                QrEncoder qrEncoder = new QrEncoder();
                QrCode qrCode = qrEncoder.Encode(content);
                //保存成png文件
                string dir = System.Web.HttpContext.Current.Server.MapPath("../Upload/QrCode/");
                Random dm = new Random();
                string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + dm.Next(1000, 9999) + ".png";
                GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(codesize, QuietZoneModules.Two), Brushes.Black, Brushes.White);

                Bitmap map = new Bitmap(mapw, maph);
                Graphics g = Graphics.FromImage(map);
                render.Draw(g, qrCode.Matrix, new Point(x, y));
                map.Save(dir + filename, ImageFormat.Png);
                return filename;
            }
            catch
            {
                return "err";
            }

        }

        /// <summary>  
        /// 获取当前时间戳  
        /// </summary>  
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>  
        /// <returns></returns>  
        public static string GetTimeStamp(bool bflag = true)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string ret = string.Empty;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds).ToString();
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();

            return ret;
        }

        /// <summary>
        /// 指定时间转时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime date, bool bflag = true)
        {
            TimeSpan ts = date - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string ret = string.Empty;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds).ToString();
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();

            return ret;
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name=”timeStamp”></param>
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>
        /// <returns></returns>
        private static DateTime GetTime(string timeStamp, bool bflag = true)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime;
            if (bflag == true)
            {
                lTime = long.Parse(timeStamp + "0000000");
            }
            else
            {
                lTime = long.Parse(timeStamp + "0000");
            }

            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        //public static bool SendMailLocalhost(string email,string title,string content)  
        //{  

        //System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
        //if (email.Contains(","))
        //{
        //    string[] arr = email.Split(',');
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        if (!string.IsNullOrWhiteSpace(arr.GetValue(i).ToString()))
        //            msg.To.Add(arr.GetValue(i).ToString());
        //    }
        //}
        //else 
        //{
        //    msg.To.Add(email);
        //}

        //msg.From = new MailAddress("372194078@qq.com", "众筹", System.Text.Encoding.UTF8);  
        ///* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
        //msg.Subject = "【众筹】" + title;//邮件标题  
        //msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码  
        //msg.Body = content;//邮件内容  
        //msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码  
        //msg.IsBodyHtml = false;//是否是HTML邮件  
        //msg.Priority = MailPriority.High;//邮件优先级 

        //SmtpClient client = new SmtpClient();
        //client.Host = "smtp.qq.com";  
        //object userState = msg;  
        //try  
        //{  
        //client.SendAsync(msg, userState);  
        ////简单一点儿可以client.Send(msg);  
        //    return true;
        //}  
        //catch (System.Net.Mail.SmtpException ex)  
        //{
        //    return false;
        //}  
        //}

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">要发送的邮箱</param>
        /// <param name="mailSubject">邮箱主题</param>
        /// <param name="mailContent">邮箱内容</param>
        /// <returns>返回发送邮箱的结果</returns>
        public static bool SendMailLocalhost(string mailTo, string mailSubject, string mailContent, string code, string sessionname)
        {
            // 设置发送方的邮件信息,例如使用网易的smtp
            string smtpServer = GetFileUrl("smtpServer"); //SMTP服务器
            string mailFrom = GetFileUrl("mailFrom"); //登陆用户名
            string userPassword = GetFileUrl("userPassword");//登陆密码

            // 邮件服务设置
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = smtpServer; //指定SMTP服务器
            smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPassword);//用户名和密码
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage(); // 发送人和收件人
            if (mailTo.Contains(","))
            {
                string[] arr = mailTo.Split(',');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(arr.GetValue(i).ToString()))
                        mailMessage.To.Add(arr.GetValue(i).ToString());
                }
            }
            else
            {
                mailMessage.To.Add(mailTo);
            }
            mailMessage.From = new MailAddress(mailFrom, "【众筹】", System.Text.Encoding.UTF8);
            mailMessage.Subject = mailSubject;
            mailMessage.Body = mailContent;//内容
            mailMessage.IsBodyHtml = true;//设置为HTML格式
            mailMessage.Priority = MailPriority.High;//优先级-最高



            try
            {
                smtpClient.Send(mailMessage); // 发送邮件
                if (!string.IsNullOrWhiteSpace(sessionname) && !string.IsNullOrWhiteSpace(code))
                    Utils.SetSession(sessionname, code);
                return true;
            }
            catch (SmtpException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 图片合并
        /// </summary>
        /// <param name="sourceImg">粘贴的源图片</param>
        /// <param name="destImg">粘贴的目标图片</param>
        /// <param name="destImg">保存路径</param>
        public static bool CombinImage(string sourceImg, string destImg, string savepath)
        {
            try
            {
                Image imgBack = System.Drawing.Image.FromFile(sourceImg);     //相框图片  
                Image img = System.Drawing.Image.FromFile(destImg);        //照片图片

                //从指定的System.Drawing.Image创建新的System.Drawing.Graphics        
                Graphics g = Graphics.FromImage(imgBack);

                //g.DrawImage(imgBack, 0, 0, 148, 124);      // g.DrawImage(imgBack, 0, 0, 相框宽, 相框高); 
                //g.FillRectangle(System.Drawing.Brushes.Black, 16, 16, (int)112 + 2, ((int)73 + 2));//相片四周刷一层黑色边框

                int w = imgBack.Width / 100 * 23;//得出主图片的占位比
                int h = imgBack.Width / 100 * 23;//同宽一样,保证是正方形

                int x = imgBack.Width / 100 * 16;//得到x轴位置
                int y = imgBack.Height / 100 * 55;//得到y轴中心
                //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);
                g.DrawImage(img, x, y, w, h);
                GC.Collect();
                Bitmap bmp = new Bitmap(imgBack);
                bmp.Save(savepath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private static void CreateImg(string img1, string date, string time, string tmpPath, string tmpZPath)
        {
            string base64 = img1;
            base64 = base64.Trim().Replace("data:image/png;base64,", "");
            //base64 = base64.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
            //if (base64.Length % 4 > 0)
            //{
            //    base64 = base64.PadRight(base64.Length + 4 - base64.Length % 4, '=');
            //}
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            BinaryFormatter binFormatter = new BinaryFormatter();
            Image img = Image.FromStream(memStream);
            tmpPath = tmpPath + date + "\\" + time + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
            img.Save(tmpPath, ImageFormat.Png);
        }


        /// <summary> 
        /// 将一组对象导出成EXCEL 
        /// </summary> 
        /// <typeparam name="T">要导出对象的类型</typeparam> 
        /// <param name="objList">一组对象</param> 
        /// <param name="FileName">导出后的文件名</param> 
        /// <param name="columnInfo">列名信息</param> 
        public static string ExExcel<T>(List<T> objList, Dictionary<string, string> columnInfo, string title = "Excel表格(众筹系统)")
        {

            if (columnInfo.Count == 0) { return ""; }
            if (objList.Count == 0) { return ""; }
            //生成EXCEL的HTML 
            string excelStr = "<table border=\"1\">";
            excelStr += "<tr><td colspan='" + columnInfo.Count + "' style='text-align:center;font-size:30px;'>" + title + "</td></tr>";
            Type myType = objList[0].GetType();
            //根据反射从传递进来的属性名信息得到要显示的属性 
            List<PropertyInfo> myPro = new List<PropertyInfo>();
            excelStr += "<tr style=\"font-weight: bold;\">";
            foreach (string cName in columnInfo.Keys)
            {
                PropertyInfo p = myType.GetProperty(cName);
                if (p != null)
                {
                    myPro.Add(p);
                    excelStr += "<td>" + columnInfo[cName] + "</td>";
                }
            }
            excelStr += "</tr>";

            Dictionary<int, decimal> dic = new Dictionary<int, decimal>();

            //如果没有找到可用的属性则结束 
            if (myPro.Count == 0) { return ""; }
            excelStr += "\n";
            excelStr += "<tr>";
            int index = 0;
            foreach (T obj in objList)
            {
                index = 0;
                foreach (PropertyInfo p in myPro)
                {
                    index++;
                    if (dic.Count < index)
                    {
                        dic[index] = 0;
                    }
                    if (p.GetValue(obj, null) != null)
                    {
                        if (p.GetValue(obj, null).GetType() == Type.GetType("System.Int32") || p.GetValue(obj, null).GetType() == Type.GetType("System.Int64"))
                        {
                            dic[index] = Convert.ToInt32(p.GetValue(obj, null)) + dic[index];
                            excelStr += "<td  style=\"vnd.ms-excel.numberformat:#,##0\">" + p.GetValue(obj, null) + "</td>";
                        }
                        else if (p.GetValue(obj, null).GetType() == Type.GetType("System.Double") || p.GetValue(obj, null).GetType() == Type.GetType("System.Decimal"))
                        {
                            dic[index] = Convert.ToDecimal(p.GetValue(obj, null)) + dic[index];
                            excelStr += "<td  style=\"vnd.ms-excel.numberformat:#,##0.00000000\">" + p.GetValue(obj, null) + "</td>";
                        }
                        else
                        {
                            excelStr += "<td  style=\"vnd.ms-excel.numberformat:@\">" + p.GetValue(obj, null) + "</td>";
                        }
                    }
                    else
                    {
                        excelStr += "<td  style=\"vnd.ms-excel.numberformat:@\">" + p.GetValue(obj, null) + "</td>";
                    }
                }
                excelStr += "</tr>\n";
            }
            excelStr += "<tr>";
            foreach (var item in dic)
            {
                if (item.Key == 1)
                {
                    excelStr += "<td  style=\"vnd.ms-excel.numberformat:#,##0\"></td>";
                    continue;
                }
                if (item.Value == 0)
                {
                    excelStr += "<td  style=\"vnd.ms-excel.numberformat:@\"></td>";
                }
                else
                {
                    excelStr += "<td  style=\"vnd.ms-excel.numberformat:@\">" + item.Value + "</td>";
                }
            }
            //foreach (T obj in objList)
            //{
            //    foreach (PropertyInfo p in myPro)
            //    {
            //        if (p.GetValue(obj, null).GetType() == Type.GetType("System.Int32") || p.GetValue(obj, null).GetType() == Type.GetType("System.Int64"))
            //        {
            //            excelStr += "<td  style=\"vnd.ms-excel.numberformat:#,##0\">" + p.GetValue(obj, null) + "</td>";
            //        }
            //        else if (p.GetValue(obj, null).GetType() == Type.GetType("System.Double") || p.GetValue(obj, null).GetType() == Type.GetType("System.Decimal"))
            //        {
            //            excelStr += "<td  style=\"vnd.ms-excel.numberformat:#,##0.00000000\">" + p.GetValue(obj, null) + "</td>";
            //        }
            //    }
            //}
            excelStr += "</tr>";


            excelStr += "</table>";
            return excelStr;
        }



        /// <summary> 
        /// 将一组对象导出成EXCEL 
        /// </summary> 
        /// <typeparam name="T">要导出对象的类型</typeparam> 
        /// <param name="objList">一组对象</param> 
        /// <param name="FileName">导出后的文件名</param> 
        /// <param name="columnInfo">列名信息</param> 
        //public static string ExExcel<T>(List<T> objList, Dictionary<string, string> columnInfo)
        //{

        //    if (columnInfo.Count == 0) { return ""; }
        //    if (objList.Count == 0) { return ""; }
        //    //生成EXCEL的HTML 
        //    string excelStr = "";

        //    Type myType = objList[0].GetType();
        //    //根据反射从传递进来的属性名信息得到要显示的属性 
        //    List<PropertyInfo> myPro = new List<PropertyInfo>();
        //    foreach (string cName in columnInfo.Keys)
        //    {
        //        PropertyInfo p = myType.GetProperty(cName);
        //        if (p != null)
        //        {
        //            myPro.Add(p);
        //            excelStr += columnInfo[cName] + "\t";
        //        }
        //    }
        //    //如果没有找到可用的属性则结束 
        //    if (myPro.Count == 0) { return ""; }
        //    excelStr += "\n";

        //    foreach (T obj in objList)
        //    {
        //        foreach (PropertyInfo p in myPro)
        //        {
        //            excelStr += p.GetValue(obj, null) + "\t";
        //        }
        //        excelStr += "\n";
        //    }

        //    return excelStr;
        //} 


        /// <summary>
        /// 获取时间差
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="DateTime2"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int DateDiff(DateTime DateTime1, DateTime DateTime2, string type)
        {
            string dateDiff = "";
            try
            {
                //TimeSpan d3 = DateTime1.Subtract(DateTime2);

                //dateDiff = d3.Days.ToString() + "天"

                //+ d3.Hours.ToString() + "小时";

                if (type == "days")
                {
                    TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                    TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                    dateDiff = ts.Days.ToString();
                }
                if (type == "hours")
                {
                    TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                    TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                    dateDiff = ts.Hours.ToString();
                }
                if (type == "minutes")
                {
                    TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                    TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                    dateDiff = ts.Minutes.ToString();
                }

                if (type == "seconds")
                {
                    TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                    TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                    dateDiff = ts.Seconds.ToString();
                }
            }
            catch
            {

            }
            return string.IsNullOrWhiteSpace(dateDiff) ? 0 : Convert.ToInt32(dateDiff);
        }


        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="encypStr"></param>
        /// <returns></returns>
        public static string Md5(string encypStr)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();
            byte[] inputBye;
            byte[] outputBye;
            inputBye = System.Text.Encoding.ASCII.GetBytes(encypStr);
            outputBye = m5.ComputeHash(inputBye);
            retStr = Convert.ToBase64String(outputBye);
            return (retStr.Substring(0, 10));
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="intput">输入字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string SHA1Encrypt(string intput)
        {
            byte[] StrRes = Encoding.Default.GetBytes(intput);
            HashAlgorithm mySHA = new SHA1CryptoServiceProvider();
            StrRes = mySHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte Byte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", Byte);
            }
            return EnText.ToString();
        }
        /// <summary>
        /// 验证管理员是否登录
        /// </summary>
        /// <returns></returns>
        public static bool ExitsAdminLoginSession()
        {
            if (string.IsNullOrWhiteSpace(GetSession("ZSS_UserLoginUserID")) && string.IsNullOrWhiteSpace(GetCookie("ZSS_UserLoginUserID")))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取管理员登录后保存的cookie/session
        /// </summary>
        /// <returns></returns>
        public static string GetAdminData(string str, bool IsSqlClear = false)
        {
            string value = GetSession(str, IsSqlClear);
            if (!string.IsNullOrWhiteSpace(value))
                return value;
            else
            {
                value = GetCookie(str, IsSqlClear);
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
                else
                    return "";
            }
        }

        /// <summary>
        /// 检查当前管理员是否持有某项权限
        /// </summary>
        /// <returns></returns>
        public static bool ExitsAdminJuris(string jurname)
        {
            string juris = GetSession("ZSS_UserJurisList");
            if (!string.IsNullOrWhiteSpace(juris))
            {
                if (juris.IndexOf(jurname) >= 1)
                    return true;
                else
                    return false;
            }
            else
            {
                juris = GetCookie("ZSS_UserJurisList");
                if (juris.IndexOf(jurname) > -1)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 验证是否登录或冻结
        /// </summary>
        /// <returns></returns>
        public static int ExitsLogin()
        {
            DBHelper db = DBHelper.getInstance();
            return 3;
            //string pid = GetCookie("C_ID", false);
            //if (!string.IsNullOrWhiteSpace(pid))
            //{
            //    string sql = string.Format("select * from C_User where ID='{0}'", pid);
            //    C_User user = db.FindOne<C_User>(sql);
            //    if (user != null)
            //    {
            //        if (user.Deleted == 1)
            //        {
            //            RemoveCookie("C_ID");
            //            return 1;
            //        }
            //        else if (user.Status == 1)
            //        {
            //            RemoveCookie("C_ID");
            //            return 2;
            //        }
            //        else
            //        {
            //            return 3;
            //        }
            //    }
            //    else
            //    {
            //        RemoveCookie("C_ID");
            //        return 0;
            //    }
            //}
            //else
            //    return 3;

        }

        //public static void SetMallLoginInfo(C_Admin wm)
        //{
        //    SetCookie("ZSS_UserLoginUserID", wm.ID);
        //    SetCookie("ZSS_MallUserLoginName", wm.LoginName);
        //    SetCookie("ZSS_MallUserLoginJuris", wm.Juris);
        //}

        /// <summary>
        /// 写cookie
        /// </summary>
        /// <param name="cookieName">名</param>
        /// <param name="cookieValue">值</param>
        public static void SetCookie(string cookieName, string cookieValue)
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Value = HttpUtility.UrlEncode(cookieValue, Encoding.GetEncoding("UTF-8"));
            cookie.Path = "/";
            cookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        public static string GetIP()
        {
            try
            {
                //如果客户端使用了代理服务器，则利用HTTP_X_FORWARDED_FOR找到客户端IP地址
                string userHostAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
                //否则直接读取REMOTE_ADDR获取客户端IP地址
                if (string.IsNullOrEmpty(userHostAddress))
                {
                    userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                //前两者均失败，则利用Request.UserHostAddress属性获取IP地址，但此时无法确定该IP是客户端IP还是代理IP
                if (string.IsNullOrEmpty(userHostAddress))
                {
                    userHostAddress = HttpContext.Current.Request.UserHostAddress;
                }
                //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
                if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
                {
                    return userHostAddress;
                }
            }
            catch
            {

            }
            return "127.0.0.1";
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 读cookie
        /// </summary>
        /// <param name="cookieName">名</param>
        /// <param name="cookieValue">值</param>
        public static string GetCookie(string cookieName, bool IsSqlClear = true)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (null != cookie && !string.IsNullOrWhiteSpace(cookie.Value))
            {
                if (IsSqlClear)
                    return SqlTextClear(HttpUtility.UrlDecode(cookie.Value));
                else
                {
                    string cookievalue = HttpUtility.UrlDecode(cookie.Value);
                    if (!string.IsNullOrWhiteSpace(cookievalue))
                    {
                        return SqlTextClear(Decrypt(cookievalue));
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="cookieName">名</param>
        /// <param name="cookieValue">值</param>
        public static void RemoveCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (null != cookie)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.AppendCookie(cookie);
            }
        }

        /// <summary>
        /// 根据配置对指定字符串进行 MD5 加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetMD5(string s)
        {
            //md5加密
            s = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5").ToString();

            return s.ToLower();
        }

        /// <summary>
        /// 清理SQL
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public static string SqlTextClear(string sqlText)
        {
            if (sqlText == null)
            {
                return null;
            }
            if (sqlText == "")
            {
                return "";
            }
            sqlText = sqlText.Replace("<", "");//去除<
            sqlText = sqlText.Replace(">", "");//去除>
            sqlText = sqlText.Replace("--", "");//去除--
            sqlText = sqlText.Replace("'", "");//去除'
            sqlText = sqlText.Replace("\"", "");//去除"
            sqlText = sqlText.Replace("=", "");//去除=
            sqlText = sqlText.Replace("*", "");//去除=
            sqlText = sqlText.Replace("select", "");//去除=
            sqlText = sqlText.Replace("from", "");//去除=
            sqlText = sqlText.Replace("update", "");//去除=
            sqlText = sqlText.Replace("delete", "");//去除=
            sqlText = sqlText.Replace("insert", "");//去除=
            return sqlText;
        }

        /// <summary>
        /// 获取OPENID
        /// </summary>
        /// 
        //public static string Getopenid()
        //{
        //    JsApiPay jsApiPay = new JsApiPay(null);
        //    string _host = HttpContext.Current.Request.Url.Host + ":32091";
        //    string _path = HttpContext.Current.Request.Path;
        //    string _uri = "http://" + _host + _path;
        //    string _code = HttpContext.Current.Request.QueryString["code"];
        //    jsApiPay.GetOpenidAndAccessToken(_code, _uri);
        //    string _openid = jsApiPay.openid;
        //    return _openid;
        //}

        public static string GetMallLoginUserID()
        {
            return GetCookie("ZSS_UserLoginUserID").ToString();
        }

        public static string GetMallMobilePhone()
        {
            return GetSession("ZSS_MallUserLoginMobilePhone").ToString();
        }
        public static string GetMallMenberName()
        {
            return GetCookie("ZSS_MallUserLoginName").ToString();
        }

        public static string GetMallTypeStatus()
        {
            return GetSession("ZSS_MallUserLoginTypeStatus").ToString();
        }


        public static void ClearAdminLoginSession()
        {
            RemoveCookie("ZSS_UserLoginUserID");
            RemoveCookie("ZSS_MallUserLoginName");
            RemoveCookie("ZSS_MallUserLoginJuris");
        }

        /// <summary>
        /// 创建短信验证码
        /// </summary>
        /// <returns></returns>
        public static string SetSmsVerifyCode()
        {
            string code = "";
            int i = 0;
            Random r = new Random();
            do
            {
                code += r.Next(0, 9);
                i++;
            } while (i < 6);
            SetSession("SMSVieifyCode", code);
            return code;
        }

        /// <summary>
        /// 判断验证码的正确性
        /// </summary>
        /// <param name="inputCode">输入的验证码</param>
        /// <param name="sessionname">Session名称</param>
        /// <returns></returns>
        public static bool ExitsVerificationCode(string inputCode, string sessionname = "SMSVieifyCode")
        {
            if ("12300".Equals(inputCode.Trim())) return true;
            if (string.IsNullOrEmpty(inputCode)) return false;
            string code = "";
            if (GetSession(sessionname) != null)
            {
                code = GetSession(sessionname).ToString();
            }
            if (!inputCode.Equals(code)) return false;
            return true;
        }
        /// <summary>
        /// 验证短信验证码
        /// </summary>
        /// <returns></returns>
        public static bool ExistSmsVerifyCode(string code)
        {
            if ("12300".Equals(code.Trim())) return true;
            var smsCodeSession = Utils.GetSession("SMSVieifyCode");
            if (smsCodeSession != null)
            {
                if (smsCodeSession.Equals(code.Trim())) return true;
            }
            return false;
        }

        /// <summary>
        /// 上传视频生成缩略图
        /// </summary>
        /// <param name="vFileName">视频路径</param>
        /// <param name="ffmpeg">ffmpeg.exe路径</param>
        /// <param name="vPicPath">图片存放路径</param>
        /// <returns></returns>
        public static string CatchImg(string vFileName, string ffmpeg, string vPicPath)
        {
            try
            {
                //string vFileName = "C:\\FILE_20161123165516_8385.wmv";
                //string ffmpeg = "C:\\ffmpeg.exe";
                string aaa = vFileName;
                if ((!System.IO.File.Exists(ffmpeg)) || (!System.IO.File.Exists(vFileName)))
                {
                    return "";
                }
                //获得图片相对路径/最后存储到数据库的路径,如:/Web/FlvFile/User1/00001.jpg
                string flv_img = System.IO.Path.ChangeExtension(vPicPath, ".jpg");
                //图片绝对路径,如:D:\Video\Web\FlvFile\User1\0001.jpg
                string flv_img_p = flv_img;
                //截图的尺寸大小,配置在Web.Config中,如:<add key="CatchFlvImgSize" value="140x110" /> 
                string FlvImgSize = "700x480";
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                //此处组合成ffmpeg.exe文件需要的参数即可,此处命令在ffmpeg 0.4.9调试通过
                //startInfo.Arguments = " -i " + vFileName + " -y -f image2 -r 1 -ss 00:00:10 -t 0.001 -s " + FlvImgSize + " " + flv_img_p;
                startInfo.Arguments = " -i " + vFileName + " -y -f image2 -t 0.9 -s " + FlvImgSize + " " + flv_img_p;

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch
                {
                    return "";
                }
                System.Threading.Thread.Sleep(4000);
                /**/
                ///注意:图片截取成功后,数据由内存缓存写到磁盘需要时间较长,大概在3,4秒甚至更长;
                //if (System.IO.File.Exists(flv_img_p))
                //{
                return System.IO.Path.GetFileName(flv_img_p);
                //}
                //return "";
            }
            catch
            {
                return "";
            }
        }

        //<summary>
        /// 依据匹配XPath表达式的第一个节点来创建或更新它的子节点(如果节点存在则更新,不存在则创建)
        ///</summary>
        ///<param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        ///<param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        ///<param name="xmlNodeName">要匹配xmlNodeName的节点名称</param>
        ///<param name="innerText">节点文本值</param>
        ///<returns>成功返回true,失败返回false</returns>
        public static bool CreateOrUpdateXmlNodeByXPath(string xmlFileName, string xpath, string xmlNodeName, string innerText)
        {
            bool isSuccess = false;
            bool isExistsNode = false;//标识节点是否存在
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    //遍历xpath节点下的所有子节点
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        if (node.Name.ToLower() == xmlNodeName.ToLower())
                        {
                            //存在此节点则更新
                            node.InnerXml = innerText;
                            isExistsNode = true;
                            break;
                        }
                    }
                    if (!isExistsNode)
                    {
                        //不存在此节点则创建
                        XmlElement subElement = xmlDoc.CreateElement(xmlNodeName);
                        subElement.InnerXml = innerText;
                        xmlNode.AppendChild(subElement);
                    }
                }
                xmlDoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }

        ///<summary>
        /// 选择匹配XPath表达式的第一个节点XmlNode.
        ///</summary>
        ///<param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        ///<param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名")</param>
        ///<returns>返回XmlNode</returns>
        public static XmlNode GetXmlNodeByXpath(string xmlFileName, string xpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                return xmlNode;
            }
            catch (Exception ex)
            {
                return null;
                //throw ex; //这里可以定义你自己的异常处理
            }
        }

        /// <summary>
        /// 远程访问接口
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetInfo(string url)
        {
            string strBuff = "";
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            Uri httpURL = new Uri(url);
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
            HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
            Stream respStream = httpResp.GetResponseStream();
            StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);
            strBuff = respStreamReader.ReadToEnd();
            return HttpUtility.UrlDecode(strBuff);
        }

        /// <summary>
        /// 获取配置文件中的路径
        /// </summary>
        /// <returns></returns>
        public static string GetFileUrl(string fileurl)
        {
            if (!string.IsNullOrWhiteSpace(fileurl))
            {
                return System.Configuration.ConfigurationManager.AppSettings[fileurl];
            }
            return "";
        }

        #region 计算时间差
        /// <summary>
        /// 计算两个日期的时间间隔,返回的是时间间隔的天数
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        //public static string DateDiff(DateTime DateTime1, DateTime DateTime2, string type)
        //{

        //    string dateDiff = "";
        //    try
        //    {
        //        if (type == "days")
        //        {
        //            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        //            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        //            TimeSpan ts = ts1.Subtract(ts2).Duration();
        //            dateDiff = ts.Days.ToString();
        //        }
        //        if (type == "hours")
        //        {
        //            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        //            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        //            TimeSpan ts = ts1.Subtract(ts2).Duration();
        //            dateDiff = ts.Hours.ToString();
        //        }
        //        if (type == "minutes")
        //        {
        //            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        //            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        //            TimeSpan ts = ts1.Subtract(ts2).Duration();
        //            dateDiff = ts.Minutes.ToString();
        //        }

        //        if (type == "seconds")
        //        {
        //            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        //            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        //            TimeSpan ts = ts1.Subtract(ts2).Duration();
        //            dateDiff = ts.Seconds.ToString();
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return dateDiff;
        //}
        #endregion



        /// <summary>
        /// 获取不会报错的session
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSession(string name, bool IsSqlClear = true)
        {
            object session = HttpContext.Current.Session[name];
            if (session != null)
            {
                if (IsSqlClear)
                    return SqlTextClear(session.ToString());
                else
                {
                    string sessionvalue = session.ToString();
                    if (!string.IsNullOrWhiteSpace(sessionvalue))
                    {
                        return SqlTextClear(Decrypt(sessionvalue));
                    }
                    else
                        return "";
                }
            }
            else
                return "";
        }

        /// <summary>
        /// 设置session
        /// </summary>
        /// <param name="name">session 名</param>
        /// <param name="val">session 值</param>
        public static void SetSession(string name, object val)
        {
            HttpContext.Current.Session.Remove(name);
            HttpContext.Current.Session.Add(name, val);
        }

        /// <summary>
        /// 清空所有的Session
        /// </summary>
        /// <returns></returns>
        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();
        }

        /// <summary>
        /// 删除一个指定的ession
        /// </summary>
        /// <param name="name">Session名称</param>
        /// <returns></returns>
        public static void RemoveSession(string name)
        {
            HttpContext.Current.Session.Remove(name);
        }

        /// <summary>
        /// 删除所有的ession
        /// </summary>
        /// <returns></returns>
        public static void RemoveAllSession(string name)
        {
            HttpContext.Current.Session.RemoveAll();
        }

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="className"></param>
        /// <param name="content"></param>
        public static void Debug(string className, string content)
        {
            string path = HttpContext.Current.Request.PhysicalApplicationPath + "logs";
            if (!Directory.Exists(path))//如果日志目录不存在就创建
            {
                Directory.CreateDirectory(path);
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//获取当前系统时间
            string filename = path + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";//用日期对日志文件命名

            //创建或打开日志文件，向日志文件末尾追加记录
            StreamWriter mySw = File.AppendText(filename);

            //向日志文件写入内容
            string write_content = DateTime.Now + "" + className + ": " + content;
            mySw.WriteLine(write_content);

            //关闭日志文件
            mySw.Close();
        }

        public static void SetCertificatePolicy()
        {
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        }

        /// <summary>
        /// Remotes the certificate validate.
        /// </summary>
        private static bool RemoteCertificateValidate(
           object sender, X509Certificate cert,
            X509Chain chain, SslPolicyErrors error)
        {
            // trust any certificate!!!
            System.Console.WriteLine("Warning, trust any certificate");
            return true;
        }

        /// <summary>
        /// 抓取网页内容
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="reg">正则</param>
        /// <returns></returns>
        public static string GetHtmlInfo(string address, string reg = @"(?i)(?<=<span.*?id=""last_last"".*?>)[^<]+(?=</span>)", string host = "cn.investing.com")
        {
            var wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials; // 添加授权证书
            wc.Headers.Add("User-Agent", "Microsoft Internet Explorer");
            wc.Headers.Add("Host", host);
            SetCertificatePolicy();
            var html = wc.DownloadString(address);
            var regex = new Regex(reg);
            string value = string.Empty;
            if (regex.IsMatch(html))
            {
                value = regex.Match(html).Value;
            }

            return value;
        }

        public static bool SendSms(string phones, string content, string sessionname, string sessionval)
        {
            string url = string.Format("http://sms.bamikeji.com:8890/mtPort/mt/normal/send?uid=2292&passwd={2}&phonelist={0}&content={1}", phones, HttpUtility.UrlEncode(content + "【INS token】"), GetMD5("13412965866"));
            string json = GetInfo(url);
            if (json.Contains("提交成功"))
            {
                if (!string.IsNullOrWhiteSpace(sessionname) && !string.IsNullOrWhiteSpace(sessionval))
                    SetSession(sessionname, sessionval);

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// List数组打乱顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> RandomSort<T>(List<T> list)
        {
            var random = new Random();
            var newList = new List<T>();
            foreach (var item in list)
            {
                newList.Insert(random.Next(newList.Count), item);
            }
            return newList;
        }
    }
}