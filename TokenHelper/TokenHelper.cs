using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOTApi.Lib
{
    public class TokenHelper
    {
        
        private static string secret = HOTConfig.GetConfig().GetTokenSecret(); //"GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
         
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="payload">一个json对象，一般是验证信息。对这个json进行加密</param>
        /// <returns></returns>
        public static string SetJwtEncode(JObject payload)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, secret); //加密
            return token;
        }
        /// <summary>
        /// 解析token
        /// </summary>
        /// <param name="token">之前生成的token</param>
        /// <returns>返回解析后的结果，这个返回类型必须与加密的入参类型一致。</returns>
        public static JObject GetJwtDecode(string token)
        {
            try
            {

                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                var algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                var userInfo = decoder.DecodeToObject<JObject>(token, secret, verify: true);//token为之前生成的字符串
                return userInfo;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        

    }
}