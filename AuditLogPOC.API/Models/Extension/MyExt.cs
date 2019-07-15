using System.ComponentModel;
using System.Reflection;
using AuditLogPOC.API.Models.Enum;

namespace AuditLogPOC.API.Models.Extension
{
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