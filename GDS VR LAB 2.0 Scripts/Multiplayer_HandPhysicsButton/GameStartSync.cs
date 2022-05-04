using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Collections;
using Normal.Realtime.Serialization;
using System;

//*
[Serializable]
public class GameStartToggle
{
    //public uint _key;
    public uint _gameKey; // - model.gameNumber_button
    public bool _gameStart; // - model.isStart_button

    public GameStartToggle(uint key, bool start)
    {
        _gameKey = key;
        _gameStart = start;
    }
}

public class GameStartSync : RealtimeComponent<GameButtonModel>
{
    [SerializeField]
    private ActiveGames _activeObjects;

    [SerializeField]
    int totalGameNumbers;
    [SerializeField]
    public List<GameStartToggle> gameStartToggle_List = new List<GameStartToggle>();

    private void Awake()
    {
        for (int i = 0; i < totalGameNumbers + 1; i++)
        {
            gameStartToggle_List.Add(new GameStartToggle((uint)i, false));
        }
    }

    void Start()
    {
        foreach(GameStartToggle gameStartToggle in gameStartToggle_List)
        {
            AddGameButtonToggle(gameStartToggle._gameKey, false);
        }
    }

    public void AddGameButtonToggle(uint index, bool start)
    {
        StartToggleDicModel startToggle = new StartToggleDicModel();
        startToggle.gameIndex = index;
        startToggle.gameStart = start;
        model.d_GameStartToggle.Add(startToggle.gameIndex, startToggle);
        //Debug.Log("game key:" + model.d_GameStartToggle[index].gameIndex + " game start: " + model.d_GameStartToggle[index].gameStart);
    }

    private void Update()
    {
        #region test
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ReplaceGameStartToggle(1);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ReplaceGameStartToggle(2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            ReplaceGameStartToggle(3);
        }
        #endregion
    }

    #region sync model
    protected override void OnRealtimeModelReplaced(GameButtonModel previousModel, GameButtonModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.d_GameStartToggle.modelReplaced -= StartToggleReplaced;
        }

        if (currentModel != null)
        {
            currentModel.d_GameStartToggle.modelReplaced += StartToggleReplaced;
        }

    }


    private void StartToggleReplaced(RealtimeDictionary<StartToggleDicModel> dictionary, uint key, StartToggleDicModel oldModel, StartToggleDicModel newModel, bool remote)
    {        
        ReplaceToggleList(key, newModel.gameStart);
        
        //Debug.Log("add fire key: " + key + "; add fire new model start: " + newModel.gameStart);
    }

    private void ReplaceToggleList(uint key, bool start)
    {
        for (int i = 0; i < gameStartToggle_List.Count; i++)
        {
            if (gameStartToggle_List[i]._gameKey == key)
            {
                gameStartToggle_List.Remove(gameStartToggle_List[i]);
                gameStartToggle_List.Add(new GameStartToggle(key, start));
                Debug.Log("add fire key: " + key + "; add fire new model start: " + start);
                break;
            }
        }
        _activeObjects.ActiveGameObject(key, start);
    }
    #endregion

    public void ReplaceGameStartToggle(int gameId)
    {
        uint gameKey = (uint)gameId;
        
        for (int i = 0; i < gameStartToggle_List.Count; i++)
        {
            if (gameStartToggle_List[i]._gameKey == gameKey)
            {
                bool previousToggle = gameStartToggle_List[i]._gameStart;
                gameStartToggle_List.Remove(gameStartToggle_List[i]);
                bool nextToggle = !previousToggle;
                gameStartToggle_List.Add(new GameStartToggle(gameKey, nextToggle));

                if (model.d_GameStartToggle.ContainsKey(gameKey))
                {
                    model.d_GameStartToggle.Remove(gameKey);
                    StartToggleDicModel startToggle = new StartToggleDicModel();
                    startToggle.gameIndex = gameKey;
                    startToggle.gameStart = nextToggle;
                    model.d_GameStartToggle.Add(gameKey, startToggle);
                    _activeObjects.ActiveGameObject(gameKey, nextToggle);
                    //Debug.Log("game key:" + model.d_GameStartToggle[gameKey].gameIndex + " game start: " + model.d_GameStartToggle[gameKey].gameStart);
                    Debug.Log("press button");
                    break;
                }                
            }

        }
        
    }
}
//*/