namespace org.maltparser.core.syntaxgraph
{

	using  org.maltparser.core.symbol;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class LabelSet : LinkedHashMap<SymbolTable, int>
	{
		public const long serialVersionUID = 8045567022124816378L;
		public LabelSet() : base()
		{
		}
		public LabelSet(int initialCapacity) : base(initialCapacity)
		{
		}
		public LabelSet(int initialCapacity, float loadFactor) : base(initialCapacity,loadFactor)
		{
		}
		public LabelSet(LabelSet labelSet) : base(labelSet)
		{
		}
	}

}