using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System;
using UnityEngine;

namespace ConsoleSFSample
{
	/// <summary>
	/// Provides a decoder for the 'Color' type.
	/// </summary>
	public class ColorDecoder : IReflectable
	{
		[DecoderMethod]
		[Description("A decoder for the 'Color' type.")]
		[Description("Convert a hex color code to the 'Color' type.")]
		[Description("e.g. '#00FF00', '#112233'")]
		public Color ConvertHexToColor(string input)
		{
			return ColorUtility.TryParseHtmlString(input, out var color) ? color : throw new ArgumentException($"Failed to convert '{input}' to Color.");
		}
	}
}
