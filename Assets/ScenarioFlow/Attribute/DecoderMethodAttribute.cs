using System;

namespace ScenarioFlow
{
    /// <summary>
    /// A method with this attribute is exported as a decoder.
    /// The method must have only single 'string' parameter and return a converted value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DecoderMethodAttribute : Attribute
    {

    }
}