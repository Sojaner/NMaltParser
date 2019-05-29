using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Feature.Function;
using NMaltParser.Core.Feature.Spec;
using NMaltParser.Core.Feature.Value;

namespace NMaltParser.Core.Feature
{
    [Serializable]
	public class FeatureVector : List<FeatureFunction>
	{
		private readonly SpecificationSubModel specSubModel;

		private readonly FeatureModel featureModel;

		public FeatureVector(FeatureModel featureModel, SpecificationSubModel specSubModel)
		{
			this.specSubModel = specSubModel;

			this.featureModel = featureModel;

			foreach (string spec in this.specSubModel)
			{
				Add(this.featureModel.identifyFeature(spec));
			}
		}

		public virtual SpecificationSubModel SpecSubModel => specSubModel;

        public virtual FeatureModel FeatureModel => featureModel;

        public virtual FeatureValue GetFeatureValue(int index)
		{
			return this[index].FeatureValue;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			foreach (FeatureFunction function in this)
			{
				if (function != null)
				{
					stringBuilder.Append(function.FeatureValue);

					stringBuilder.Append('\n');
				}
			}

			return stringBuilder.ToString();
		}
	}

}