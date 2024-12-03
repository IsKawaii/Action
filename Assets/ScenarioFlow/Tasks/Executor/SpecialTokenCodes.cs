namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Provides the symbols of the special token code and functions about them.
    /// </summary>
    public static class SpecialTokenCodes
    {
        //Special token codes
        public static readonly string StandardTokenCode = "standard";
        public static readonly string FluentStandardTokenCode = "f-standard";
        public static readonly string SerialStandardTokenCode = "s-standard";
        public static readonly string ParallelStandardTokenCode = "p-standard";
        public static readonly string ForcedTokenCode = "forced";
        public static readonly string FluentForcedTokenCode = "f-forced";
        public static readonly string SerialForcedTokenCode = "s-forced";
        public static readonly string ParallelForcedTokenCode = "p-forced";
        public static readonly string PromisedTokenCode = "promised";
        public static readonly string FluentPromisedTokenCode = "f-promised";
        public static readonly string SerialPromisedTokenCode = "s-promised";
        public static readonly string ParallelPromisedTokenCode = "p-promised";

        public static bool IsSpecial(string tokenCode)
        {
            return tokenCode == StandardTokenCode ||
                tokenCode == FluentStandardTokenCode ||
                tokenCode == SerialStandardTokenCode ||
                tokenCode == ParallelStandardTokenCode ||
                tokenCode == ForcedTokenCode ||
                tokenCode == FluentForcedTokenCode ||
                tokenCode == SerialForcedTokenCode ||
                tokenCode == ParallelForcedTokenCode ||
                tokenCode == PromisedTokenCode ||
                tokenCode == FluentPromisedTokenCode ||
                tokenCode == SerialPromisedTokenCode ||
                tokenCode == ParallelPromisedTokenCode;
        }

        public static bool IsStandard(string tokenCode)
        {
            return tokenCode == StandardTokenCode ||
                tokenCode == FluentStandardTokenCode ||
                tokenCode == SerialStandardTokenCode ||
                tokenCode == ParallelStandardTokenCode;
        }

        public static bool IsForced(string tokenCode)
        {
            return tokenCode == ForcedTokenCode ||
                tokenCode == FluentForcedTokenCode ||
                tokenCode == SerialForcedTokenCode ||
                tokenCode == ParallelForcedTokenCode;
        }

        public static bool IsPromised(string tokenCode)
        {
            return tokenCode == PromisedTokenCode ||
                tokenCode == FluentPromisedTokenCode ||
                tokenCode == SerialPromisedTokenCode ||
                tokenCode == ParallelPromisedTokenCode;
        }

        public static bool IsPlain(string tokenCode)
        {
            return tokenCode == StandardTokenCode ||
                tokenCode == ForcedTokenCode ||
                tokenCode == PromisedTokenCode;
        }

        public static bool IsFluent(string tokenCode)
        {
            return tokenCode == FluentStandardTokenCode ||
                tokenCode == FluentForcedTokenCode ||
                tokenCode == FluentPromisedTokenCode;
        }

        public static bool IsSerial(string tokenCode)
        {
            return tokenCode == SerialStandardTokenCode ||
                tokenCode == SerialForcedTokenCode ||
                tokenCode == SerialPromisedTokenCode;
        }

        public static bool IsParallel(string tokenCode)
        {
            return tokenCode == ParallelStandardTokenCode ||
                tokenCode == ParallelForcedTokenCode ||
                tokenCode == ParallelPromisedTokenCode;
        }
    }
}