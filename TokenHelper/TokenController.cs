using DataHelper;
using HOTApi.App_Start;
using HOTApi.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace HOTApi.Controllers
{
    public class TokenController : ApiController
    {
        //定义变量
        string LocalDBPath = HOTConfig.GetConfig().GetLocalDBConnetionString();
        SQLiteHelper site = new SQLiteHelper();

        public TokenController()
        {
            site.buildConnetionString(LocalDBPath);//读取本地数据库数据
        }


        /// <summary>
        /// 客户端调用此接口，用来获取token，创建token路由。
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        [Route("token")]
        [HttpGet]
        public HttpResponseMessage CreateToken(string userName,string passWord)
        {
            Log.AddTrack("TokenController.CreateToken()", "Begin");
            string return_code;
            string return_msg;
            try
            {
                JsonDocument InData = new JsonDocument();
                InData.add(new JsonElement("userName", userName));
                InData.add(new JsonElement("passWord", passWord));
                JObject JInData = (JObject)JsonConvert.DeserializeObject(InData.innerText);
                Log.AddTrack("TokenController.CreateToken()", "判断用户合法性...");
                int APIUser = isAPIUser(userName, passWord); //查询API用户信息
                if (APIUser != -1)
                {
                    Log.AddTrack("TokenController.CreateToken()", "准备生成token");
                    string token = TokenHelper.SetJwtEncode(JInData);//生成token
                    Log.AddTrack("TokenController.CreateToken()", "token生成完毕：" + token);
                    return_code = "SUCCESS";
                    return_msg = "生成token成功";
                    JsonDocument outParam = new JsonDocument();//出参部分
                    outParam.add(new JsonElement("return_code", return_code));
                    outParam.add(new JsonElement("return_msg", return_msg));
                    outParam.add(new JsonElement("Token", token));
                    outParam.add(new JsonElement("expires_in", HOTConfig.GetConfig().GetTokenTimeStamp()));
                    //写入数据库日志
                    writeTokenLog(APIUser, token);
                    //返回纯文本text/plain  ,返回json application/json  ,返回xml text/xml
                    HttpResponseMessage result = new HttpResponseMessage
                    {

                        Content = new StringContent
                        (
                            outParam.innerText,
                            Encoding.GetEncoding("UTF-8"),
                            "application/json"
                        )
                    };
                    Log.AddTrack("TokenController.CreateToken()", "End");
                    return result;
                }
                else
                {
                    return_code = "FAIL";
                    return_msg = "生成token失败：无效用户用与密码";
                    Log.AddError("TokenController.CreateToken()", "生成token失败：无效用户用与密码");
                    JsonDocument outParam = new JsonDocument();//出参部分
                    outParam.add(new JsonElement("return_code", return_code));
                    outParam.add(new JsonElement("return_msg", return_msg));
                    //返回纯文本text/plain  ,返回json application/json  ,返回xml text/xml
                    HttpResponseMessage result = new HttpResponseMessage
                    {

                        Content = new StringContent
                        (
                            outParam.innerText,
                            Encoding.GetEncoding("UTF-8"),
                            "application/json"
                        )
                    };
                    return result;
                }
               

            }
            catch (Exception ex)
            {
                return_code = "FAIL";
                return_msg = "获取token异常";
                JsonDocument outParam = new JsonDocument();//出参部分
                outParam.add(new JsonElement("return_code", return_code));
                outParam.add(new JsonElement("return_msg", return_msg + " :" + ex.Message));
                //返回纯文本text/plain  ,返回json application/json  ,返回xml text/xml
                HttpResponseMessage result = new HttpResponseMessage
                {

                    Content = new StringContent
                    (
                        outParam.innerText,
                        Encoding.GetEncoding("UTF-8"),
                        "application/json"
                    )
                };
                return result;
            }

        }

        //验证账号密码
        private int isAPIUser(string UserNo,string PassWord)
        {
            Log.AddInfo("TokenController.isAPIUser()", "Begin");
            string sqlstr = "SELECT APIUserID FROM T_APIUser where UserNo = @UserNo and PassWord = @PassWord";
            SQLiteParameter[] par = { 
                new SQLiteParameter("@UserNo", UserNo),
                new SQLiteParameter("@PassWord",PassWord)
            };       
            DataSet qry = site.SelectData(sqlstr, par);
            if (qry.Tables.Count == 1 && qry.Tables[0].Rows.Count == 0)
            {
                Log.AddError("TokenController.isAPIUser()", "APIUser账号不存在");
                return -1;
            }
            else
            {
                Log.AddInfo("TokenController.isAPIUser()", "End");
                return Convert.ToInt32(qry.Tables[0].Rows[0]["APIUserID"].ToString());
            }
        }

        //写入token获取日志
        private void writeTokenLog(int APIUser,string token)
        {
            Log.AddInfo("TokenController.writeTokenLog()", "Begin");
            string sqlstr = "INSERT into T_TokenTrace VALUES(null,@APIUser,@nTime,@token,0)";
            SQLiteParameter[] par = {
                new SQLiteParameter("@APIUser", APIUser),
                new SQLiteParameter("@nTime",DateTime.Now.ToString()),
                new SQLiteParameter("@token",token),
            };
            int i = site.ChangeData(sqlstr, par);
            Log.AddInfo("TokenController.writeTokenLog()", "End");
        }

        [Route ("tokenTest")]
        [HttpGet]
        [APIFilter]
        public string test()
        {
            return "HELLO WORLD";
        }

    }
}
