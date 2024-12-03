using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScenarioFlow.Scripts.Composite
{
	[CreateAssetMenu(fileName = "CompositeScenarioScript", menuName = "ScenarioFlow/Composite Scenario Script")]
    public class CompositeScenarioScript : ScenarioScript
    {
        [SerializeField]
        private ScenarioScript[] scenarioScripts;

        public override IEnumerable<IEnumerable<string>> Lines => scenarioScripts.SelectMany(s => s.Lines);
        public override IReadOnlyDictionary<string, int> LabelDictionary
        {
            get
            {
                var labels = scenarioScripts.SelectMany(s => s.LabelDictionary.Keys);
                var distinctLabels = labels.Distinct();
                if (labels.Count() != distinctLabels.Count())
                {
                    var duplicateLabels = distinctLabels.Where(x => labels.Where(label => label == x).Count() > 1);
                    throw new FormatException($"Label duplication was detected: {string.Join(", ", duplicateLabels)}");
                }
                else
                {
                    var lineCounts = scenarioScripts.Select(s => s.Lines.Count()).ToArray();
                    return new Dictionary<string, int>(scenarioScripts.SelectMany((s, i) => s.LabelDictionary.ToDictionary(pair => pair.Key, pair => pair.Value + lineCounts.Take(i).Sum())));
                }
            }
        }

        public IEnumerable<string> SearchDuplicatedLabelAll()
        {
            var labels = new List<string>();
            foreach (var label in scenarioScripts.SelectMany(s => s.LabelDictionary.Keys))
            {
                if (labels.Contains(label))
                {
                    yield return label;
                }
                else
                {
                    labels.Add(label);
                }
            }
		}
	}
}