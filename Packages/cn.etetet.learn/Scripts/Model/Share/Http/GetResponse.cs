/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET
{
    public class GetResponse : Entity
    {
        public int code;
        public string msg;
        public Dictionary<string, Dictionary<int, long>> data;
    }
}