using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text;

namespace SimpleTemplate
{
	public class Templater
	{
		private List<object> f = new List<object>();

		public Templater()
		{
		}

		public string Apply(object data)
		{
			StringBuilder result = new StringBuilder();
			foreach (var o in f)
			{
				var str = o as string;
				if (str == null)
				{
					var token = (Token)o;
					var value = ObjectFormatter.FormatObject(token.ValueGetter.Get(data), token.FormatString);

					if (value != null)
						result.Append(value);
				}
				else
					result.Append(str);
			}
			return result.ToString();
		}

		public void Compile(string template)
		{
			bool inToken = false;
			StringBuilder sb = new StringBuilder();

			for (var i = 0; i < template.Length; i++)
			{
				if (template[i] == '{')
				{
					if (sb.Length > 0)
						f.Add(sb.ToString());

					sb.Clear();
					inToken = true;
					continue;
				}

				if (template[i] == '}')
				{
					f.Add(new Token(sb.ToString()));
					sb.Clear();

					inToken = false;
					continue;
				}

				sb.Append(template[i]);
			}

			if (sb.Length > 0 && !inToken)
				f.Add(sb.ToString());
		}
	}

	public class Test1
	{
		public static string StaticTest { get; set; }

		public Test1()
		{
			StaticTest = "staticvar";
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			var vg = new ValueGetter("StaticTest");

			dynamic expando = new ExpandoObject();
			expando.test = "helloworld";

			var anom = new { test = "big day" };

			Stopwatch sw = new Stopwatch();

			sw.Start();
			string x = null;
			for (var i = 0; i < 10; i++)
			{
				x = (string)vg.Get(new Test1());
				//x = (string)vg.Get(expando2);
			}

			sw.Stop();

			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine(x);
			Console.ReadLine();
		}
	}
}
