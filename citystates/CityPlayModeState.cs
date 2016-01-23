using UnityEngine;
using System.Collections;

public class CityPlayModeState : BaseCityModeState {

    //---- Private Fields ----//
    private CityMoodMusicManager cityMoodMusicManager;

    //---- Public Methods ----/
    public override void Initialize() {
        base.Initialize();

        cityMoodMusicManager = FindObjectOfType<CityMoodMusicManager>();
        if (cityMoodMusicManager != null) {
            cityMoodMusicManager.enabled = false;
        }
    }

    public override void Activate() {
        base.Activate();

        if (cityMoodMusicManager != null) {
            cityMoodMusicManager.enabled = true;
        }
    }

    public override void Deactivate() {
        base.Deactivate();

        if (cityMoodMusicManager != null) {
            cityMoodMusicManager.enabled = false;
        }
    }

    //---- Protected Methods ----/
    protected override bool IsInteractable(CityObject cityObject) {
        if (cityObject == null) { return false; }
        Clickable clickable = cityObject.GetComponent<Clickable>();
        if (clickable != null) {
            if (!clickable.enabled) { return false; }
        } 
        bool isPowerStation = cityObject is PowerStation;
        bool isApp = cityObject is App;
        bool isIOBuilding = cityObject is IOBuilding;
        bool isConnection = cityObject is Connection;
        bool isOrchestrator = cityObject is Orchestrator;
        bool isTransporterStation = cityObject is TransporterStation;
        bool isLion = cityObject is CityLion;
        bool isTerminal = cityObject is ClientTerminal;
        return isPowerStation || isApp || isIOBuilding || isLion || isTerminal || isConnection || isOrchestrator || isTransporterStation;
    }
}