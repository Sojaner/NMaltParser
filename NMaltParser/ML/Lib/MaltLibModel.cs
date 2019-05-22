namespace NMaltParser.ML.Lib
{
	public interface MaltLibModel
	{
		int[] predict(MaltFeatureNode[] x);
		int predict_one(MaltFeatureNode[] x);
	}

}