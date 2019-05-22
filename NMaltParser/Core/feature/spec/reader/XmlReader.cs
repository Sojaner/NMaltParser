namespace NMaltParser.Core.Feature.Spec.Reader
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class XmlReader : FeatureSpecReader
	{

		public XmlReader()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.net.URL specModelURL, org.maltparser.core.feature.spec.SpecificationModels featureSpecModels) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(URL specModelURL, SpecificationModels featureSpecModels)
		{
			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				Element root = null;

				root = db.parse(specModelURL.openStream()).DocumentElement;

				if (root == null)
				{
					throw new FeatureException("The feature specification file '" + specModelURL.File + "' cannot be found. ");
				}

				readFeatureModels(root, featureSpecModels);
			}
			catch (IOException e)
			{
				throw new FeatureException("The feature specification file '" + specModelURL.File + "' cannot be found. ", e);
			}
			catch (SAXParseException e)
			{
				throw new FeatureException("Problem parsing the feature specification XML-file " + specModelURL.File + ". ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new FeatureException("Problem parsing the feature specification XML-file " + specModelURL.File + ". ", e);
			}
			catch (SAXException e)
			{
				throw new FeatureException("Problem parsing the feature specification XML-file " + specModelURL.File + ". ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readFeatureModels(org.w3c.dom.Element featuremodels, org.maltparser.core.feature.spec.SpecificationModels featureSpecModels) throws org.maltparser.core.exception.MaltChainedException
		private void readFeatureModels(Element featuremodels, SpecificationModels featureSpecModels)
		{
			NodeList featureModelList = featuremodels.getElementsByTagName("featuremodel");
			for (int i = 0; i < featureModelList.Length; i++)
			{
				readFeatureModel((Element)featureModelList.item(i), featureSpecModels);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readFeatureModel(org.w3c.dom.Element featuremodel, org.maltparser.core.feature.spec.SpecificationModels featureSpecModels) throws org.maltparser.core.exception.MaltChainedException
		private void readFeatureModel(Element featuremodel, SpecificationModels featureSpecModels)
		{
			int specModelIndex = featureSpecModels.NextIndex;
			NodeList submodelList = featuremodel.getElementsByTagName("submodel");
			if (submodelList.Length == 0)
			{
				NodeList featureList = featuremodel.getElementsByTagName("feature");
				for (int i = 0; i < featureList.Length; i++)
				{
					string featureText = ((Element)featureList.item(i)).TextContent.Trim();
					if (featureText.Length > 1)
					{
						featureSpecModels.add(specModelIndex, featureText);
					}
				}
			}
			else
			{
				for (int i = 0; i < submodelList.Length; i++)
				{
					string name = ((Element)submodelList.item(i)).getAttribute("name");
					NodeList featureList = ((Element)submodelList.item(i)).getElementsByTagName("feature");
					for (int j = 0; j < featureList.Length; j++)
					{
						string featureText = ((Element)featureList.item(j)).TextContent.Trim();
						if (featureText.Length > 1)
						{
							featureSpecModels.add(specModelIndex, name, featureText);
						}
					}
				}
			}
		}
	}

}