using System;
using System.Collections.Generic;

namespace org.maltparser.core.config
{
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ConfigurationRegistry : Dictionary<Type, object>
	{
		public const long serialVersionUID = 3256444702936019250L;

		public ConfigurationRegistry() : base()
		{
		}

		public override object get(object key)
		{
			return base[key];
		}

		public override object put(Type key, object value)
		{
			return base[key] = value;
		}
	}

}