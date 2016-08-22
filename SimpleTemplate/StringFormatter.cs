namespace SimpleTemplate
{
	public class ObjectFormatter
	{
		public static string FormatObject(object o, string formatString)
		{
			if (o == null)
				return string.Empty;

			var str = o as string;
			if (str != null)
				return str;

			if (formatString == null)
				return o.ToString();

			var m = o.GetType().GetMethod("ToString", new[] { typeof(string) });
			return (string)m.Invoke(o, new[] { formatString });
		}
	}
}