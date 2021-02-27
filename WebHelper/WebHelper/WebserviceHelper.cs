using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebHelper
{
    class WebServiceHelper
    {
        /// <summary>
        /// CreateBy： redAnt
        /// Time:2021年2月2日10:22:21
        /// 说明：使用http的post方式调用webservice
        /// </summary>
        /// <param name="url">接口地址（含接口方法）</param>
        /// <param name="body">入参体，由于webservice默认使用application/x-www-form-urlencoded，需要加码UrlEncode
        /// 总体结构为：inparam = string/int/...</param>
        /// <param name="isReturnHeader">是否包含信息头，返回的信息默认带saop头，默认不显示</param>
        /// <returns>返回出参</returns>
        public string httpPostWebService(string url, string body, bool isReturnHeader = false)
        {
            Stream writer = null;
            Stream ResponseStream = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            StreamReader sReader = null;
            try
            {
                string result;
                //组织信息头
                Encoding encoding = Encoding.UTF8;
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Accept = "text/html,text/html, application/xhtml+xml, */*";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] buffer = encoding.GetBytes(body);
                request.ContentLength = buffer.Length;
                //尝试调用
                try
                {
                    writer = request.GetRequestStream();  //获取用于写入请求数据的Stream对象
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                writer.Write(buffer, 0, buffer.Length);  //把参数数据写入请求数据流
                writer.Close();
                //尝试获取响应
                try
                {
                    response = (HttpWebResponse)request.GetResponse();  //获得响应
                }
                catch (WebException ex)
                {
                    return ex.Message;
                }
                ResponseStream = response.GetResponseStream();
                //如果需要返回头，则返回(原汁原味，不要做xmlreader处理）
                if (isReturnHeader == true)
                {
                    sReader = new StreamReader(ResponseStream, Encoding.UTF8);
                    result = sReader.ReadToEnd();
                }
                else //否则直接返回内部xml
                {
                    XmlTextReader XMLreader = new XmlTextReader(ResponseStream);
                    XMLreader.MoveToContent();
                    result = XMLreader.ReadInnerXml();
                    XMLreader.Dispose();
                    XMLreader.Close();
                }
                //返回的xml，如果有需要转义的字符便转义
                result = result.Replace("&lt;", "<").Replace("&gt;", ">");
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                //最终不管执行的如何，都释放连接
                response.Dispose();
                response.Close();
                ResponseStream.Dispose();
                ResponseStream.Close();
                writer.Dispose();
                writer.Close();
            }
        }

        /// <summary>
        /// CreateBy： redAnt
        /// Time:2021年2月2日11:47:15
        /// 说明：使用http的GET方式调用webservice(需要ws支持此方式)
        /// </summary>
        /// <param name="url">接口链接地址</param>
        /// <param name="body">入参，参考接口格式</param>
        /// <param name="isReturnHeader">是否含有信息头</param>
        /// <returns>出参</returns>
        public string httpGetWebService(string url, string body, bool isReturnHeader = false)
        {
            HttpWebRequest request = null;
            Stream ResponseStream = null;
            StreamReader sReader = null;
            string result;
            try
            {
                //信息头
                request = (HttpWebRequest)WebRequest.Create(url + "?" + body);//构造get形式的url
                request.Method = "GET";
                request.Accept = "text/html,text/html, application/xhtml+xml, */*";
                request.ContentType = "application/x-www-form-urlencoded";

                //尝试使用get方法请求连接
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    ResponseStream = response.GetResponseStream();
                }
                catch (WebException ex)
                {
                    return ex.Message;
                }
                if (isReturnHeader == true)
                {
                    sReader = new StreamReader(ResponseStream, Encoding.UTF8);
                    result = sReader.ReadToEnd();
                }
                else
                {
                    XmlTextReader XMLreader = new XmlTextReader(ResponseStream);
                    XMLreader.MoveToContent();
                    result = XMLreader.ReadInnerXml();
                    XMLreader.Dispose();
                    XMLreader.Close();
                }
                //返回的xml需要转义，具体原因未明
                result = result.Replace("&lt;", "<").Replace("&gt;", ">");
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
