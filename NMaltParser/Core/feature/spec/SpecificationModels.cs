using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.feature.spec
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature.spec.reader;
	using  org.maltparser.core.feature.spec.reader;
	using  org.maltparser.core.helper;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class SpecificationModels
	{
		private readonly HashMap<URL, FeatureSpecReader> specReaderMap;
		private readonly HashMap<string, SpecificationModel> specModelMap;
		private readonly HashMap<int, SpecificationModel> specModelIntMap;
		private readonly LinkedHashMap<URL, List<SpecificationModel>> specModelKeyMap;
		private readonly List<SpecificationModel> currentSpecModelURL;
		private int counter = 0;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SpecificationModels() throws org.maltparser.core.exception.MaltChainedException
		public SpecificationModels()
		{
			this.specReaderMap = new HashMap<URL, FeatureSpecReader>();
			this.specModelMap = new HashMap<string, SpecificationModel>();
			this.specModelIntMap = new HashMap<int, SpecificationModel>();
			this.specModelKeyMap = new LinkedHashMap<URL, List<SpecificationModel>>();
			this.currentSpecModelURL = new List<SpecificationModel>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(int index, String featureSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(int index, string featureSpec)
		{
			this.add(Convert.ToString(index), "MAIN", featureSpec);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(String specModelName, String featureSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(string specModelName, string featureSpec)
		{
			this.add(specModelName, "MAIN", featureSpec);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(int index, String subModelName, String featureSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(int index, string subModelName, string featureSpec)
		{
			this.add(Convert.ToString(index), subModelName, featureSpec);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(String specModelName, String subModelName, String featureSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(string specModelName, string subModelName, string featureSpec)
		{
			if (string.ReferenceEquals(featureSpec, null))
			{
				throw new FeatureException("Feature specification is missing.");
			}
			if (string.ReferenceEquals(specModelName, null))
			{
				throw new FeatureException("Unknown feature model name.");
			}
			if (string.ReferenceEquals(subModelName, null))
			{
				throw new FeatureException("Unknown subfeature model name.");
			}

			if (!specModelMap.ContainsKey(specModelName.ToUpper()))
			{
				SpecificationModel specModel = new SpecificationModel(specModelName.ToUpper());
				specModelMap[specModelName.ToUpper()] = specModel;
				currentSpecModelURL.Add(specModel);
				specModelIntMap[counter++] = specModel;
			}
			specModelMap[specModelName.ToUpper()].add(subModelName, featureSpec);
		}

		public virtual int NextIndex
		{
			get
			{
				return counter;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadParReader(java.net.URL specModelURL, String markingStrategy, String coveredRoot) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadParReader(URL specModelURL, string markingStrategy, string coveredRoot)
		{
			if (specModelURL == null)
			{
				throw new FeatureException("The URL to the feature specification model is missing or not well-formed. ");
			}
			FeatureSpecReader specReader = null;
			string urlSuffix = specModelURL.ToString().Substring(specModelURL.ToString().Length - 3);
			urlSuffix = char.ToUpper(urlSuffix[0]) + urlSuffix.Substring(1);
			try
			{
				Type clazz = Type.GetType("org.maltparser.core.feature.spec.reader." + urlSuffix + "Reader");
				specReader = (FeatureSpecReader)System.Activator.CreateInstance(clazz);
			}
			catch (InstantiationException e)
			{
				throw new FeatureException("Could not initialize the feature specification reader to read the specification file: " + specModelURL.ToString(), e);
			}
			catch (IllegalAccessException e)
			{
				throw new FeatureException("Could not initialize the feature specification reader to read the specification file: " + specModelURL.ToString(), e);
			}
			catch (ClassNotFoundException e)
			{
				throw new FeatureException("Could not find the feature specification reader to read the specification file: " + specModelURL.ToString(), e);
			}
			specReaderMap[specModelURL] = specReader;

			if (specReader is ParReader)
			{
				if (markingStrategy.Equals("head", StringComparison.OrdinalIgnoreCase) || markingStrategy.Equals("path", StringComparison.OrdinalIgnoreCase) || markingStrategy.Equals("head+path", StringComparison.OrdinalIgnoreCase))
				{
					((ParReader)specReader).Pplifted = true;
				}
				if (markingStrategy.Equals("path", StringComparison.OrdinalIgnoreCase) || markingStrategy.Equals("head+path", StringComparison.OrdinalIgnoreCase))
				{
					((ParReader)specReader).Pppath = true;
				}
				if (!coveredRoot.Equals("none", StringComparison.OrdinalIgnoreCase))
				{
					((ParReader)specReader).PpcoveredRoot = true;
				}
			}

			specModelKeyMap.put(specModelURL, currentSpecModelURL);
			specReader.load(specModelURL, this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.net.URL specModelURL) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(URL specModelURL)
		{
			if (specModelURL == null)
			{
				throw new FeatureException("The URL to the feature specification model is missing or not well-formed. ");
			}
			FeatureSpecReader specReader = null;
			string urlSuffix = specModelURL.ToString().Substring(specModelURL.ToString().Length - 3);
			urlSuffix = char.ToUpper(urlSuffix[0]) + urlSuffix.Substring(1);
			try
			{
				Type clazz = Type.GetType("org.maltparser.core.feature.spec.reader." + urlSuffix + "Reader");
				specReader = (FeatureSpecReader)System.Activator.CreateInstance(clazz);
			}
			catch (InstantiationException e)
			{
				throw new FeatureException("Could not initialize the feature specification reader to read the specification file: " + specModelURL.ToString(), e);
			}
			catch (IllegalAccessException e)
			{
				throw new FeatureException("Could not initialize the feature specification reader to read the specification file: " + specModelURL.ToString(), e);
			}
			catch (ClassNotFoundException e)
			{
				throw new FeatureException("Could not find the feature specification reader to read the specification file: " + specModelURL.ToString(), e);
			}
			specReaderMap[specModelURL] = specReader;

			specModelKeyMap.put(specModelURL, currentSpecModelURL);
			specReader.load(specModelURL, this);
		}

		public virtual SpecificationModel getSpecificationModel(URL url, int specModelUrlIndex)
		{
			return specModelKeyMap.get(url).get(specModelUrlIndex);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach (URL url in specModelKeyMap.Keys)
			{
				for (int i = 0; i < specModelKeyMap.get(url).size(); i++)
				{
					sb.Append(url.ToString());
					sb.Append(':');
					sb.Append(i);
					sb.Append('\n');
					sb.Append(specModelKeyMap.get(url).get(i).ToString());
				}
			}
			return sb.ToString();
		}
	}

}