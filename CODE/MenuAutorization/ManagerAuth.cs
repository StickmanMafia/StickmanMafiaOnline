using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerAuth : MonoBehaviour
{
    public Transform SignIn;
    public Transform SignUp;
    public LoginUIBehaviour loginUIBehaviour;
    public Text textMissing;
    public TMP_InputField UserNameInput;
    public Color[] colorsText;
    public void ShowSignIn(){
        SignIn.DOLocalMoveX(0, 0.8f);
        SignUp.DOLocalMoveX(2488, 0.8f);
    }
    public void ShowSignUp(){
        SignIn.DOLocalMoveX(-2488, 0.8f);
        SignUp.DOLocalMoveX(0, 0.8f);
    }
    public void AnimateMe(Transform me){
        me.DOPunchScale(new Vector3(1, 1, 1), 0.3f)
            .OnComplete(() => me.DOScale(Vector3.one, 0.3f));
    }
    public void AuthEnded(){
        string UserName = UserNameInput.text;
        PlayerPrefs.SetString("UserName", UserName);
        SignIn.DOScale(0,0.5f);
        SignUp.DOScale(0,0.5f).OnComplete(()=>loginUIBehaviour.SignInButtonOnClicked(UserName));
        
    }
     public void AuthEnded(string name){
        string UserName = name;
        PlayerPrefs.SetString("UserName", UserName);
        SignIn.DOScale(0,0.5f);
        SignUp.DOScale(0,0.5f).OnComplete(()=>loginUIBehaviour.SignInButtonOnClicked(UserName));
        
    }
    public void AuthFailed(string error){
        textMissing.text = error;
        textMissing.DOColor(colorsText[0], 0.4f).OnComplete(() => textMissing.DOColor(colorsText[1], 2f));
    }
}
