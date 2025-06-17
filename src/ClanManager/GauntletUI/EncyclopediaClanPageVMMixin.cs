using System.Linq;
using System.Reflection;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

using JetBrains.Annotations;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;

namespace ClanManager.GauntletUI
{
    [ViewModelMixin]
    [UsedImplicitly]
    public class EncyclopediaClanPageVMMixin : BaseViewModelMixin<EncyclopediaClanPageVM>
    {
        private bool _canChangeClanName;
        private readonly ChangeClanNameInterface _changeClanNameInterface;
        private readonly PropertyChangedWithValueEventHandler _eventHandler;

        public EncyclopediaClanPageVMMixin(EncyclopediaClanPageVM vm) : base(vm)
        {

            _changeClanNameInterface = new ChangeClanNameInterface();

            _eventHandler = new PropertyChangedWithValueEventHandler(OnPropertyChangedWithValue);
            ViewModel!.PropertyChangedWithValue += _eventHandler;

            RefreshCanChangeClanName();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            ViewModel!.PropertyChangedWithValue -= _eventHandler;
        }

        private void OnPropertyChangedWithValue(object sender, PropertyChangedWithValueEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.NameText))
            {
                RefreshCanChangeClanName();
            }
        }

        private void RefreshCanChangeClanName()
        {
            ViewModel!.Refresh();
            CanChangeClanName = Settings.Current.EnableEncyclopediaTweaks;
            if (CanChangeClanName)
            {
                Clan c = Clan.All.Where((c) => c.Name.ToString() == ViewModel!.GetName()).FirstOrDefault();
                _changeClanNameInterface.ShowChangeClanNameInterface(ScreenManager.TopScreen);

            }
        }

        [DataSourceMethod]
        public void ChangeClanName()
        {
            GameTexts.SetVariable("MAX_LETTER_COUNT", 50);
            GameTexts.SetVariable("MIN_LETTER_COUNT", 1);
            InformationManager.ShowTextInquiry(new TextInquiryData(GameTexts.FindText("str_change_clan_name", null).ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), OnChangeClanNameDone, null, false, FactionHelper.IsClanNameApplicable, "", ""), false, false);
        }

        private void OnChangeClanNameDone(string newClanName)
        {
            TextObject textObject = GameTexts.FindText("str_generic_clan_name", null);
            textObject.SetTextVariable("CLAN_NAME", new TextObject(newClanName, null));
            Clan _clan = (Clan)ViewModel!.GetType().GetField("_clan", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ViewModel!);
            _clan.ChangeClanName(textObject, textObject);
            ViewModel!.Refresh();
        }

        [DataSourceProperty]
        public bool CanChangeClanName
        {
            get => _canChangeClanName;
            set
            {
                if (value != _canChangeClanName)
                {
                    _canChangeClanName = value;
                    ViewModel!.OnPropertyChanged(nameof(_canChangeClanName));
                }
            }
        }
    }
}
