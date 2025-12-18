using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace ClanManager
{
    internal class ClanCreatorData
    {

        [SaveableProperty(1)]
        internal CampaignTime LastTickTime { get; private set; }

        [SaveableProperty(2)]
        internal int LastTickType { get; private set; }

        internal ClanCreatorData()
        {
            LastTickType = 1;
        }

        public bool HasExceededCooldown()
        {
            bool result = false;
            int type = Settings.Current.SpawnIntervalType.SelectedIndex;
            if (LastTickType == type)
            {
                int interval = Settings.Current.SpawnInterval;
                if (type == 0 && LastTickTime.ElapsedHoursUntilNow >= interval)
                {
                    result = true;
                }
                else if (type == 1 && LastTickTime.ElapsedDaysUntilNow >= interval)
                {
                    result = true;
                }
                else if (type == 2 && LastTickTime.ElapsedWeeksUntilNow >= interval)
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
