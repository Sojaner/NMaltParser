using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.feature
{

	using  exception;
	using  function;
	using  spec;
	using  value;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	[Serializable]
	public class FeatureVector : List<FeatureFunction>
	{
		public const long serialVersionUID = 3256444702936019250L;
		private readonly SpecificationSubModel specSubModel;
		private readonly FeatureModel featureModel;

		/// <summary>
		/// Constructs a feature vector
		/// </summary>
		/// <param name="_featureModel">	the parent feature model </param>
		/// <param name="_specSubModel">	the subspecifiction-model </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FeatureVector(FeatureModel _featureModel, org.maltparser.core.feature.spec.SpecificationSubModel _specSubModel) throws org.maltparser.core.exception.MaltChainedException
		public FeatureVector(FeatureModel _featureModel, SpecificationSubModel _specSubModel)
		{
			specSubModel = _specSubModel;
			featureModel = _featureModel;
			foreach (string spec in specSubModel)
			{
				Add(featureModel.identifyFeature(spec));
			}
		}

		/// <summary>
		/// Returns the subspecifiction-model.
		/// </summary>
		/// <returns> the subspecifiction-model </returns>
		public virtual SpecificationSubModel SpecSubModel
		{
			get
			{
				return specSubModel;
			}
		}

		/// <summary>
		/// Returns the feature model that the feature vector belongs to.
		/// </summary>
		/// <returns> the feature model that the feature vector belongs to </returns>
		public virtual FeatureModel FeatureModel
		{
			get
			{
				return featureModel;
			}
		}

		public virtual FeatureValue getFeatureValue(int index)
		{
			return this[index].FeatureValue;
		}

		/* (non-Javadoc)
		 * @see java.util.AbstractCollection#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (FeatureFunction function in this)
			{
				if (function != null)
				{
					sb.Append(function.FeatureValue.ToString());
					sb.Append('\n');
				}
			}
			return sb.ToString();
		}
	}

}