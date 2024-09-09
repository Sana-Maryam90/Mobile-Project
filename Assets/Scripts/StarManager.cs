using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;


public class StarManager : MonoBehaviour
{
    public Button[] levelButtons; 
    public Sprite filledStarSprite;
    public static StarManager instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        GetStars();
    }

    public void SaveStarCount(int starsInput)
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex - 1; //levels start form index 2 in build settings
        Debug.Log("Stars Input" + starsInput);
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("levelData"))
                {
                    string levelDataJson = result.Data["levelData"].Value;
                    LevelArrayWrapper levelData = JsonUtility.FromJson<LevelArrayWrapper>(levelDataJson);
                    var levels = levelData.Levels;

                    //since levels start form index 2 in build settings but index 0 inplayfab player data
                    if (starsInput > levels[currentLevelIndex - 1].stars)
                    {
                        //since index starts from 0 in data, level1 is at index 0, level2 is at index 1..
                        levels[currentLevelIndex - 1].stars = starsInput;

                        // Save updated level data back to PlayFab
                        string updatedLevelDataJson = JsonUtility.ToJson(levelData);
                        var data = new Dictionary<string, string>
                        {
                            { "levelData", updatedLevelDataJson }
                        };

                        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
                        {
                            Data = data
                        },
                        result => Debug.Log("Next level unlocked and data updated in PlayFab."),
                        OnError);
                    }
                }
            },
            OnError
        );
    }


    void GetStars()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnStarsDataReceived, OnError);
    }


    void OnStarsDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("levelData"))
        {
            // Deserialize the JSON data to LevelArrayWrapper
            string levelDataJson = result.Data["levelData"].Value;
            LevelArrayWrapper levelData = JsonUtility.FromJson<LevelArrayWrapper>(levelDataJson);
            var levels = levelData.Levels;
            
            for (int i = 0; i < levelButtons.Length; i++)
            {
                Button levelButton = levelButtons[i];
                int starCount = levels[i].stars;
                Debug.Log("Level  stars: " + starCount);

                Transform starPanel = levelButton.transform.Find("StarPanel");

                Image[] stars = starPanel.GetComponentsInChildren<Image>();
                Debug.Log(stars.Length);


                for(int j = 0; j < starCount; j++)
                {
                    stars[j].sprite = filledStarSprite;
                }
            }
        }
        else
        {
            Debug.LogWarning("No level data found in PlayFab.");
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    
}
