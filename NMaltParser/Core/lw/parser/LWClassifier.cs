using System.Collections.Generic;

namespace org.maltparser.core.lw.parser
{
    using  feature;
	using  feature.value;
    using  ml.lib;
    using  org.maltparser.parser.history.action;

	/// <summary>
	/// A lightweight version of org.maltparser.ml.lib.{Lib,LibLinear,LibSvm} and can only predict the next transition.
	/// 
	/// @author Johan Hall
	/// </summary>
	public class LWClassifier
	{
		private readonly FeatureMap featureMap;
		private readonly bool excludeNullValues;
		private readonly MaltLibModel model;

		public LWClassifier(McoModel mcoModel, string prefixFileName, bool _excludeNullValues)
		{
			model = (MaltLibModel)mcoModel.getMcoEntryObject(prefixFileName + ".moo");
			featureMap = (FeatureMap)mcoModel.getMcoEntryObject(prefixFileName + ".map");
			excludeNullValues = _excludeNullValues;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision, boolean one_prediction) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureVector featureVector, SingleDecision decision, bool one_prediction)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<org.maltparser.ml.lib.MaltFeatureNode> featureList = new java.util.ArrayList<org.maltparser.ml.lib.MaltFeatureNode>();
			List<MaltFeatureNode> featureList = new List<MaltFeatureNode>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = featureVector.size();
			int size = featureVector.Count;
			for (int i = 1; i <= size; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.FeatureValue featureValue = featureVector.getFeatureValue(i-1);
				FeatureValue featureValue = featureVector.getFeatureValue(i - 1);
				if (featureValue != null && !(excludeNullValues == true && featureValue.NullValue))
				{
					if (!featureValue.Multiple)
					{
						SingleFeatureValue singleFeatureValue = (SingleFeatureValue)featureValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = featureMap.getIndex(i, singleFeatureValue.getIndexCode());
						int index = featureMap.getIndex(i, singleFeatureValue.IndexCode);
						if (index != -1 && singleFeatureValue.Value != 0)
						{
							featureList.Add(new MaltFeatureNode(index,singleFeatureValue.Value));
						}
					}
					else
					{
						foreach (int? value in ((MultipleFeatureValue)featureValue).Codes)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int v = featureMap.getIndex(i, value);
							int v = featureMap.getIndex(i, value.Value);
							if (v != -1)
							{
								featureList.Add(new MaltFeatureNode(v,1));
							}
						}
					}
				}
			}
			try
			{
				if (one_prediction)
				{
					decision.KBestList.add(model.predict_one(featureList.ToArray()));
				}
				else
				{
					decision.KBestList.addList(model.predict(featureList.ToArray()));
				}
			}
			catch (System.OutOfMemoryException e)
			{
				throw new LibException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
			}
			return true;
		}
	}

}