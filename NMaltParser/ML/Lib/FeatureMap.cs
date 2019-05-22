using System;

namespace org.maltparser.ml.lib
{


	using HashMap = org.maltparser.core.helper.HashMap;

	/// <summary>
	/// The purpose of the feature map is to map MaltParser's column based features together with the symbol code from the symbol table to 
	/// unique indices suitable for liblinear and libsvm.  A feature column position are combined together with the symbol code in a 
	/// 64-bit key (Long), where 16 bits are reserved for the position and 48 bits are reserved for the symbol code.  
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	[Serializable]
	public class FeatureMap
	{
		private const long serialVersionUID = 7526471155622776147L;
		private readonly HashMap<long, int> map;
		private int featureCounter;

		/// <summary>
		/// Creates a feature map and sets the feature counter to 1
		/// </summary>
		public FeatureMap()
		{
			map = new HashMap<long, int>();
			this.featureCounter = 1;
		}

		/// <summary>
		/// Adds a mapping from a combination of the position in the column-based feature vector and the symbol code to 
		/// an index value suitable for liblinear and libsvm.
		/// </summary>
		/// <param name="featurePosition"> a position in the column-based feature vector </param>
		/// <param name="code"> a symbol code </param>
		/// <returns> the index value  </returns>
		public virtual int addIndex(int featurePosition, int code)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long key = ((((long)featurePosition) << 48) | (long)code);
			long key = ((((long)featurePosition) << 48) | (long)code);
			int? index = map[key];
			if (index == null)
			{
				index = featureCounter++;
				map[key] = index;
			}
			return index.Value;
		}

		/// <summary>
		/// Return
		/// </summary>
		/// <param name="featurePosition"> the position in the column-based feature vector </param>
		/// <param name="code"> the symbol code suitable for liblinear and libsvm </param>
		/// <returns> the index value if it exists, otherwise -1 </returns>
		public virtual int getIndex(int featurePosition, int code)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<int> index = map.get(((((long)featurePosition) << 48) | (long)code));
			int? index = map[((((long)featurePosition) << 48) | (long)code)];
			return (index == null)?-1:index.Value;
		}


		public virtual int addIndex(int featurePosition, int code1, int code2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long key = ((((long)featurePosition) << 48) | (((long)code1) << 24) | (long)code2);
			long key = ((((long)featurePosition) << 48) | (((long)code1) << 24) | (long)code2);
			int? index = map[key];
			if (index == null)
			{
				index = featureCounter++;
				map[key] = index;
			}
			return index.Value;
		}

		public virtual int setIndex(long key, int index)
		{
			return map[key] = index;
		}

		public virtual int decrementIndex(long? key)
		{
			int? index = map[key];
			if (index != null)
			{
				map[key] = index - 1;
			}
			return (index != null)?index - 1 : -1;
		}

		public virtual void decrementfeatureCounter()
		{
			featureCounter--;
		}

		public virtual int? removeIndex(long key)
		{
			return map.Remove(key);
		}

		public virtual int getIndex(int featurePosition, int code1, int code2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<int> index = map.get(((((long)featurePosition) << 48) | (((long)code1) << 24) | (long)code2));
			int? index = map[((((long)featurePosition) << 48) | (((long)code1) << 24) | (long)code2)];
			return (index == null)?-1:index.Value;
		}

		/// <returns> the size of the map </returns>
		public virtual int size()
		{
			return map.Count;
		}


		public virtual long?[] reverseMap()
		{
			long?[] reverseMap = new long?[map.Count + 1];

			foreach (long? key in map.Keys)
			{
				reverseMap[map[key]] = key;
			}
			return reverseMap;
		}



		public virtual int FeatureCounter
		{
			set
			{
				this.featureCounter = value;
			}
			get
			{
				return featureCounter;
			}
		}

	}

}