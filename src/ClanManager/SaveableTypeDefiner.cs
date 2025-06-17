using ClanManager.ClanCreator;

namespace ClanManager
{
    internal class SaveableTypeDefiner : TaleWorlds.SaveSystem.SaveableTypeDefiner
    {
        public SaveableTypeDefiner() : base((0xd41318 << 8) | 123) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(ClanCreatorData), 1);
        }
    }
}