using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace SimpleTemplate
{
	public class ValueGetter
	{
		private Func<Func<object>, object> _f;

		private string[] _path;

		public ValueGetter(string path)
		{
			_path = path.Split('.');
		}

		private static ConcurrentDictionary<Type, CallSite<Func<CallSite, object, object>>> _cache = new ConcurrentDictionary<Type, CallSite<Func<CallSite, object, object>>>();

		static object GetDynamicMember(object obj, string memberName)
		{
			var callsite = _cache.GetOrAdd(obj.GetType(), t =>
			{
				var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, memberName, obj.GetType(),
					new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
				return CallSite<Func<CallSite, object, object>>.Create(binder);
			});
			return callsite.Target(callsite, obj);
		}

		public object Resolve(object o, string memberName)
		{
			if (o is IDynamicMetaObjectProvider)
			{
				return (GetDynamicMember(o, memberName));
			}

			var p = o.GetType().GetProperty(memberName);

			if (p != null)
				return p.GetValue(o);

			var f = o.GetType().GetField(memberName);

			if (f != null)
				return f.GetValue(o);

			return null;
		}

		public object Get(object o)
		{
			foreach (var s in _path)
			{
				o = Resolve(o, s);

				if (o == null)
					return null;
			}
			return o;
		}
	}
}