namespace SimpleTemplate
{
	public class Token
	{
		public ValueGetter ValueGetter { get; set; }
		public string FormatString { get; set; }

		public Token(string token)
		{
			var parts = token.Split(':');
			ValueGetter = new ValueGetter(parts[0]);

			if (parts.Length > 1)
				FormatString = parts[1];
		}
	}
}