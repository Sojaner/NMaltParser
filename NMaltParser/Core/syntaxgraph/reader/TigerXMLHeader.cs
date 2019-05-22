using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph.reader
{

	using Util = org.maltparser.core.helper.Util;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class TigerXMLHeader
	{
		public enum Domain
		{
			T, // feature for terminal nodes
			NT, // feature for nonterminal nodes
			FREC, //feature for both
			EL, // edge label (same as "edgelabel" in TigerXML schema)
			SEL // secondary edge Label (same as "secedgelabel" in TigerXML schema)
		}

		private string corpusID;
		private string corpusVersion;
		private string external;
		private string metaName;
		private string metaAuthor;
		private string metaDescription;
		private string metaInDate;
		private string metaFormat;
		private string metaHistory;
	//	private SymbolTableHandler symbolTableHandler;
		private FeatureEdgeLabel edgeLabels;
		private FeatureEdgeLabel secEdgeLabels;
		private LinkedHashMap<string, FeatureEdgeLabel> features;

		public TigerXMLHeader()
		{
	//		setSymbolTableHandler(symbolTableHandler);
			features = new LinkedHashMap<string, FeatureEdgeLabel>();
		}

		public virtual bool TigerXMLWritable
		{
			get
			{
				return true;
				//return features.size() > 0;
			}
		}

		public virtual void addFeature(string featureName, string domainName)
		{
			if (!features.containsKey(featureName))
			{
				features.put(featureName, new FeatureEdgeLabel(this, featureName, domainName));
			}
		}

		public virtual void addFeatureValue(string featureName, string name)
		{
			addFeatureValue(featureName, name, "\t");
		}

		public virtual void addFeatureValue(string featureName, string name, string desc)
		{
			if (features.containsKey(featureName))
			{
				if (string.ReferenceEquals(desc, null) || desc.Length == 0)
				{
					features.get(featureName).addValue(name, "\t");
				}
				else
				{
					features.get(featureName).addValue(name, desc);
				}
			}
		}

		public virtual void addEdgeLabelValue(string name)
		{
			addEdgeLabelValue(name, "\t");
		}

		public virtual void addEdgeLabelValue(string name, string desc)
		{
			if (edgeLabels == null)
			{
				edgeLabels = new FeatureEdgeLabel(this, "edgelabel", Domain.EL);
			}
			if (string.ReferenceEquals(desc, null) || desc.Length == 0)
			{
				edgeLabels.addValue(name, "\t");
			}
			else
			{
				edgeLabels.addValue(name, desc);
			}
		}

		public virtual void addSecEdgeLabelValue(string name)
		{
			addSecEdgeLabelValue(name, "\t");
		}

		public virtual void addSecEdgeLabelValue(string name, string desc)
		{
			if (secEdgeLabels == null)
			{
				secEdgeLabels = new FeatureEdgeLabel(this, "secedgelabel", Domain.SEL);
			}
			if (string.ReferenceEquals(desc, null) || desc.Length == 0)
			{
				secEdgeLabels.addValue(name, "\t");
			}
			else
			{
				secEdgeLabels.addValue(name, desc);
			}
		}

		public virtual string CorpusID
		{
			get
			{
				return corpusID;
			}
			set
			{
				this.corpusID = value;
			}
		}


		public virtual string CorpusVersion
		{
			get
			{
				return corpusVersion;
			}
			set
			{
				this.corpusVersion = value;
			}
		}


		public virtual string External
		{
			set
			{
				this.external = value;
			}
			get
			{
				return external;
			}
		}


		public virtual void setMeta(string metaElement, string value)
		{
			if (metaElement.Equals("name"))
			{
				MetaName = value;
			}
			if (metaElement.Equals("author"))
			{
				MetaAuthor = value;
			}
			if (metaElement.Equals("description"))
			{
				MetaDescription = value;
			}
			if (metaElement.Equals("date"))
			{
				MetaInDate = value;
			}
			if (metaElement.Equals("format"))
			{
				MetaFormat = value;
			}
			if (metaElement.Equals("history"))
			{
				MetaHistory = value;
			}
		}

		public virtual string MetaName
		{
			get
			{
				return metaName;
			}
			set
			{
				this.metaName = value;
			}
		}


		public virtual string MetaAuthor
		{
			get
			{
				return metaAuthor;
			}
			set
			{
				this.metaAuthor = value;
			}
		}


		public virtual string MetaDescription
		{
			get
			{
				return metaDescription;
			}
			set
			{
				this.metaDescription = value;
			}
		}


		public virtual string MetaInDate
		{
			get
			{
				return metaInDate;
			}
			set
			{
				this.metaInDate = value;
			}
		}

		public virtual string MetaCurrentDate
		{
			get
			{
				return getMetaCurrentDate("yyyy-MM-dd HH:mm:ss");
			}
		}

		public virtual string getMetaCurrentDate(string format)
		{
			return (new SimpleDateFormat("yyyy-MM-dd HH:mm:ss")).format(DateTime.Now);
		}


		public virtual string MetaFormat
		{
			get
			{
				return metaFormat;
			}
			set
			{
				this.metaFormat = value;
			}
		}


		public virtual string MetaHistory
		{
			get
			{
				return metaHistory;
			}
			set
			{
				this.metaHistory = value;
			}
		}


	//	public SymbolTableHandler getSymbolTableHandler() {
	//		return symbolTableHandler;
	//	}
	//
	//	protected void setSymbolTableHandler(SymbolTableHandler symbolTableHandler) {
	//		this.symbolTableHandler = symbolTableHandler;
	//	}

		public virtual string toTigerXML()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();

			if (string.ReferenceEquals(CorpusVersion, null))
			{
				sb.Append("<corpus id=\"");
				sb.Append(((string.ReferenceEquals(CorpusID, null))?"GeneratedByMaltParser":CorpusID));
				sb.Append("\">\n");
			}
			else
			{
				sb.Append("<corpus id=\"");
				sb.Append(((string.ReferenceEquals(CorpusID, null))?"GeneratedByMaltParser":CorpusID));
				sb.Append("\" version=\"");
				sb.Append(CorpusVersion);
				sb.Append("\">\n");
			}
			sb.Append("  <head>\n");
			sb.Append("    <meta>\n");
			sb.Append("      <name>");
			sb.Append(((string.ReferenceEquals(MetaName, null))?"GeneratedByMaltParser":Util.xmlEscape(MetaName)));
			sb.Append("</name>\n");
			sb.Append("      <author>MaltParser</author>\n");
			sb.Append("      <date>");
			sb.Append(MetaCurrentDate);
			sb.Append("</date>\n");

			sb.Append("      <description>");
			sb.Append(Util.xmlEscape("Unfortunately, you have to add the annotations header data yourself. Maybe in later releases this will be fixed. "));
			sb.Append("</description>\n");

	//		if (getMetaDescription() != null) {
	//			sb.append("      <description>");
	//			sb.append(Util.xmlEscape(getMetaDescription()));
	//			sb.append("</description>\n");
	//		}
	//		if (getMetaFormat() != null) {
	//			sb.append("      <format>");
	//			sb.append(Util.xmlEscape(getMetaFormat()));
	//			sb.append("</format>\n");
	//		}
	//		if (getMetaHistory() != null) {
	//			sb.append("      <history>");
	//			sb.append(Util.xmlEscape(getMetaHistory()));
	//			sb.append("</history>\n");
	//		}
			sb.Append("    </meta>\n");
			sb.Append("    <annotation/>\n");
	//		sb.append("    <annotation>\n");
	//		for (String name : features.keySet()) {
	//			sb.append(features.get(name).toTigerXML());
	//		}
	//		if (edgeLabels != null) {
	//			sb.append(edgeLabels.toTigerXML());
	//		}
	//		if (secEdgeLabels != null) {
	//			sb.append(secEdgeLabels.toTigerXML());
	//		}
	//		sb.append("    </annotation>\n");
			sb.Append("  </head>\n");
			sb.Append("  <body>\n");
			return sb.ToString();
		}

		public override string ToString()
		{
			return toTigerXML();
		}

		protected internal class FeatureEdgeLabel
		{
			private readonly TigerXMLHeader outerInstance;

			internal string name;
			internal Domain domain;
			// values: key mapped to \t (tab) indicates that the description part is missing
			internal SortedDictionary<string, string> values;
			internal SymbolTable table;

			public FeatureEdgeLabel(TigerXMLHeader outerInstance, string name, string domainName)
			{
				this.outerInstance = outerInstance;
				Name = name;
				setDomain(domainName);
			}

			public FeatureEdgeLabel(TigerXMLHeader outerInstance, string name, Domain domain)
			{
				this.outerInstance = outerInstance;
				Name = name;
				setDomain(domain);
			}

			public virtual string Name
			{
				get
				{
					return name;
				}
				set
				{
					this.name = value;
				}
			}


			public virtual void setDomain(string domainName)
			{
				domain = Enum.Parse(typeof(Domain), domainName);
			}

			public virtual void setDomain(Domain domain)
			{
				this.domain = domain;
			}

			public virtual string DomainName
			{
				get
				{
					return domain.ToString();
				}
			}

			public virtual Domain getDomain()
			{
				return domain;
			}

			public virtual SymbolTable Table
			{
				get
				{
					return table;
				}
				set
				{
					this.table = value;
				}
			}


			public virtual void addValue(string name)
			{
				addValue(name, "\t");
			}

			public virtual void addValue(string name, string desc)
			{
				if (values == null)
				{
					values = new SortedDictionary<string, string>();
				}
				values[name] = desc;
			}

			public virtual string toTigerXML()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
				StringBuilder sb = new StringBuilder();
				if (domain == Domain.T || domain == Domain.FREC || domain == Domain.NT)
				{
					sb.Append("      <feature domain=\"");
					sb.Append(DomainName);
					sb.Append("\" name=\"");
					sb.Append(Name);
					sb.Append((values == null)?"\" />\n":"\">\n");
				}
				if (domain == Domain.EL)
				{
					sb.Append((values != null)?"      <edgelabel>\n":"      <edgelabel />\n");
				}
				if (domain == Domain.SEL)
				{
					sb.Append((values != null)?"      <secedgelabel>\n":"      <secedgelabel />\n");
				}
				if (values != null)
				{
					foreach (string name in values.Keys)
					{
						sb.Append("        <value name=\"");
						sb.Append(name);
						if (values[name].Equals("\t"))
						{
							sb.Append("\" />\n");
						}
						else
						{
							sb.Append("\">");
							sb.Append(Util.xmlEscape(values[name]));
							sb.Append("</value>\n");
						}
					}
				}
				if (domain == Domain.T || domain == Domain.FREC || domain == Domain.NT)
				{
					if (values != null)
					{
						sb.Append("      </feature>\n");
					}
				}
				if (domain == Domain.EL && values != null)
				{
					sb.Append("      </edgelabel>\n");
				}
				if (domain == Domain.SEL && values != null)
				{
					sb.Append("      </secedgelabel>\n");
				}
				return sb.ToString();
			}

			public override string ToString()
			{
				return toTigerXML();
			}
		}
	}



}