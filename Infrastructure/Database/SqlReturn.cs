using System.Collections.Generic;

namespace MonthlyRevenueApi.Infrastructure.Database
{
    public class SqlReturn
    {
        public string OutString { get; set; }
        public Dictionary<string, object> ParameterResult { get; set; }
    }

    public class SqlReturn<T>
    {
        public string OutString { get; set; }
        public List<T> Result { get; set; }
        public Dictionary<string, object> ParameterResult { get; set; }
    }

    public class SqlReturn<T1, T2>
    {
        public string OutString { get; set; }
        public List<T1> T1Result { get; set; }
        public List<T2> T2Result { get; set; }
        public Dictionary<string, object> ParameterResult { get; set; }
    }
}
