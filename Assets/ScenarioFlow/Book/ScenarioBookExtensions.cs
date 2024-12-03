using System;

namespace ScenarioFlow
{
    public static class ScenarioBookExtensions
    {
        /// <summary>
        /// Changes the current index to the next index.
        /// </summary>
        /// <param name="scenarioBook"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ScenarioBook Flip(this ScenarioBook scenarioBook)
        {
            if (scenarioBook.Remain())
            {
                return scenarioBook.OpenTo(scenarioBook.CurrentIndex + 1);
            }
            else
            {
                throw new InvalidOperationException("No page remains.");
            }
        }
        /// <summary>
        /// Whether the next command to be invoked remains.
        /// </summary>
        /// <param name="scenarioBook"></param>
        /// <returns></returns>
        public static bool Remain(this ScenarioBook scenarioBook)
        {
            return scenarioBook.CurrentIndex + 1 < scenarioBook.Length;
        }
    }
}