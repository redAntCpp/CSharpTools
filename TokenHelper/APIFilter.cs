using DataHelper;
using HOTApi.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace HOTApi.App_Start
{
    //创建API过滤类，只有通过验证才允许调用接口
    public class APIFilter : AuthorizeAttribute
    {      
        private JObject TokenConfig;
        private string TimeStamp;//用于加时
        private string ErrorMessage = "";
        SQLiteHelper site = new SQLiteHelper();

        //构造函数，获取从配置类中的配置
        public APIFilter()
        {
            string HOTConfigPath = HostingEnvironment.MapPath(@"/HOTApiConfig.Json");
            using (StreamReader file = System.IO.File.OpenText(HOTConfigPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    TokenConfig = o;
                    TimeStamp = TokenConfig["TokenConfig"]["TimeStamp"].ToString();
                }
            }
            site.buildConnetionString(HOTConfig.GetConfig().GetLocalDBConnetionString());
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
            
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var httpContext = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
            var authHeader = httpContext.Request.Headers["token"];
            if (authHeader == null)
            {
                ErrorMessage = "此接口必须携带token访问！";
                return false;//没有头文件为空，返回失败
            }
            else //字段值不为空
                
            {
                try
                {
                    JObject LoginInfo = TokenHelper.GetJwtDecode(authHeader);
                    string UserName = LoginInfo["userName"].ToString();
                    string PassWord = LoginInfo["passWord"].ToString();
                    int APIUserID = isAPIUser(UserName, PassWord);
                    if (APIUserID != -1)
                    {
                        //验证有效期
                        string TokenCreateTime = getTokenCreateTime(APIUserID);
                        DateTime Requestdt = DateTime.Parse(TokenCreateTime).AddMinutes(int.Parse(TimeStamp));
                        if (Requestdt < DateTime.Now)
                        {
                            ErrorMessage = "token已过期，请重新获取";
                            return false;
                        }
                        else
                        {

                            return true;
                        }
                    }
                    else
                    {
                        ErrorMessage = "token验证失败：您没有权限调用此接口，请联系管理员！";
                        return false;
                    }
                }catch(Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return false;
                }
                }

        }

     
        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            var response = filterContext.Response = filterContext.Response ?? new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.Forbidden;
            JsonDocument outParm = new JsonDocument();
            outParm.add(new JsonElement("retCode", "FAIL"));
            outParm.add(new JsonElement("retMsg", ErrorMessage));
            response.Content = new StringContent
                    (
                        outParm.innerText,
                        Encoding.GetEncoding("UTF-8"),
                        "application/json"
                    );

        }


        //辅助方法
        
        //验证账号密码
        private int isAPIUser(string UserNo, string PassWord)
        {
            Log.AddInfo("APIFilter.isAPIUser()", "Begin");
            string sqlstr = "SELECT APIUserID FROM T_APIUser where UserNo = @UserNo and PassWord = @PassWord";
            SQLiteParameter[] par = {
                new SQLiteParameter("@UserNo", UserNo),
                new SQLiteParameter("@PassWord",PassWord)
            };
            DataSet qry = site.SelectData(sqlstr, par);
            if (qry.Tables.Count == 1 && qry.Tables[0].Rows.Count == 0)
            {
                Log.AddError("APIFilter.isAPIUser()", "APIUser账号不存在");
                return -1;
            }
            else
            {
                Log.AddInfo("APIFilter.isAPIUser()", "End");
                return Convert.ToInt32(qry.Tables[0].Rows[0]["APIUserID"].ToString());
            }
        }

        private string getTokenCreateTime(int APIUserID)
        {
            string sqlStr = "SELECT CreateTime FROM T_TokenTrace where APIUserID = @APIUserID ORDER BY TokenTraceID  DESC LIMIT 1";
            SQLiteParameter[] par = {
                new SQLiteParameter("@APIUserID", APIUserID),
            };
            DataSet qry = site.SelectData(sqlStr, par);
            return qry.Tables[0].Rows[0]["CreateTime"].ToString();
        }
    }
}