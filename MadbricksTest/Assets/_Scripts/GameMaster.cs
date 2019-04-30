using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System;

public class GameMaster : MonoBehaviour
{
    readonly string DataKey = "Data";

    [SerializeField] TMPro.TMP_InputField LoginInput = null;
    [SerializeField] TMPro.TMP_InputField DataInput = null;
    [SerializeField] Button LoginButton = null;
    [SerializeField] Button SaveButton = null;

    string PlayfabId = null;

    void Start()
    {
        PlayFabSettings.TitleId = "39114";
        DataInput.interactable = false;
        SaveButton.interactable = false;
    }

    public void Login()
    {
        ActivateElements(false);

        var user = LoginInput.text;
        var request = new LoginWithCustomIDRequest { CustomId = user, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request,
            (result) =>
            {
                Debug.Log("Logged in");
                PlayfabId = result.PlayFabId;
                Load();
            },
            OnError);
    }

    void Load()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request,
            (result) =>
            {
                Debug.Log("Data obtained");
                if (result.Data.TryGetValue(DataKey, out UserDataRecord dataRecord))
                    DataInput.text = dataRecord.Value;
                else
                    DataInput.text = "";
                ActivateElements(true);
            },
            OnError);
    }

    public void Save()
    {
        ActivateElements(false);

        var request = new UpdateUserDataRequest();

        request.Data = new Dictionary<string, string>();
        request.Data.Add(DataKey, DataInput.text);

        PlayFabClientAPI.UpdateUserData(request,
            (result) =>
            {
                Debug.Log("Data saved");
                ActivateElements(true);
            },
            OnError);
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Playfab Error");
        Debug.LogWarning(error.GenerateErrorReport());
        ActivateElements(true);
    }

    void ActivateElements(bool active)
    {
        LoginInput.interactable = active;
        LoginButton.interactable = active;

        DataInput.interactable = active && PlayfabId != null;
        SaveButton.interactable = active && PlayfabId != null;
    }
}
