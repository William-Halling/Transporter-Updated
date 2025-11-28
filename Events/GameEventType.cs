public enum GameEventType
{
    NewHour = 1000,
    NewDay = 1001,

    WorldGenerated,

    // Core Events
    GameStateChanged,
    SceneLoaded,
    SceneLoadStarted,


    GameDataReady, // fired when loaded data should be applied to scene systems

    
    
    SaveCompleted,
    SaveGameRequested,
    LoadCompleted,
    LoadGameRequested,


        // Input Events
    InputMove,
    InputInteract,
    InputPause,


        // Player Events
    //PlayerHealthChanged,
    //PlayerMoneyChanged,


    // Ship Events
    //ShipDamaged,
    //ShipRepaired,
    //ShipSunk,
}