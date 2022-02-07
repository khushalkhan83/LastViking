using Game.Models;

namespace Game.Controllers
{
    public class TokenMineableStoneController : TokenMineableControllerBase, ITokenMineableStoneController
    {
        protected override int TokenConfigId => 12;
        protected override string TockenID => "toeken_stone";
        protected override OutLineMinableObjectID outLineMinableObjectID => OutLineMinableObjectID.Stone;
    }
}
