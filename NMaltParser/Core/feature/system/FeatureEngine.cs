using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.feature.system
{
    using  function;
	using  helper;
    using  plugin;

    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	[Serializable]
	public class FeatureEngine : Dictionary<string, FunctionDescription>
	{
		public const long serialVersionUID = 3256444702936019250L;

		public FeatureEngine() : base()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.Function newFunction(String functionName, org.maltparser.core.feature.FeatureRegistry registry) throws org.maltparser.core.exception.MaltChainedException
		public virtual Function newFunction(string functionName, FeatureRegistry registry)
		{
			int i = 0;
			Function func = null;
			while (true)
			{
				FunctionDescription funcDesc = get(functionName + "~~" + i);
				if (funcDesc == null)
				{
					break;
				}
				func = funcDesc.newFunction(registry);
				if (func != null)
				{
					break;
				}
				i++;
			}
			return func;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(String urlstring) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(string urlstring)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
			URLFinder f = new URLFinder();
			load(f.findURL(urlstring));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(org.maltparser.core.plugin.PluginLoader plugins) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(PluginLoader plugins)
		{
			 foreach (Plugin plugin in plugins)
			 {
				URL url = null;
				try
				{
					url = new URL("jar:" + plugin.Url + "!/appdata/plugin.xml");
				}
				catch (MalformedURLException e)
				{
					throw new FeatureException("Malformed URL: 'jar:" + plugin.Url + "!plugin.xml'", e);
				}
				try
				{
					Stream @is = url.openStream();
					@is.Close();
				}
				catch (IOException)
				{
					continue;
				}

				load(url);
			 }
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.net.URL specModelURL) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(URL specModelURL)
		{
			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				Element root = null;

				root = db.parse(specModelURL.openStream()).DocumentElement;

				if (root == null)
				{
					throw new FeatureException("The feature system file '" + specModelURL.File + "' cannot be found. ");
				}

				readFeatureSystem(root);
			}
			catch (IOException e)
			{
				throw new FeatureException("The feature system file '" + specModelURL.File + "' cannot be found. ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new FeatureException("Problem parsing the file " + specModelURL.File + ". ", e);
			}
			catch (SAXException e)
			{
				throw new FeatureException("Problem parsing the file " + specModelURL.File + ". ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFeatureSystem(org.w3c.dom.Element system) throws org.maltparser.core.exception.MaltChainedException
		public virtual void readFeatureSystem(Element system)
		{
			NodeList functions = system.getElementsByTagName("function");
			for (int i = 0; i < functions.Length; i++)
			{
				readFunction((Element)functions.item(i));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFunction(org.w3c.dom.Element function) throws org.maltparser.core.exception.MaltChainedException
		public virtual void readFunction(Element function)
		{
			bool hasSubFunctions = function.getAttribute("hasSubFunctions").equalsIgnoreCase("true");

			bool hasFactory = false;
			if (function.getAttribute("hasFactory").length() > 0)
			{
				hasFactory = function.getAttribute("hasFactory").equalsIgnoreCase("true");
			}
			Type clazz = null;
			try
			{
				if (PluginLoader.instance() != null)
				{
					clazz = PluginLoader.instance().getClass(function.getAttribute("class"));
				}
				if (clazz == null)
				{
					clazz = Type.GetType(function.getAttribute("class"));
				}
			}
			catch (ClassNotFoundException e)
			{
				throw new FeatureException("The feature system could not find the function class" + function.getAttribute("class") + ".", e);
			}
			if (hasSubFunctions)
			{
				NodeList subfunctions = function.getElementsByTagName("subfunction");
				for (int i = 0; i < subfunctions.Length; i++)
				{
					readSubFunction((Element)subfunctions.item(i), clazz, hasFactory);
				}
			}
			else
			{
				int i = 0;
				string n = null;
				while (true)
				{
					n = function.getAttribute("name") + "~~" + i;
					if (!containsKey(n))
					{
						break;
					}
					i++;
				}
				put(n, new FunctionDescription(function.getAttribute("name"), clazz, false, hasFactory));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readSubFunction(org.w3c.dom.Element subfunction, Class clazz, boolean hasFactory) throws org.maltparser.core.exception.MaltChainedException
		public virtual void readSubFunction(Element subfunction, Type clazz, bool hasFactory)
		{
			int i = 0;
			string n = null;
			while (true)
			{
				n = subfunction.getAttribute("name") + "~~" + i;
				if (!containsKey(n))
				{
					break;
				}
				i++;
			}
			put(n, new FunctionDescription(subfunction.getAttribute("name"), clazz, true, hasFactory));
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			if (Count != ((FeatureEngine)obj).Count)
			{
				return false;
			}
			foreach (string name in keySet())
			{
				if (!get(name).Equals(((FeatureEngine)obj)[name]))
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (string name in keySet())
			{
				sb.Append(name);
				sb.Append('\t');
				sb.Append(get(name));
				sb.Append('\n');
			}
			return sb.ToString();
		}
	}

}