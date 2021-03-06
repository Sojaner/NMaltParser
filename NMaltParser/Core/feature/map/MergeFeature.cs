﻿using System;
using System.Text;
using NMaltParser.Core.Feature.Function;
using NMaltParser.Core.Feature.Value;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;

namespace NMaltParser.Core.Feature.Map
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public sealed class MergeFeature : FeatureMapFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(FeatureFunction), typeof(FeatureFunction)};
		private FeatureFunction firstFeature;
		private FeatureFunction secondFeature;
		private readonly SymbolTableHandler tableHandler;
		private SymbolTable table;
		private readonly SingleFeatureValue singleFeatureValue;
		private int type;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MergeFeature(org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public MergeFeature(SymbolTableHandler tableHandler)
		{
			this.tableHandler = tableHandler;
			singleFeatureValue = new SingleFeatureValue(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void initialize(object[] arguments)
		{
			if (arguments.Length != 2)
			{
				throw new FeatureException("Could not initialize MergeFeature: number of arguments are not correct. ");
			}
			if (!(arguments[0] is FeatureFunction))
			{
				throw new FeatureException("Could not initialize MergeFeature: the first argument is not a feature. ");
			}
			if (!(arguments[1] is FeatureFunction))
			{
				throw new FeatureException("Could not initialize MergeFeature: the second argument is not a feature. ");
			}
			FirstFeature = (FeatureFunction)arguments[0];
			SecondFeature = (FeatureFunction)arguments[1];

			if (firstFeature.Type != secondFeature.Type)
			{
				throw new FeatureException("Could not initialize MergeFeature: the first and the second arguments are not of the same type.");
			}
			type = firstFeature.Type;
			SymbolTable = tableHandler.addSymbolTable("MERGE2_" + firstFeature.MapIdentifier + "_" + secondFeature.MapIdentifier, ColumnDescription.INPUT, ColumnDescription.STRING, "One");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public void update()
		{
			singleFeatureValue.reset();
			firstFeature.update();
			secondFeature.update();
			FeatureValue firstValue = firstFeature.FeatureValue;
			FeatureValue secondValue = secondFeature.FeatureValue;
			if (firstValue.Multiple || secondValue.Multiple)
			{
				throw new FeatureException("It is not possible to merge Split-features. ");
			}

			string firstSymbol = ((SingleFeatureValue)firstValue).Symbol;
			if (firstValue.NullValue && secondValue.NullValue)
			{
				singleFeatureValue.IndexCode = firstFeature.SymbolTable.getSymbolStringToCode(firstSymbol);
				singleFeatureValue.Symbol = firstSymbol;
				singleFeatureValue.NullValue = true;
				return;
			}
			if (Type == ColumnDescription.STRING)
			{
				StringBuilder mergedValue = new StringBuilder();
				mergedValue.Append(firstSymbol);
				mergedValue.Append('~');
				mergedValue.Append(((SingleFeatureValue)secondValue).Symbol);
				singleFeatureValue.IndexCode = table.addSymbol(mergedValue.ToString());
				singleFeatureValue.Symbol = mergedValue.ToString();
				singleFeatureValue.NullValue = false;
				singleFeatureValue.Value = 1;
			}
			else
			{
				if (firstValue.NullValue || secondValue.NullValue)
				{
					singleFeatureValue.Value = 0;
					table.addSymbol("#null#");
					singleFeatureValue.Symbol = "#null#";
					singleFeatureValue.NullValue = true;
					singleFeatureValue.IndexCode = 1;
				}
				else
				{
					if (Type == ColumnDescription.BOOLEAN)
					{
						bool result = false;
						int dotIndex = firstSymbol.IndexOf('.');
						result = firstSymbol.Equals("1") || firstSymbol.Equals("true") || firstSymbol.Equals("#true#") || (dotIndex != -1 && firstSymbol.Substring(0,dotIndex).Equals("1"));
						if (result == true)
						{
							string secondSymbol = ((SingleFeatureValue)secondValue).Symbol;
							dotIndex = secondSymbol.IndexOf('.');
							result = secondSymbol.Equals("1") || secondSymbol.Equals("true") || secondSymbol.Equals("#true#") || (dotIndex != -1 && secondSymbol.Substring(0,dotIndex).Equals("1"));
						}
						if (result)
						{
							singleFeatureValue.Value = 1;
							table.addSymbol("true");
							singleFeatureValue.Symbol = "true";
						}
						else
						{
							singleFeatureValue.Value = 0;
							table.addSymbol("false");
							singleFeatureValue.Symbol = "false";
						}
					}
					else if (Type == ColumnDescription.INTEGER)
					{
						int? firstInt = 0;
						int? secondInt = 0;

						int dotIndex = firstSymbol.IndexOf('.');
						try
						{
							if (dotIndex == -1)
							{
								firstInt = int.Parse(firstSymbol);
							}
							else
							{
								firstInt = int.Parse(firstSymbol.Substring(0,dotIndex));
							}
						}
						catch (FormatException e)
						{
							throw new FeatureException("Could not cast the feature value '" + firstSymbol + "' to integer value.", e);
						}
						string secondSymbol = ((SingleFeatureValue)secondValue).Symbol;
						dotIndex = secondSymbol.IndexOf('.');
						try
						{
							if (dotIndex == -1)
							{
								secondInt = int.Parse(secondSymbol);
							}
							else
							{
								secondInt = int.Parse(secondSymbol.Substring(0,dotIndex));
							}
						}
						catch (FormatException e)
						{
							throw new FeatureException("Could not cast the feature value '" + secondSymbol + "' to integer value.", e);
						}
						int? result = firstInt * secondInt;
						singleFeatureValue.Value = result;
						table.addSymbol(result.ToString());
						singleFeatureValue.Symbol = result.ToString();
					}
					else if (Type == ColumnDescription.REAL)
					{
						double? firstReal = 0.0;
						double? secondReal = 0.0;
						try
						{
							firstReal = double.Parse(firstSymbol);
						}
						catch (FormatException e)
						{
							throw new FeatureException("Could not cast the feature value '" + firstSymbol + "' to real value.", e);
						}
						string secondSymbol = ((SingleFeatureValue)secondValue).Symbol;
						try
						{
							secondReal = double.Parse(secondSymbol);
						}
						catch (FormatException e)
						{
							throw new FeatureException("Could not cast the feature value '" + secondSymbol + "' to real value.", e);
						}
						double? result = firstReal * secondReal;
						singleFeatureValue.Value = result.Value;
						table.addSymbol(result.ToString());
						singleFeatureValue.Symbol = result.ToString();
					}
					singleFeatureValue.NullValue = false;
					singleFeatureValue.IndexCode = 1;
				}
			}
		}

		public Type[] ParameterTypes
		{
			get
			{
				return paramTypes;
			}
		}

		public FeatureValue FeatureValue
		{
			get
			{
				return singleFeatureValue;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException
		public string getSymbol(int code)
		{
			return table.getSymbolCodeToString(code);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public int getCode(string symbol)
		{
			return table.getSymbolStringToCode(symbol);
		}

		public FeatureFunction FirstFeature
		{
			get
			{
				return firstFeature;
			}
			set
			{
				firstFeature = value;
			}
		}


		public FeatureFunction SecondFeature
		{
			get
			{
				return secondFeature;
			}
			set
			{
				secondFeature = value;
			}
		}


		public SymbolTableHandler TableHandler
		{
			get
			{
				return tableHandler;
			}
		}

		public SymbolTable SymbolTable
		{
			get
			{
				return table;
			}
			set
			{
				table = value;
			}
		}


		public int Type
		{
			get
			{
				return type;
			}
		}

		public string MapIdentifier
		{
			get
			{
				return SymbolTable.Name;
			}
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
			return obj.ToString().Equals(ToString());
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("Merge(");
			sb.Append(firstFeature.ToString());
			sb.Append(", ");
			sb.Append(secondFeature.ToString());
			sb.Append(')');
			return sb.ToString();
		}

	}

}