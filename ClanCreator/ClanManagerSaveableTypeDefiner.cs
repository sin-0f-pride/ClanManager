using ClanManager.ClanCreator;
using TaleWorlds.SaveSystem;

namespace ClanManager
{
    internal class ClanManagerSaveableTypeDefiner : SaveableTypeDefiner
    {
        public ClanManagerSaveableTypeDefiner() : base((0xd41318 << 8) | 123) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(ClanCreatorData), 1);
        }
    }
}
