using System.Text;

namespace org.maltparser.core.syntaxgraph.headrules
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using ColumnDescription = org.maltparser.core.io.dataformat.ColumnDescription;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class PrioSetMember
	{
		protected internal enum RelationToPrevMember
		{
			START,
			DISJUNCTION,
			CONJUNCTION
		}
		protected internal PrioSet prioSet;
		protected internal SymbolTable table;
		protected internal ColumnDescription column;
		protected internal int symbolCode;
		protected internal RelationToPrevMember relationToPrevMember;

		public PrioSetMember(PrioSet prioSet, SymbolTable table, ColumnDescription column, int symbolCode, RelationToPrevMember relationToPrevMember)
		{
			PrioSet = prioSet;
			Table = table;
			Column = column;
			SymbolCode = symbolCode;
			setRelationToPrevMember(relationToPrevMember);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrioSetMember(PrioSet prioSet, org.maltparser.core.symbol.SymbolTable table, org.maltparser.core.io.dataformat.ColumnDescription column, String symbolString, RelationToPrevMember relationToPrevMember) throws org.maltparser.core.exception.MaltChainedException
		public PrioSetMember(PrioSet prioSet, SymbolTable table, ColumnDescription column, string symbolString, RelationToPrevMember relationToPrevMember)
		{
			PrioSet = prioSet;
			Table = table;
			Column = column;
			if (table != null)
			{
				SymbolCode = table.getSymbolStringToCode(symbolString);
			}
			else
			{
				SymbolCode = -1;
			}
			setRelationToPrevMember(relationToPrevMember);
		}

		public virtual PrioSet PrioSet
		{
			get
			{
				return prioSet;
			}
			set
			{
				this.prioSet = value;
			}
		}



		public virtual ColumnDescription Column
		{
			get
			{
				return column;
			}
			set
			{
				this.column = value;
			}
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


		public virtual int SymbolCode
		{
			get
			{
				return symbolCode;
			}
			set
			{
				this.symbolCode = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbolString() throws org.maltparser.core.exception.MaltChainedException
		public virtual string SymbolString
		{
			get
			{
				if (table != null && symbolCode >= 0)
				{
					return table.getSymbolCodeToString(symbolCode);
				}
				else
				{
					return null;
				}
			}
		}


		public virtual RelationToPrevMember getRelationToPrevMember()
		{
			return relationToPrevMember;
		}

		public virtual void setRelationToPrevMember(RelationToPrevMember relationToPrevMember)
		{
			this.relationToPrevMember = relationToPrevMember;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + symbolCode;
			result = prime * result + ((relationToPrevMember == null) ? 0 : relationToPrevMember.GetHashCode());
			return result;
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
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			PrioSetMember other = (PrioSetMember) obj;
			if (symbolCode != other.symbolCode)
			{
				return false;
			}
			if (relationToPrevMember == null)
			{
				if (other.relationToPrevMember != null)
				{
					return false;
				}
			}
			else if (!relationToPrevMember.Equals(other.relationToPrevMember))
			{
				return false;
			}

			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(table.Name);
			sb.Append(':');
			try
			{
				sb.Append(SymbolString);
			}
			catch (MaltChainedException e)
			{
				if (prioSet.Logger.DebugEnabled)
				{
					prioSet.Logger.debug("",e);
				}
				else
				{
					prioSet.Logger.error(e.MessageChain);
				}
			}
			return sb.ToString();
		}
	}

}