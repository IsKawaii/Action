using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ScenarioFlow.Scripts
{
    /// <summary>
    /// A scenario script that inherits the ScriptableObject.
    /// </summary>
    public abstract class ScenarioScript : ScriptableObject, IScenarioScript
    {
        public abstract IEnumerable<IEnumerable<string>> Lines { get; }
        public abstract IReadOnlyDictionary<string, int> LabelDictionary { get; }


        [Serializable]
        protected class Line
        {
            [SerializeField]
            private string[] elements;
            public IEnumerable<string> Elements => elements;

            public Line(IEnumerable<string> elements)
            {
                if (elements == null)
                {
                    throw new ArgumentNullException(nameof(elements));
                }
                this.elements = elements.ToArray();
            }
        }

        [Serializable]
        protected class Label
        {
            [SerializeField]
            private string name;
            public string Name => name;
            [SerializeField]
            private int index;
            public int Index => index;

            public Label(string name, int index)
            {
                this.name = name;
                this.index = index;
            }
        }
    }
}