using System;

namespace MVCDemo.Models
{
    [Serializable]
    public class JsonData
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string type { get; set; }
    }
}