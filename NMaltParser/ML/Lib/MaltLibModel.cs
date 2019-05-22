namespace org.maltparser.ml.lib
{
	public interface MaltLibModel
	{
		int[] predict(MaltFeatureNode[] x);
		int predict_one(MaltFeatureNode[] x);
	}

}