using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;
using UnityEngine.UI;

public abstract class BaseCityModeState : GameState {

    //---- Protected Fields ----//
    protected CityCamera cityCamera;
    protected CityGrid grid;
    protected City city;
    protected SharedHUDGuiState sharedHUDGuiState;

    //---- Private Fields ----//
    private CityGridColorController gridObject;

    //---- Public Methods ----/
    public override void Initialize() {
        base.Initialize();

        cityCamera = FindObjectOfType<CityCamera>();
        city = GetController<City>();
        sharedHUDGuiState = FindObjectOfType<SharedHUDGuiState>();
        gridObject = city.gameObject.FindChildComponentByName<CityGridColorController>("r_GridLines");
    }

    //---- Protected Methods ----/
    public override void Activate() {
        base.Activate();
        ToggleGrid();
    }

    protected override void OnActivate() {
        base.OnActivate();
        cityCamera.SetMode(typeof(PanningCamera));

        city.OnCityObjectHoverOut += OnHoverOut;
        city.OnCityObjectHoverOver += OnHoverOver;

        sharedHUDGuiState.OnMainMenuButtonClicked += OnCityWantsMainMenu;
        sharedHUDGuiState.OnShareButtonClicked += OnCityWantsShare;
        sharedHUDGuiState.OnRestartButtonClicked += OnCityWantsRestart;

        sharedHUDGuiState.OnTimeButtonClick += OnTimeButtonClick;

        UICamera.onPress += OnMouseClick;
    }

    protected void ToggleGrid() {
        gridObject.Toggle(this);
    }

    protected abstract bool IsInteractable(CityObject cityObject);

    //---- Delegate Handlers ----//
    private void OnTimeButtonClick(SharedHUDGuiState.TimeControl controlType) {
        if (controlType == SharedHUDGuiState.TimeControl.Build) {
            stateManager.SetState<CityBuildModeState>();
            return;
        } 
        
        bool isStatisticMode = stateManager.State.CurrentChild is CityStatisticsState;
        bool isEditMode = stateManager.State.CurrentChild is EditCityObjectState;

        if (isStatisticMode || isEditMode) { return; }

        stateManager.SetState<CityPlayModeState>();
    }

    protected override void OnDeactivate() {
        base.OnDeactivate();

        city.OnCityObjectHoverOut -= OnHoverOut;
        city.OnCityObjectHoverOver -= OnHoverOver;

        sharedHUDGuiState.OnMainMenuButtonClicked -= OnCityWantsMainMenu;
        sharedHUDGuiState.OnShareButtonClicked -= OnCityWantsShare;
        sharedHUDGuiState.OnRestartButtonClicked -= OnCityWantsRestart;

        sharedHUDGuiState.OnTimeButtonClick -= OnTimeButtonClick;

        if (city.HoveredCityObject != null) {
            city.HoveredCityObject.Highlight(false);
        }

        UICamera.onPress -= OnMouseClick;
    }

    protected virtual void OnMouseClick(MouseOrTouch touch, bool isPressed) {
        if (isPressed || touch.dragStarted || UICamera.isOverUI) { return; }
        if (touch.fingerId != 0 || city.HoveredCityObject == null) { return; }
        if (!IsInteractable(city.HoveredCityObject)) { return; }

        // When the player clicks the lion, add some money
        if (city.HoveredCityObject is CityLion) {
            (city.HoveredCityObject as CityLion).Cheat();
            return;
        }

        stateManager.SetStateWithData<EditCityObjectState>(city.HoveredCityObject);
    }

    private void OnHoverOver(CityObject cityObject) {
        if (!IsInteractable(cityObject)) {
            cityObject.Highlight(false);
            return;
        }

        cityObject.Highlight(true);
        AudioManager.Instance.Play(AudioChannelIDX.GameSFX, "GridMove");
    }

    private void OnHoverOut(CityObject cityObject) {
        cityObject.Highlight(false);
    }

    private void OnCityWantsMainMenu() {
        city.Reset();
        ObjectPool.Reset();

        ManagersSystem ms = FindObjectOfType<ManagersSystem>();
        ms.Get<MusicManager>().StopMusic();

        stateManager.SetState<MainMenu>();
    }

    private void OnCityWantsShare() {
        WebCommunicationManager.ShareCurrentGame(city.GenerateXML());
    }

    private void OnCityWantsRestart() {
        FindObjectOfType<BaseScenario>().RetryScenario();
    }

    //---- Private Methods ----//
    private void LoadCity(string xmlData) {
        object newStateData = new CityLoadSettings() { xml = xmlData, isShare = false };
        stateManager.SetStateWithData<CityLoadingState>(newStateData, "", true);
    }
}
