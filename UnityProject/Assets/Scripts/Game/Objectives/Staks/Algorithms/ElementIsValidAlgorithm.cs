using System.Linq;
using Extensions;

namespace Game.Objectives.Stacks
{
    public partial class ObjectivesStack
    {
        private bool NotCompleatedElementIsValid(OjbectiveStackData data)
        {
            return !data.Done;
        }

        private bool AnyElementIsValid(OjbectiveStackData data)
        {
            return true;
        }
    }
}