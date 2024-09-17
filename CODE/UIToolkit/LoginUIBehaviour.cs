using System;
using Photon.Pun;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUIBehaviour : UIController
{
    private VisualElement _loginTile;
    private VisualElement _internetCheckTile;
    private VisualElement _internetFailTile;
    public LoadingScreenManager SceneManagerAuth;
    
    public string MyName;

    private void Awake() => Application.targetFrameRate = 60;

    // private void OnEnable()
    // {
    //     var root = GetComponent<UIDocument>().rootVisualElement;
    //
    //     _loginTile = root.Q<VisualElement>("reg_login_tile");
    //     _internetCheckTile = root.Q<VisualElement>("internet_check_tile");
    //     _internetFailTile = root.Q<VisualElement>("internet_fail_tile");
    //     
    //     _emailField = root.Q<TextField>("login_field");
    //     _passwordField = root.Q<TextField>("password_field");
    //
    //     var signInButton = root.Q<Button>("sign_in_button");
    //     signInButton.clicked += SignInButtonOnClicked;
    //     
    //     var refreshConnectionButton = root.Q<Button>("refresh_connection_status_button");
    //     refreshConnectionButton.clicked += RefreshConnectionButtonOnClicked;
    //     
    //     CheckInternet();
    // }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

       
        
       
    }

 

    public void SignInButtonOnClicked(string Name)
    {
        MyName = Name;
        //if (RuntimeVariables.PhotonConnected && !_nicknameField.text.Equals(string.Empty))
       // {
       //     SceneManager.LoadScene("MainMenu");
      //      PhotonNetwork.NickName = _nicknameField.text;
      //  }
      //  else
       //     CheckInternet();
        if (RuntimeVariables.PhotonConnected)
        {
            StartCoroutine(SceneManagerAuth.LoadSceneAsync("MainMenu"));
            PhotonNetwork.NickName = Name;
            
        }
        else{
            
            Invoke("Retry",0.5f);
        }
       
    }
    public void Retry(){
        if (RuntimeVariables.PhotonConnected)
        {
            SceneManager.LoadScene("MainMenu");
            PhotonNetwork.NickName =MyName;
            
        }
        else{
            
            Invoke("Retry",0.5f);
        }
    }

    
}