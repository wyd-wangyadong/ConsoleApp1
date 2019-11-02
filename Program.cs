using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using Ninject.Activation;
using System.Web;
using System.IO;
using System.Xml;

namespace ConsoleApp1
{
    class Program
    {
        public string Post(string methodName, string jsonParas)
        {
            string strURL = "http://192.168.43.136:8088/WebService1.asmx" + "/" + methodName;

            //创建一个HTTP请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            //Post请求方式  
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/x-www-form-urlencoded";

            //设置参数，并进行URL编码  
            //虽然我们需要传递给服务器端的实际参数是JsonParas(格式：[{\"UserID\":\"0206001\",\"UserName\":\"ceshi\"}])，
            //但是需要将该字符串参数构造成键值对的形式（注："paramaters=[{\"UserID\":\"0206001\",\"UserName\":\"ceshi\"}]"），
            //其中键paramaters为WebService接口函数的参数名，值为经过序列化的Json数据字符串
            //最后将字符串参数进行Url编码
            string paraUrlCoded = System.Web.HttpUtility.UrlEncode("paramaters");
            paraUrlCoded += "=" + System.Web.HttpUtility.UrlEncode(jsonParas);

            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流  

            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception)
            {
                writer = null;
                Console.Write("连接服务器失败!");
            }
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            writer.Close();//关闭请求流

            //String strValue = "";//strValue为http响应所返回的字符流
            String strValue;//strValue为http响应所返回的字符流
            HttpWebResponse response;
            try
            {
                //获得响应流
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            Stream s = response.GetResponseStream();//获取用于响应请求数据的Stream对象

            //服务器端返回的是一个XML格式的字符串，XML的Content才是我们所需要的Json数据
            XmlTextReader Reader = new XmlTextReader(s);
            Reader.MoveToContent();
            strValue = Reader.ReadInnerXml();//取出Content中的Json数据
            Reader.Close();
            s.Close();

            return strValue;//返回Json数据
        }
        static void Main(string[] args)
        {
            Program p = new Program();
           
            Console.WriteLine(p.Post("HelloWorld", "{\"workOrderCode\":\"110\",\"serialNum\":\"11\"}"));
            Console.WriteLine(p.Post("Project", "{\"workOrderCode\":\"110\",\"serialNum\":\"11\"}"));

            Console.ReadKey(); 
        }
    }
}
