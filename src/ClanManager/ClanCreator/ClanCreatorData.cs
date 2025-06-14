using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace ClanManager.ClanCreator
{
    internal class ClanCreatorData
    {

        [SaveableProperty(1)]
        internal CampaignTime LastTickTime { get; private set; }

        [SaveableProperty(2)]
        internal int LastTickType { get; private set; }

        internal ClanCreatorData()
        {
            LastTickTime = CampaignTime.Now;
            LastTickType = 1;
        }

        public bool HasExceededCooldown()
        {
            bool result = false;
            int type = Settings.Current.SpawnIntervalType.SelectedIndex;
            if (LastTickType == type)
            {
                if (type == 0 && LastTickTime.ElapsedHoursUntilNow >= Settings.Current.SpawnInterval)
                {
                    result = true;
                }
                else if (type == 1 && LastTickTime.ElapsedDaysUntilNow >= Settings.Current.SpawnInterval)
                {
                    result = true;
                }
                else if (type == 2 && LastTickTime.ElapsedWeeksUntilNow >= Settings.Current.SpawnInterval)
                {
                    result = true;
                }
            }
            if (result || type != LastTickType)
            {
                LastTickTime = CampaignTime.Now;
            }
            LastTickType = type;
            return result;
        }
    }
}
