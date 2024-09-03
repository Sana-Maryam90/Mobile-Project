using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Authentication : MonoBehaviour
{
    [SerializeField] GameObject loginTab, signupTab;
    public InputField username, email, password, emailLogin, passwordLogin;
    string encryptedPassword;
    public Text message, messageLogin;

    public void OpenSignupTab()
    {
        signupTab.SetActive(true);
        loginTab.SetActive(false);
        messageLogin.text = "";
        message.text = "";
    }

    public void OpenLoginTab()
    {
        signupTab.SetActive(false);
        loginTab.SetActive(true);
        messageLogin.text = "";
        message.text = "";
    }

    public void SignUp()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email.text,
            Password = password.text,
            Username = username.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Account Created Successfully!");
        message.text = "Logged in Successfully!";
        StoreCredentials(username.text);
        LoadMainMenu();
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        if(message != null)
        {
            message.text = error.ErrorMessage;
        }
        if(messageLogin != null)
        {
            messageLogin.text = error.ErrorMessage;
        }
    }


    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailLogin.text,
            Password = passwordLogin.text
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Logged in Successfully!");
        messageLogin.text = "Logged in Successfully!";
        GetUserProfile(result.PlayFabId);
    }

    void GetUserProfile(string playFabId)
    {
        var request = new GetAccountInfoRequest
        {
            PlayFabId = playFabId
        };
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnError);
    }

    void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        string username = result.AccountInfo.Username;
        Debug.Log("Username: " + username);
        StoreCredentials(username);
        LoadMainMenu();
    }

    void LoadMainMenu()
    {       
        SceneManager.LoadScene("MainMenu");
    }

    void StoreCredentials(string username)
    {
        PlayerPrefs.SetString("Username", username);
    }
}