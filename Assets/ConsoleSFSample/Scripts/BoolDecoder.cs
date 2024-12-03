using ScenarioFlow;

public class BoolDecoder : IReflectable
{
	[DecoderMethod]
	public bool ConvertToBool(string input)
	{
		return bool.Parse(input);
	}
}