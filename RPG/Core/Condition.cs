using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] private Disjunction[] and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (var pred in and)
            {
                if (!pred.Check(evaluators)) return false;
            }

            return true;
        }

        [System.Serializable]
        public class Disjunction
        {
            [SerializeField] private Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var pred in or)
                {
                    if (pred.Check(evaluators)) return true;
                }
                return false;
            }
        }
        
        [System.Serializable]
        public class Predicate
        {
            [SerializeField] private string predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    var result = evaluator.Evaluate(predicate, parameters);
                    if(result == null) continue;
                    if (result == negate) return false;
                }
                return true;
            }    
        }
        
    }
}
