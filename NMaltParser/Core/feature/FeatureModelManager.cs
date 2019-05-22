using NMaltParser.Core.Feature.Spec;
using NMaltParser.Core.Feature.System;

namespace NMaltParser.Core.Feature
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class FeatureModelManager
	{
		private readonly SpecificationModels specModels;
		private readonly FeatureEngine featureEngine;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FeatureModelManager(org.maltparser.core.feature.system.FeatureEngine engine) throws org.maltparser.core.exception.MaltChainedException
		public FeatureModelManager(FeatureEngine engine)
		{
			specModels = new SpecificationModels();
			featureEngine = engine;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadSpecification(java.net.URL specModelURL) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadSpecification(URL specModelURL)
		{
			specModels.load(specModelURL);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadParSpecification(java.net.URL specModelURL, String markingStrategy, String coveredRoot) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadParSpecification(URL specModelURL, string markingStrategy, string coveredRoot)
		{
			specModels.loadParReader(specModelURL, markingStrategy, coveredRoot);
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FeatureModel getFeatureModel(java.net.URL specModelURL, int specModelUrlIndex, FeatureRegistry registry, String dataSplitColumn, String dataSplitStructure) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureModel getFeatureModel(URL specModelURL, int specModelUrlIndex, FeatureRegistry registry, string dataSplitColumn, string dataSplitStructure)
		{
			return new FeatureModel(specModels.getSpecificationModel(specModelURL, specModelUrlIndex), registry, featureEngine, dataSplitColumn, dataSplitStructure);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FeatureModel getFeatureModel(java.net.URL specModelURL, FeatureRegistry registry, String dataSplitColumn, String dataSplitStructure) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureModel getFeatureModel(URL specModelURL, FeatureRegistry registry, string dataSplitColumn, string dataSplitStructure)
		{
			return new FeatureModel(specModels.getSpecificationModel(specModelURL, 0), registry, featureEngine, dataSplitColumn, dataSplitStructure);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FeatureModel getFeatureModel(org.maltparser.core.feature.spec.SpecificationModel specModel, FeatureRegistry registry, String dataSplitColumn, String dataSplitStructure) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureModel getFeatureModel(SpecificationModel specModel, FeatureRegistry registry, string dataSplitColumn, string dataSplitStructure)
		{
			return new FeatureModel(specModel, registry, featureEngine, dataSplitColumn, dataSplitStructure);
		}

		public virtual SpecificationModels SpecModels
		{
			get
			{
				return specModels;
			}
		}

		public virtual FeatureEngine FeatureEngine
		{
			get
			{
				return featureEngine;
			}
		}


		public override string ToString()
		{
			return specModels.ToString();
		}
	}

}