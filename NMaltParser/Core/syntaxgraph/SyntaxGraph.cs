namespace org.maltparser.core.syntaxgraph
{
    using  pool;
	using  symbol;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public abstract class SyntaxGraph : LabeledStructure, Observer
	{
		protected internal SymbolTableHandler symbolTables;
		protected internal readonly ObjectPoolList<LabelSet> labelSetPool;
		protected internal int numberOfComponents;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SyntaxGraph(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public SyntaxGraph(SymbolTableHandler symbolTables)
		{
			this.symbolTables = symbolTables;
			labelSetPool = new ObjectPoolListAnonymousInnerClass(this);
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<LabelSet>
		{
			private readonly SyntaxGraph outerInstance;

			public ObjectPoolListAnonymousInnerClass(SyntaxGraph outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			protected internal override LabelSet create()
			{
				return new LabelSet(6);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(LabelSet o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(LabelSet o)
			{
				o.clear();
			}
		}

		public virtual SymbolTableHandler SymbolTables
		{
			get
			{
				return symbolTables;
			}
			set
			{
				symbolTables = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(Element element, String labelFunction, String label) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLabel(Element element, string labelFunction, string label)
		{
			element.addLabel(symbolTables.addSymbolTable(labelFunction), label);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LabelSet checkOutNewLabelSet() throws org.maltparser.core.exception.MaltChainedException
		public virtual LabelSet checkOutNewLabelSet()
		{
			return labelSetPool.checkOut();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkInLabelSet(LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException
		public virtual void checkInLabelSet(LabelSet labelSet)
		{
			labelSetPool.checkIn(labelSet);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public virtual void clear()
		{
			numberOfComponents = 0;
			labelSetPool.checkInAll();
		}
	}

}