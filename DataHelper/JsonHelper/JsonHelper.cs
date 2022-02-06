using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHelper
{
    /// <summary>
    /// Author: redAnt
    /// Date:2021年2月27日14:33:18
    /// Description：
    ///   1.JsonDocument,一份完整的json文档，文档由 { 开头，由若干json对象或者数组组成，再以 } 闭合
    ///   2.add方法，往此文档中新增一行，可以是一个json对象，也可以是json元素，也可以是json数组   
    /// </summary>
    class JsonDocument
    {
        string JDocument;
        ArrayList al = new ArrayList();//创建动态数组，因为json文本有多个不同类型的对象
        public string innerText
        {
            get
            {
                return this.tostring();
            }
        }

        private string tostring()
        {
            return "{" + JDocument + "}";
        }
        public JsonDocument()
        {
            this.JDocument = "";
        }
        public void add(JsonElement je)
        {
            if (JDocument == "")
            {
                this.JDocument = je.innerText;
            }
            else
            {
                this.JDocument = JDocument + "," + je.innerText;
            }
        }
        public void add(JsonObject jo)
        {
            if (JDocument == "")
            {
                this.JDocument = jo.innerText;
            }
            else
            {
                this.JDocument = JDocument + "," + jo.innerText;
            }
        }
        public void add(JsonArray ja)
        {
            if (JDocument == "")
            {
                this.JDocument = ja.innerText;
            }
            else
            {
                this.JDocument = JDocument + "," + ja.innerText;
            }
        }
        public void add(ArrayList al)
        {
            for (int i = 0; i < al.Count; i++)
            {
                if (al[i] is JsonElement)
                {
                    JsonElement je = al[i] as JsonElement;//object和自定义类的互转，通过：object对象 as  自定义类，方式转换
                    this.add(je);
                }
                else if (al[i] is JsonObject)
                {
                    JsonObject jo = al[i] as JsonObject;
                    this.add(jo);
                }
                else if (al[i] is JsonArray)
                {
                    JsonArray ja = al[i] as JsonArray;
                    this.add(ja);
                }
                else
                {
                    //抛出异常，这里还没写
                }
            }
        }
    }
    /// <summary>
    /// Author: redAnt
    /// Date:2021年2月27日14:36:35
    /// Description：
    ///   1.JsonElement，一个简单的json元素，表现形式为key：value   
    /// </summary>
    class JsonElement
    {
        //定义数组
        ArrayList JsonElementList = new ArrayList();
        private string JElement;
        //拾取器
        private string JsonKey = "";
        public string Jkey
        {
            get
            {
                return JsonKey;
            }
            set
            {
                if (value != "")
                {
                    JsonKey = value;
                }
            }
        }
        private string JValue;
        public string jsonValue
        {
            get
            {
                return JValue;
            }
            set
            {
                JValue = value;
            }
        }
        public string innerText
        {
            get
            {
                return this.tostring();
            }
        }

        private string[] VlaueList;
        public string[] setVlaueList
        {
            get
            {
                return VlaueList;
            }
            set
            {
                VlaueList = value;
            }
        }

        //输出字符串
        public string tostring()
        {
            return JElement;
        }
        public string valueToString()
        {
            return JValue;
        }
        public string keyToString()
        {
            return JsonKey;
        }
        //构造函数 "key": "value"
        public JsonElement(string key, string value)
        {
            this.Jkey = key;
            this.jsonValue = value;
            JElement = string.Format("\"{0}\":\"" + JValue + "\"", Jkey);
        }
    }


    /// <summary>
    /// Author: redAnt
    /// Date:2021年2月27日14:38:00
    /// Description：
    ///   1.JsonObject，一个简单的json对象，通常一个对象内存在多个json元素或者json数组 
    ///   2.add方法，对此json对象追加一行，用于向此json对象中追加一个元素
    /// </summary>
    class JsonObject
    {
        private string JObjectValue;
        private string Jkey;
        public string innerText
        {
            get
            {
                return this.tostring();
            }
            set
            {
                JObjectValue = value;
            }
        }
        private string Jsonkey
        {
            set
            {
                if (value != "")
                {
                    Jkey = value;
                }
            }
        }

        //输出函数
        private string tostring()
        {
            string temp = string.Format("\"{0}\":", Jkey);
            temp = temp + JObjectValue;
            return temp;
        }
        public JsonObject(string key)
        {
            this.Jsonkey = key;
            this.innerText = "";
        }
        public JsonObject(string key,JsonDocument jd)
        {
            this.Jsonkey = key;
            this.innerText = jd.innerText;
        }
    }

    /// <summary>
    /// Author: redAnt
    /// Date:2021年2月27日14:42:27
    /// Description：
    ///   1.JsonArray，一个简单的json数组，通常一个json数组内为一个字符串数组，或者一个json对象素组
    ///   2.add方法，对此json数组新增一个元素
    ///   ============================================
    ///   2022年1月23日20:54:22 redAnt 新增对数组的处理，当Jkey为空的情况
    /// </summary>

    class JsonArray
    {
        private string Jkey;
        List<JsonDocument> JArryList; //动态数组，用于处理一系列json文档的集合
        List<string> JstrArryList;//动态数组，用于处理字符串型数组
        public string innerText
        {
            get
            {
                return this.tostring();
            }
        }
        //构造函数 "key":[value1,value2...]
        public JsonArray(string key, string[] ValueList)
        {
            this.Jkey = key;
            JstrArryList = new List<string>(ValueList);//将数组转为list
        }
        //构造函数"key":[josn1,json2...]
        public JsonArray(string key, JsonDocument[] ValueList)
        {
            this.Jkey = key;
            JArryList = new List<JsonDocument>(ValueList);
        }
        public JsonArray(string key)
        {
            this.Jkey = key;
            JArryList = new List<JsonDocument>();
            JstrArryList = new List<string>();
        }
        public JsonArray()
        {
            this.Jkey = "";
            JArryList = new List<JsonDocument>();
            JstrArryList = new List<string>();
        }
        //输出函数
        public string tostring()
        {
            string StrValueList = "";
            //输出json对象
            if (JArryList != null && JArryList.Count != 0)//存在且元素个数为0
            {
                for (int i = 0; i < JArryList.Count; i++)
                {
                    StrValueList = StrValueList + JArryList[i].innerText + ",";
                }
                StrValueList = StrValueList.Remove(StrValueList.Length - 1, 1);
            }
            else if (JstrArryList != null && JstrArryList.Count != 0)  //输出json数组字符串
            {
                for (int i = 0; i < JstrArryList.Count; i++)
                {
                    StrValueList = StrValueList + "\"" + JstrArryList[i] + "\"" + ",";
                }
                StrValueList = StrValueList.Remove(StrValueList.Length - 1, 1);
            }
            else
            {
                StrValueList = "";
            }
            //分开解析
            StrValueList = "[" + StrValueList + "]";
            //2022年1月23日20:53:25 新增数组中key为空的情况
            if (Jkey == "")
            {
                return StrValueList;
            }
            else
            {
                string temp = string.Format("\"{0}\":", Jkey);
                StrValueList = temp + StrValueList;
                return StrValueList;
            }
        }
        //新增一个元素
        public void add(string strValue)
        {
            JstrArryList.Add(strValue);
        }
        public void add(JsonDocument jd)
        {
            JArryList.Add(jd);
        }
    }
}
