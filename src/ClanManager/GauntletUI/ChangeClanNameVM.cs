using System;

using TaleWorlds.Library;

namespace ClanManager.GauntletUI
{
    internal sealed class ChangeClanNameVM : ViewModel
    {

        private readonly Action _onFinalize;
        //private bool _enableChangeClanNameButton;

        public ChangeClanNameVM(Action onFinalize)
        {
            _onFinalize = onFinalize;

            RefreshValues();
        }

        public void OnCancel()
        {
            _onFinalize.Invoke();
        }

        /*[DataSourceProperty]
        public bool EnableChangeClanNameButton
        {
            get => _enableChangeClanNameButton;

            set { if (value != _enableChangeClanNameButton) { _enableChangeClanNameButton = value; } }
        }*/
    }
}
