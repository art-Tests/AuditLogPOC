using System;
using System.ComponentModel;
using System.Reflection;

namespace AuditLogPOC.API.Models
{
    public class AuditLogRequest
    {
        public int RefId { get; set; }
        public string LogType { get; set; }
        public string NewForm { get; set; }
        public string OldForm { get; set; }
    }

    public enum LogType
    {
        [Description("Customer")]
        Customer,
        [Description("Contract")]
        Contract
    }

    public static class MyExt
    {
        public static string GetDescriptionText(this LogType source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);
            if (attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
}