﻿using NMaltParser.Core.Feature;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser.Guide
{
    public interface ClassifierGuide : Guide
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException;
		void addInstance(FeatureModel featureModel, GuideDecision decision);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances() throws org.maltparser.core.exception.MaltChainedException;
		void noMoreInstances();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void predict(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException;
		void predict(FeatureModel featureModel, GuideDecision decision);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector predictExtract(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException;
		FeatureVector predictExtract(FeatureModel featureModel, GuideDecision decision);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector extract(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException;
		FeatureVector extract(FeatureModel featureModel);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predictFromKBestList(org.maltparser.core.feature.FeatureModel featureModel, org.maltparser.parser.history.action.GuideDecision decision) throws org.maltparser.core.exception.MaltChainedException;
		bool predictFromKBestList(FeatureModel featureModel, GuideDecision decision);

		ClassifierGuide_GuideMode GuideMode {get;}
	}

	public enum ClassifierGuide_GuideMode
	{
		BATCH,
		CLASSIFY
	}

}