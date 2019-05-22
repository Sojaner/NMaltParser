using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.feature.spec.reader
{

	using  org.maltparser.core.exception;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class ParReader : FeatureSpecReader
	{
		public enum DataStructures
		{
			STACK,
			INPUT,
			LEFTCONTEXT,
			RIGHTCONTEXT
		}
		public enum ColumnNames
		{
			POS,
			DEP,
			LEX,
			LEMMA,
			CPOS,
			FEATS
		}
		private Dictionary<ColumnNames, string> columnNameMap;
		private Dictionary<DataStructures, string> dataStructuresMap;
		private bool useSplitFeats = true;
		private bool covington = false;
		private bool pppath;
		private bool pplifted;
		private bool ppcoveredRoot;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParReader() throws org.maltparser.core.exception.MaltChainedException
		public ParReader()
		{
			initializeColumnNameMap();
			initializeDataStructuresMap();
			Pppath = false;
			Pplifted = false;
			PpcoveredRoot = false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.net.URL specModelURL, org.maltparser.core.feature.spec.SpecificationModels featureSpecModels) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(URL specModelURL, SpecificationModels featureSpecModels)
		{
			StreamReader br = null;
			Pattern tabPattern = Pattern.compile("\t");
			if (specModelURL == null)
			{
				throw new FeatureException("The feature specification file cannot be found. ");
			}
			try
			{
				br = new StreamReader(specModelURL.openStream());
			}
			catch (IOException e)
			{
				throw new FeatureException("Could not read the feature specification file '" + specModelURL.ToString() + "'. ", e);
			}

			if (br != null)
			{
				int specModelIndex = featureSpecModels.NextIndex;
				string fileLine;
				string[] items;
				StringBuilder featureText = new StringBuilder();
				string splitfeats = "";
				List<string> fileLines = new List<string>();
				List<string> orderFileLines = new List<string>();
				while (true)
				{
					try
					{
						fileLine = br.ReadLine();
					}
					catch (IOException e)
					{
						throw new FeatureException("Could not read the feature specification file '" + specModelURL.ToString() + "'. ", e);
					}
					if (string.ReferenceEquals(fileLine, null))
					{
						break;
					}
					if (fileLine.Length <= 1 && fileLine.Trim().Substring(0, 2).Trim().Equals("--"))
					{
						continue;
					}
					fileLines.Add(fileLine);
				}
				try
				{
					br.Close();
				}
				catch (IOException e)
				{
					throw new FeatureException("Could not close the feature specification file '" + specModelURL.ToString() + "'. ", e);
				}

				for (int j = 0; j < fileLines.Count; j++)
				{
					orderFileLines.Add(fileLines[j]);
				}

				bool deprel = false;
				for (int j = 0; j < orderFileLines.Count; j++)
				{
					deprel = false;
					featureText.Length = 0;
					splitfeats = "";
					items = tabPattern.split(orderFileLines[j]);
					if (items.Length < 2)
					{
						throw new FeatureException("The feature specification file '" + specModelURL.ToString() + "' must contain at least two columns.");
					}
					if (!(columnNameMap.ContainsKey(Enum.Parse(typeof(ColumnNames), items[0].Trim())) || columnNameMap.ContainsValue(items[0].Trim())))
					{
						throw new FeatureException("Column one in the feature specification file '" + specModelURL.ToString() + "' contains an unknown value '" + items[0].Trim() + "'. ");
					}
					if (items[0].Trim().Equals("DEP", StringComparison.OrdinalIgnoreCase) || items[0].Trim().Equals("DEPREL", StringComparison.OrdinalIgnoreCase))
					{
						featureText.Append("OutputColumn(DEPREL, ");
						deprel = true;
					}
					else
					{
						if (columnNameMap.ContainsKey(Enum.Parse(typeof(ColumnNames), items[0].Trim())))
						{
							featureText.Append("InputColumn(" + columnNameMap[Enum.Parse(typeof(ColumnNames), items[0].Trim())] + ", ");
						}
						else if (columnNameMap.ContainsValue(items[0].Trim()))
						{
							featureText.Append("InputColumn(" + items[0].Trim() + ", ");
						}
						if (items[0].Trim().Equals("FEATS", StringComparison.OrdinalIgnoreCase) && UseSplitFeats)
						{
							splitfeats = "Split(";
						}
					}
					if (!(items[1].Trim().Equals("STACK", StringComparison.OrdinalIgnoreCase) || items[1].Trim().Equals("INPUT", StringComparison.OrdinalIgnoreCase) || items[1].Trim().Equals("CONTEXT", StringComparison.OrdinalIgnoreCase)))
					{
						throw new FeatureException("Column two in the feature specification file '" + specModelURL.ToString() + "' should be either 'STACK', 'INPUT' or 'CONTEXT' (Covington), not '" + items[1].Trim() + "'. ");
					}
					int offset = 0;
					if (items.Length >= 3)
					{
						try
						{
							offset = new int?(int.Parse(items[2]));
						}
						catch (System.FormatException e)
						{
							throw new FeatureException("The feature specification file '" + specModelURL.ToString() + "' contains a illegal integer value. ", e);
						}
					}
					string functionArg = "";

					if (items[1].Trim().Equals("CONTEXT", StringComparison.OrdinalIgnoreCase))
					{
						if (offset >= 0)
						{
							functionArg = dataStructuresMap[Enum.Parse(typeof(DataStructures), "LEFTCONTEXT")] + "[" + offset + "]";
						}
						else
						{
							functionArg = dataStructuresMap[Enum.Parse(typeof(DataStructures), "RIGHTCONTEXT")] + "[" + Math.Abs(offset + 1) + "]";
						}
					}
					else if (dataStructuresMap.ContainsKey(Enum.Parse(typeof(DataStructures), items[1].Trim())))
					{
						if (covington == true)
						{
							if (dataStructuresMap[Enum.Parse(typeof(DataStructures), items[1].Trim())].Equals("Stack", StringComparison.OrdinalIgnoreCase))
							{
								functionArg = "Left[" + offset + "]";
							}
							else
							{
								functionArg = "Right[" + offset + "]";
							}
						}
						else
						{
							functionArg = dataStructuresMap[Enum.Parse(typeof(DataStructures), items[1].Trim())] + "[" + offset + "]";
						}
					}
					else if (dataStructuresMap.ContainsValue(items[1].Trim()))
					{
						if (covington == true)
						{
							if (items[1].Trim().Equals("Stack", StringComparison.OrdinalIgnoreCase))
							{
								functionArg = "Left[" + offset + "]";
							}
							else
							{
								functionArg = "Right[" + offset + "]";
							}
						}
						else
						{
							functionArg = items[1].Trim() + "[" + offset + "]";
						}

					}
					else
					{
						throw new FeatureException("Column two in the feature specification file '" + specModelURL.ToString() + "' should not contain the value '" + items[1].Trim());
					}

					int linearOffset = 0;
					int headOffset = 0;
					int depOffset = 0;
					int sibOffset = 0;
					int suffixLength = 0;
					if (items.Length >= 4)
					{
						linearOffset = new int?(int.Parse(items[3]));
					}
					if (items.Length >= 5)
					{
						headOffset = new int?(int.Parse(items[4]));
					}
					if (items.Length >= 6)
					{
						depOffset = new int?(int.Parse(items[5]));
					}
					if (items.Length >= 7)
					{
						sibOffset = new int?(int.Parse(items[6]));
					}
					if (items.Length >= 8)
					{
						suffixLength = new int?(int.Parse(items[7]));
					}
					if (linearOffset < 0)
					{
						linearOffset = Math.Abs(linearOffset);
						for (int i = 0; i < linearOffset; i++)
						{
							functionArg = "pred(" + functionArg + ")";
						}
					}
					else if (linearOffset > 0)
					{
						for (int i = 0; i < linearOffset; i++)
						{
							functionArg = "succ(" + functionArg + ")";
						}
					}
					if (headOffset >= 0)
					{
						for (int i = 0; i < headOffset; i++)
						{
							functionArg = "head(" + functionArg + ")";
						}
					}
					else
					{
						throw new FeatureException("The feature specification file '" + specModelURL.ToString() + "' should not contain a negative head function value. ");
					}
					if (depOffset < 0)
					{
						depOffset = Math.Abs(depOffset);
						for (int i = 0; i < depOffset; i++)
						{
							functionArg = "ldep(" + functionArg + ")";
						}
					}
					else if (depOffset > 0)
					{
						for (int i = 0; i < depOffset; i++)
						{
							functionArg = "rdep(" + functionArg + ")";
						}
					}
					if (sibOffset < 0)
					{
						sibOffset = Math.Abs(sibOffset);
						for (int i = 0; i < sibOffset; i++)
						{
							functionArg = "lsib(" + functionArg + ")";
						}
					}
					else if (sibOffset > 0)
					{
						for (int i = 0; i < sibOffset; i++)
						{
							functionArg = "rsib(" + functionArg + ")";
						}
					}

					if (deprel == true && (pppath == true || pplifted == true || ppcoveredRoot == true))
					{
						featureSpecModels.add(specModelIndex, mergePseudoProjColumns(functionArg));
					}
					else
					{
						if (suffixLength != 0)
						{
							featureSpecModels.add(specModelIndex, "Suffix(" + featureText.ToString() + functionArg + ")," + suffixLength + ")");
						}
						else if (splitfeats.Equals("Split("))
						{
							featureSpecModels.add(specModelIndex, splitfeats + featureText.ToString() + functionArg + "),\\|)");
						}
						else
						{
							featureSpecModels.add(specModelIndex, featureText.ToString() + functionArg + ")");
						}
					}

				}
			}
		}

		private string mergePseudoProjColumns(string functionArg)
		{
			StringBuilder newFeatureText = new StringBuilder();
			int c = 1;

			if (pplifted == true)
			{
				c++;
			};
			if (pppath == true)
			{
				c++;
			};
			if (ppcoveredRoot == true)
			{
				c++;
			};

			if (c == 1)
			{ // no merge
				newFeatureText.Append("OutputColumn(DEPREL, ");
				newFeatureText.Append(functionArg);
				newFeatureText.Append(')');
				return newFeatureText.ToString();
			}
			if (c == 2)
			{
				newFeatureText.Append("Merge(");
				newFeatureText.Append("OutputColumn(DEPREL, ");
				newFeatureText.Append(functionArg);
				newFeatureText.Append("), ");
				if (pplifted == true)
				{
					newFeatureText.Append("OutputTable(PPLIFTED, ");
					newFeatureText.Append(functionArg);
					newFeatureText.Append(")");
				}
				if (pppath == true)
				{
					newFeatureText.Append("OutputTable(PPPATH, ");
					newFeatureText.Append(functionArg);
					newFeatureText.Append(")");
				}
				if (ppcoveredRoot == true)
				{
					newFeatureText.Append("OutputTable(PPCOVERED, ");
					newFeatureText.Append(functionArg);
					newFeatureText.Append(")");
				}
				newFeatureText.Append(")");
			}
			else if (c == 3)
			{ // use Merge3
				int i = 0;
				newFeatureText.Append("Merge3(");
				newFeatureText.Append("OutputColumn(DEPREL, ");
				newFeatureText.Append(functionArg);
				newFeatureText.Append("), ");
				i++;
				if (pplifted == true)
				{
					newFeatureText.Append("OutputTable(PPLIFTED, ");
					newFeatureText.Append(functionArg);
					i++;
					if (i < 3)
					{
						newFeatureText.Append("), ");
					}
					else
					{
						newFeatureText.Append(")");
					}
				}
				if (pppath == true)
				{
					newFeatureText.Append("OutputTable(PPPATH, ");
					newFeatureText.Append(functionArg);
					i++;
					if (i < 3)
					{
						newFeatureText.Append("), ");
					}
					else
					{
						newFeatureText.Append(")");
					}
				}
				if (ppcoveredRoot == true)
				{
					newFeatureText.Append("OutputTable(PPCOVERED, ");
					newFeatureText.Append(functionArg);
					i++;
					if (i < 3)
					{
						newFeatureText.Append("), ");
					}
					else
					{
						newFeatureText.Append(")");
					}
				}
				newFeatureText.Append(")");
			}
			else
			{ // c == 4
				newFeatureText.Append("Merge(Merge(");
				newFeatureText.Append("OutputColumn(DEPREL, ");
				newFeatureText.Append(functionArg);
				newFeatureText.Append("), ");
				newFeatureText.Append("OutputTable(PPLIFTED, ");
				newFeatureText.Append(functionArg);
				newFeatureText.Append(")), Merge(");
				newFeatureText.Append("OutputTable(PPPATH, ");
				newFeatureText.Append(functionArg);
				newFeatureText.Append("), ");
				newFeatureText.Append("OutputTable(PPCOVERED, ");
				newFeatureText.Append(functionArg);
				newFeatureText.Append(")))");
			}
			return newFeatureText.ToString();
		}

		public virtual Dictionary<ColumnNames, string> ColumnNameMap
		{
			get
			{
				return columnNameMap;
			}
			set
			{
				this.columnNameMap = value;
			}
		}

		public virtual void initializeColumnNameMap()
		{
			columnNameMap = new Dictionary<ColumnNames, string>(typeof(ColumnNames));
			columnNameMap[ColumnNames.POS] = "POSTAG";
			columnNameMap[ColumnNames.CPOS] = "CPOSTAG";
			columnNameMap[ColumnNames.DEP] = "DEPREL";
			columnNameMap[ColumnNames.LEX] = "FORM";
			columnNameMap[ColumnNames.LEMMA] = "LEMMA";
			columnNameMap[ColumnNames.FEATS] = "FEATS";
		}


		public virtual Dictionary<DataStructures, string> DataStructuresMap
		{
			get
			{
				return dataStructuresMap;
			}
			set
			{
				this.dataStructuresMap = value;
			}
		}

		//TODO Fix covington
		public virtual void initializeDataStructuresMap()
		{
			dataStructuresMap = new Dictionary<DataStructures, string>(typeof(DataStructures));
			dataStructuresMap[DataStructures.STACK] = "Stack";
			dataStructuresMap[DataStructures.INPUT] = "Input";
		}


		public virtual bool UseSplitFeats
		{
			get
			{
				return useSplitFeats;
			}
			set
			{
				this.useSplitFeats = value;
			}
		}


		public virtual bool Covington
		{
			get
			{
				return covington;
			}
			set
			{
				this.covington = value;
			}
		}


		public virtual bool Pppath
		{
			get
			{
				return pppath;
			}
			set
			{
				this.pppath = value;
			}
		}


		public virtual bool Pplifted
		{
			get
			{
				return pplifted;
			}
			set
			{
				this.pplifted = value;
			}
		}


		public virtual bool PpcoveredRoot
		{
			get
			{
				return ppcoveredRoot;
			}
			set
			{
				this.ppcoveredRoot = value;
			}
		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Mapping of column names:\n");
			foreach (ColumnNames columnName in Enum.GetValues(typeof(ColumnNames)))
			{
				sb.Append(columnName.ToString() + "\t" + columnNameMap[columnName] + "\n");
			}
			sb.Append("Mapping of data structures:\n");
			foreach (DataStructures dataStruct in Enum.GetValues(typeof(DataStructures)))
			{
				sb.Append(dataStruct.ToString() + "\t" + dataStructuresMap[dataStruct] + "\n");
			}
			sb.Append("Split FEATS column: " + useSplitFeats + "\n");
			return sb.ToString();
		}
	}

}