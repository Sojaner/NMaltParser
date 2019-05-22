using System;
using System.Text;
using NMaltParser.Core.Helper;
using NMaltParser.Utilities;

namespace NMaltParser.Core.Config.Version
{
    public class Versioning
	{
		private string maltParserVersion;
		private string parserModelVersion;
		private File mcoPath;
		private string configName;
		private string newConfigName;
		private string configType;

		private string featureModelXML;
		private string inputFormatXML;

		public static string[] availableVersions = new string[] {"1.0.0", "1.0.1", "1.0.2", "1.0.3", "1.1", "1.2", "1.3", "1.3.1", "1.4", "1.4.1"};
		public static bool[] supportVersions = new bool[] {false, false, false, false, false, false, true, true, true};

		public Versioning(string configName, string configType, File mcoPath, string parserModelVersion)
		{
			ConfigName = configName;
			ConfigType = configType;
			McoPath = mcoPath;
			MaltParserVersion = SystemInfo.Version;
			ParserModelVersion = parserModelVersion;
			NewConfigName = configName + "." + maltParserVersion;
		}

		public virtual JarEntry getJarEntry(JarEntry @in)
		{
			if (maltParserVersion.Equals(parserModelVersion))
			{
				return @in;
			}
			string entryName = @in.Name.replace(configName + File.separator, newConfigName + File.separator);
			if (entryName.EndsWith(".info", StringComparison.Ordinal))
			{
				return new JarEntry(entryName.Replace(File.separator + configName + "_", File.separator + newConfigName + "_"));
			}
			return new JarEntry(entryName);
		}

		public virtual bool hasChanges(JarEntry @in, JarEntry @out)
		{
			if (maltParserVersion.Equals(parserModelVersion))
			{
				return false;
			}
			if (@in.Name.EndsWith(".info") || @in.Name.EndsWith(".sop"))
			{
				return true;
			}
			return false;
		}

		public virtual string modifyJarEntry(JarEntry @in, JarEntry @out, StringBuilder sb)
		{
			if (maltParserVersion.Equals(parserModelVersion))
			{
				return sb.ToString();
			}
			if (@in.Name.EndsWith(".info"))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder outString = new StringBuilder();
				StringBuilder outString = new StringBuilder();
				string[] lines = sb.ToString().Split("\\n", true);
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines[i].StartsWith("Configuration name:", StringComparison.Ordinal))
					{
						outString.Append("Configuration name:   ");
						outString.Append(configName);
						outString.Append('.');
						outString.Append(maltParserVersion);
						outString.Append('\n');
					}
					else if (lines[i].StartsWith("Created:", StringComparison.Ordinal))
					{
						outString.Append(lines[i]);
						outString.Append('\n');
						outString.Append("Converted:            ");
						outString.Append(new DateTime(DateTimeHelper.CurrentUnixTimeMillis()));
						outString.Append('\n');
					}
					else if (lines[i].StartsWith("Version:", StringComparison.Ordinal))
					{
						outString.Append("Version:                       ");
						outString.Append(maltParserVersion);
						outString.Append('\n');
						outString.Append("Created by:                    ");
						outString.Append(parserModelVersion);
						outString.Append('\n');
					}
					else if (lines[i].StartsWith("  name (  -c)                           ", StringComparison.Ordinal))
					{
						outString.Append("  name (  -c)                           ");
						outString.Append(newConfigName);
						outString.Append('\n');
					}
					else if (lines[i].StartsWith("  format ( -if)                         /appdata/dataformat/", StringComparison.Ordinal))
					{
						outString.Append("  format ( -if)                         ");
						int index = lines[i].LastIndexOf("/", StringComparison.Ordinal);
						outString.Append(lines[i].Substring(index + 1));
						outString.Append('\n');
					}
					else if (lines[i].StartsWith("  format ( -of)                         /appdata/dataformat/", StringComparison.Ordinal))
					{
						outString.Append("  format ( -of)                         ");
						int index = lines[i].LastIndexOf("/", StringComparison.Ordinal);
						outString.Append(lines[i].Substring(index + 1));
						outString.Append('\n');
					}
					else if (lines[i].StartsWith("--guide-features (  -F)                 /appdata/features/", StringComparison.Ordinal))
					{
						outString.Append("--guide-features (  -F)                 ");
						int index = lines[i].LastIndexOf("/", StringComparison.Ordinal);
						outString.Append(lines[i].Substring(index + 1));
						outString.Append('\n');
					}
					else
					{
						outString.Append(lines[i]);
						outString.Append('\n');
					}
				}
				return outString.ToString();
			}
			else if (@in.Name.EndsWith(".sop"))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder outString = new StringBuilder();
				StringBuilder outString = new StringBuilder();
				string[] lines = sb.ToString().Split("\\n", true);
				for (int i = 0; i < lines.Length; i++)
				{
					int index = lines[i].IndexOf('\t');
					int container = 0;
					if (index > -1)
					{
						container = int.Parse(lines[i].Substring(0,index));
					}
					if (lines[i].StartsWith(container + "\tguide\tfeatures", StringComparison.Ordinal))
					{
						int tabIndex = lines[i].LastIndexOf('\t');
						if (lines[i].Substring(tabIndex + 1).StartsWith("/appdata/features/", StringComparison.Ordinal))
						{
							int slashIndex = lines[i].LastIndexOf("/", StringComparison.Ordinal);
							string xmlFile = lines[i].Substring(slashIndex + 1);
							string path = lines[i].Substring(tabIndex + 1, slashIndex - (tabIndex + 1));
							FeatureModelXML = path + "/libsvm/" + xmlFile;
							outString.Append(container);
							outString.Append("\tguide\tfeatures\t");
							outString.Append(xmlFile);
							outString.Append('\n');
						}
						else
						{
							outString.Append(lines[i]);
							outString.Append('\n');
						}
					}
					else if (lines[i].StartsWith(container + "\tinput\tformat", StringComparison.Ordinal))
					{
						int tabIndex = lines[i].LastIndexOf('\t');
						if (lines[i].Substring(tabIndex + 1).StartsWith("/appdata/dataformat/", StringComparison.Ordinal))
						{
							int slashIndex = lines[i].LastIndexOf("/", StringComparison.Ordinal);
							string xmlFile = lines[i].Substring(slashIndex + 1);
							string path = lines[i].Substring(tabIndex + 1, slashIndex - (tabIndex + 1));
							InputFormatXML = path + "/" + xmlFile;
							outString.Append(container);
							outString.Append("\tinput\tformat\t");
							outString.Append(xmlFile);
							outString.Append('\n');
						}
						else
						{
							outString.Append(lines[i]);
							outString.Append('\n');
						}
					}
					else if (earlierVersion("1.3"))
					{
						if (lines[i].StartsWith(container + "\tnivre\tpost_processing", StringComparison.Ordinal))
						{
						}
						else if (lines[i].StartsWith(container + "\tmalt0.4\tbehavior", StringComparison.Ordinal))
						{
							if (lines[i].EndsWith("true", StringComparison.Ordinal))
							{
								SystemLogger.logger().info("MaltParser " + maltParserVersion + " doesn't support MaltParser 0.4 emulation.");
							}
						}
						else if (lines[i].StartsWith(container + "\tsinglemalt\tparsing_algorithm", StringComparison.Ordinal))
						{
							outString.Append(container);
							outString.Append("\tsinglemalt\tparsing_algorithm\t");
							if (lines[i].EndsWith("NivreStandard", StringComparison.Ordinal))
							{
								outString.Append("class org.maltparser.parser.algorithm.nivre.NivreArcStandardFactory");
							}
							else if (lines[i].EndsWith("NivreEager", StringComparison.Ordinal))
							{
								outString.Append("class org.maltparser.parser.algorithm.nivre.NivreArcEagerFactory");
							}
							else if (lines[i].EndsWith("CovingtonNonProjective", StringComparison.Ordinal))
							{
								outString.Append("class org.maltparser.parser.algorithm.covington.CovingtonNonProjFactory");
							}
							else if (lines[i].EndsWith("CovingtonProjective", StringComparison.Ordinal))
							{
								outString.Append("class org.maltparser.parser.algorithm.covington.CovingtonProjFactory");
							}
							outString.Append('\n');
						}
					}
					else
					{
						outString.Append(lines[i]);
						outString.Append('\n');
					}
				}
				return outString.ToString();
			}
			return sb.ToString();
		}


		public virtual bool earlierVersion(string version)
		{
			bool e = false;
			for (int i = 0; i < availableVersions.Length; i++)
			{
				if (availableVersions[i].Equals(version))
				{
					break;
				}
				else if (availableVersions[i].Equals(parserModelVersion))
				{
					e = true;
				}
			}
			return e;
		}

		public virtual bool support(string version)
		{
			for (int i = 0; i < availableVersions.Length; i++)
			{
				if (availableVersions[i].Equals(version))
				{
					return supportVersions[i];
				}
			}
			return false;
		}

		public virtual string FeatureModelXML
		{
			get
			{
				return featureModelXML;
			}
			set
			{
				featureModelXML = value;
			}
		}


		public virtual string InputFormatXML
		{
			get
			{
				return inputFormatXML;
			}
			set
			{
				inputFormatXML = value;
			}
		}


		public virtual string NewConfigName
		{
			get
			{
				return newConfigName;
			}
			set
			{
				newConfigName = value;
			}
		}


		public virtual string ConfigName
		{
			get
			{
				return configName;
			}
			set
			{
				configName = value;
			}
		}


		public virtual string ConfigType
		{
			get
			{
				return configType;
			}
			set
			{
				configType = value;
			}
		}


		public virtual File McoPath
		{
			get
			{
				return mcoPath;
			}
			set
			{
				mcoPath = value;
			}
		}


		public virtual string MaltParserVersion
		{
			get
			{
				return maltParserVersion;
			}
			set
			{
				maltParserVersion = value;
			}
		}


		public virtual string ParserModelVersion
		{
			get
			{
				return parserModelVersion;
			}
			set
			{
				parserModelVersion = value;
			}
		}

	}

}