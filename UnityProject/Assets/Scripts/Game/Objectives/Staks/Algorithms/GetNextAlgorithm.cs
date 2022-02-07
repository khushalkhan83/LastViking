using System.Linq;
using Extensions;

namespace Game.Objectives.Stacks
{
    public partial class ObjectivesStack
    {
        private OjbectiveStackData GetFirstNotCompleatedElement()
        {
            var notCompleatedObjectives = _Datas.Where(x => x.Done == false);
            var answer = notCompleatedObjectives.FirstOrDefault();
            return answer;
        }
        private OjbectiveStackData GetRandomElement()
        {
            var answer = _Datas.RandomElement();
            return answer;
        }

        private OjbectiveStackData GetFirstElementWithTrueConditions()
        {
            var answer = _Datas.Where(x => x.ConditionsOk).FirstOrDefault();
            return answer;
        }

        private OjbectiveStackData GetLastElement()
        {
            return _Datas.LastOrDefault();
        }
    }
}