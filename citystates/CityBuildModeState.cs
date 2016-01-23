using UnityEngine;
using System.Collections;

public class CityBuildModeState : BaseCityModeState {

    //---- Private Fields ----//
    private SharedCityStoreGuiState sharedCreationGuiState;

    //---- Public Methods ----/
    public override void Initialize() {
        base.Initialize();
        sharedCreationGuiState = FindObjectOfType<SharedCityStoreGuiState>();
    }

    public override void Activate() {
        base.Activate();
        sharedHUDGuiState.Build();

        ManagersSystem ms = FindObjectOfType<ManagersSystem>();
        ms.Get<MusicManager>().SetMusic("Music_Dark");

        sharedCreationGuiState.OnAppBought += OnAppBought;
        sharedCreationGuiState.OnConnectionBought += OnConnectionBought;
        sharedCreationGuiState.OnOrchestratorBought += OnOrchestratorBought;
    }

    //---- Protected Methods ----/
    protected override bool IsInteractable(CityObject cityObject) {
        if (cityObject == null) { return false; }
        Clickable clickable = cityObject.GetComponent<Clickable>();
        if (clickable != null) {
            if (!clickable.enabled) { return false; }
        }
        if (cityObject is Waypoint) { return false; }
        if (cityObject is Cloak) { return false; }
        return true;
    }

    //---- Delegate Handlers ----//
    private void OnAppBought(BuildingApplicationType buildingApplicationType) {
        stateManager.SetStateWithData<CreateAppState>(buildingApplicationType);
    }

    private void OnConnectionBought(ConnectionType connectionType) {
        stateManager.SetStateWithData<CreateConnectionState>(connectionType);
    }

    private void OnOrchestratorBought(OrchestratorBuildingType orchestratorType) {
        stateManager.SetStateWithData<CreateOrchestratorState>(orchestratorType);
    }
}