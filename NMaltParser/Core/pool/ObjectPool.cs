namespace NMaltParser.Core.Pool
{
    public abstract class ObjectPool<T>
	{
		protected internal int keepThreshold;

		public ObjectPool() : this(int.MaxValue)
		{
		}

		public ObjectPool(int keepThreshold)
		{
			KeepThreshold = keepThreshold;
		}

		public virtual int KeepThreshold
		{
			get
			{
				return keepThreshold;
			}
			set
			{
				keepThreshold = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract T create() throws org.maltparser.core.exception.MaltChainedException;
		protected internal abstract T create();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void resetObject(T o) throws org.maltparser.core.exception.MaltChainedException;
		public abstract void resetObject(T o);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract T checkOut() throws org.maltparser.core.exception.MaltChainedException;
		public abstract T checkOut();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void checkIn(T o) throws org.maltparser.core.exception.MaltChainedException;
		public abstract void checkIn(T o);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void checkInAll() throws org.maltparser.core.exception.MaltChainedException;
		public abstract void checkInAll();
	}
}