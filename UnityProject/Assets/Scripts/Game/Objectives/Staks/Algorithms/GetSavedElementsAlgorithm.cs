using System.Linq;
using Extensions;

namespace Game.Objectives.Stacks
{
    public partial class ObjectivesStack
    {
        private OjbectiveStackData GetSavedByName(string objectveName)
        {
            var answer = _Datas.Find(x => x.ObjectiveName == objectveName);
            return answer;
        }
    }
}