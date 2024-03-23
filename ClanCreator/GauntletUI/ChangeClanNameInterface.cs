using HarmonyLib.BUTR.Extensions;
using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace ClanManager.GauntletUI
{
    internal class ChangeClanNameInterface
    {
        protected bool _isShown = false;

        protected static readonly LoadMovie? loadMovie =
            AccessTools2.GetDelegate<LoadMovie>(typeof(GauntletLayer), "LoadMovie");

        protected static readonly ReleaseMovie? releaseMovie =
            AccessTools2.GetDelegate<ReleaseMovie>(typeof(GauntletLayer), "ReleaseMovie");


        protected GauntletLayer _layer = default!;

        protected object? _movie;
        protected ScreenBase _screenBase = default!;
        protected ViewModel? _vm;

        private Action? _onRefresh;

        protected string _name => "ChangeNameEncyclopediaClanPage";

        public void ShowChangeClanNameInterface(ScreenBase screenBase, Action onRefresh)
        {
            if (!_isShown)
                return;


            _screenBase = screenBase;

            _onRefresh = onRefresh;

            UIResourceManager.SpriteData.SpriteCategories["ui_clan"].Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);

            _layer = new GauntletLayer(211);
            _layer.InputRestrictions.SetInputRestrictions();
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
            screenBase.AddLayer(_layer);
            _vm = new ChangeClanNameVM(OnFinalize);
            _movie = loadMovie?.Invoke(_layer, _name, _vm);
        }

        public void ShowChangeClanNameInterface(ScreenBase screenBase)
        {
            ShowChangeClanNameInterface(screenBase, () => { });
        }

        protected virtual void OnFinalize()
        {
            _screenBase.RemoveLayer(_layer);
            if (_movie != null && releaseMovie != null) releaseMovie?.Invoke(_layer, _movie);
            _layer = null!;
            _movie = null!;
            _vm = null!;
            _screenBase = null!;
            _isShown = false;
            _onRefresh!();
        }

        protected delegate object LoadMovie(object instance, string name, ViewModel dataSource);


        protected delegate void ReleaseMovie(object instance, object movie);
    }
}
