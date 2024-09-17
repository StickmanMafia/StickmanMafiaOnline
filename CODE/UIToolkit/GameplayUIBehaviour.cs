#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static Role;
using Debug = UnityEngine.Debug;

public sealed class GameplayUIBehaviour : UIController
{
   // public Education education;
   // public Education education2;

    public InRoomCallbackListener CallbackRoom;
    public BetGame BetManager;
    [SerializeField]
    public DataBaseManager database;
     [SerializeField]

        private AllPerksStorage _perksStorage;

    [SerializeField]
    private GameObject? _gameplayControllerObject;

    // Core UI elements
    private VisualElement? _root;
    private VisualElement? _upperHeaderPanel;
    private VisualElement? _upperWarningPanel;
    private VisualElement? _playersAwaitingListPanel;
    private VisualElement? _rolePanelTile;
    private VisualElement? _interactivePanel;
    private VisualElement? _centerPanel;
    private VisualElement? _gameOverPanel;
    private ProgressBar? _progressBar;

    // Secondary yet important UI elements
    private VisualElement? _badPlayers;
    private VisualElement? _goodVsBadPlayers;
    private VisualElement? _goodPlayers;
    private VisualElement? _bigTextPanel;
    private TextField? _chatMessageTextField;
    private Label? _goodVsBadCount;
    private Label? _bigTextLabel;
    private Label? _gameOverLabel;
    private Label? _progressBarLabel;
    private Label? _warningMessageLabel;

    // Cards
    private VisualElement? _civilianCard;
    private VisualElement? _policeOfficerCard;
    private VisualElement? _doctorCard;
    private VisualElement? _witnessCard;
    private VisualElement? _mafiaCard;
    private VisualElement? _mafiaBossCard;
    private VisualElement? _maniacCard;

    // ScrollViews
    private ScrollView? _awaitingPlayersScrollViewer;
    private ScrollView? _chatScrollViewer;
    private ScrollView? _interactivePanelScrollViewer;
    
    private GameplayController? _gameplayController;
    private StopwatchController? _stopwatchController;
    private DayTimeController? _dayTimeController;
    private PerkController? _perkController;
    private FXSoundController? _soundController;
    private MusicController? _musicController;
    
    private Dictionary<VisualElement, string>? _lastUiState;
    private Dictionary<Role, VisualElement?>? _rolesWithCards;
    private string[]? _discussionButtonNames;

    private Stopwatch? _gameOverStopwatch;

    private readonly Dictionary<Type, string> _perkLocalizedStrings = new()
    {
        { typeof(ActiveRole) , "active_role_perk" },
        { typeof(Wiretapping) , "wiretapping_perk" },
        { typeof(LieDetector) , "lie_detector_perk" },
        { typeof(Revenge) , "revenge_perk" },
        { typeof(Radio) , "radio_perk" },
        { typeof(DoubleVoice), "double_voice_perk" },
        { typeof(Disguise), "disguise_perk" },
        { typeof(Helicopter), "helicopter_perk" },
        { typeof(MineDetector), "mine_detector_perk" },
        { typeof(Mine), "mine_perk" },
    };
    
    public bool PerksAcknowledged { get; set; }
    
    // Setter - only properties
    
    // Numerics
    public float ProgressBarCurrentValue { set => _progressBar.value = value; }
    
    public float ProgressBarMaxValue { set => _progressBar.highValue = value;  }
    
    // Strings
    public string ProgressBarMessage { set => _progressBarLabel.text = value; }
    
    // Booleans
    public bool ProgressBarVisible
    {
        set
        {
            SetVisibility(value, _progressBar);
            SetVisibility(value, _progressBarLabel);
        }
    }
    
    public bool BigTextVisible { set => SetVisibility(value, _bigTextPanel); } 
    
    public bool GoodVsBadPanelVisible { set => SetVisibility(value, _goodVsBadPlayers); } 
    
    public bool RolePanelVisible { set => SetVisibility(value, _rolePanelTile); }
    
    // Enums
    public Role RolePanelCard { set => EnableElement(_rolesWithCards[value]); }

    // Queries
    private IEnumerable<PlayerData> KillQuery => Me.Controller.Role == Mafia
        ? Players.Where(n => n is { IsMine: false, Controller: { Alive: true, Role: not Mafia and not MafiaBoss } })
        : Players.Where(n => n.Controller.Alive && !n.IsMine);
    
    private IEnumerable<PlayerData> BaseQuery => 
        Players.Where(n => n is { IsMine: false, Controller: { Alive: true } });

    private IEnumerable<PlayerData> OthersQuery =>
        Players.Where(n => !n.IsMine);
    
    private IEnumerable<PlayerData> VotingQuery =>
        Players.Where(n => !n.IsMine 
                           && n.Controller.Alive 
                           && !n.Controller.OutfitVisible 
                           && !Me.Controller.VotingActorNumbers.Contains(n.ActorNumber));
    
    private IEnumerable<PlayerData> LieDetectorQuery => 
        Players.Where(n => n is {IsMine: false, Controller: { OutfitVisible: false, Alive: true } });
    
    private async void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _perksStorage = await SavedPerksAsync();
        _upperHeaderPanel = _root.Q<VisualElement>("upper-header-panel");
        _upperWarningPanel = _root.Q<VisualElement>("upper-warning-panel");
        _playersAwaitingListPanel = _root.Q<VisualElement>("players-awaiting-tile");
        _rolePanelTile = _root.Q<VisualElement>("role-panel-tile");
        _interactivePanel = _root.Q<VisualElement>("lower-panel");
        _centerPanel = _root.Q<VisualElement>("center-panel");
        _gameOverPanel = _root.Q<VisualElement>("game-over-panel");
        _progressBar = _root.Q<ProgressBar>("progress-bar");

        _civilianCard = _root.Q<VisualElement>("civilian-card");
        _witnessCard = _root.Q<VisualElement>("witness-card");
        _policeOfficerCard = _root.Q<VisualElement>("police-officer-card");
        _doctorCard = _root.Q<VisualElement>("doctor-card");
        _mafiaCard = _root.Q<VisualElement>("mafia-card");
        _mafiaBossCard = _root.Q<VisualElement>("mafia-boss-card");
        _maniacCard = _root.Q<VisualElement>("maniac-card");

        _chatMessageTextField = _root.Q<TextField>("message-input");
        _goodVsBadPlayers = _upperHeaderPanel.Q<VisualElement>("left-buttons");
        _goodPlayers = _upperHeaderPanel.Q<VisualElement>("good-players");
        _goodVsBadCount = _upperHeaderPanel.Q<Label>("good-vs-bad-players-count");
        _badPlayers = _upperHeaderPanel.Q<VisualElement>("bad-players");
        _bigTextPanel = _root.Q<VisualElement>("big-message-panel");
        _bigTextLabel = _root.Q<Label>("big-message-label");
        _gameOverLabel = _root.Q<Label>("game-over-label");
        _progressBarLabel = _interactivePanel.Q<Label>("progress-bar-label");
        _warningMessageLabel = _root.Q<Label>("warning-messaage");
    
        _awaitingPlayersScrollViewer = _root.Q<ScrollView>("player-in-room-awaiting-scroll-view");
        _chatScrollViewer = _root.Q<ScrollView>("chat-message-scrollview");
        _interactivePanelScrollViewer = _interactivePanel.Q<ScrollView>("interactive-panel-scrollview");
        
        _gameplayController = _gameplayControllerObject.GetComponent<GameplayController>();
        _stopwatchController = GameObject.Find("StopwatchController").GetComponent<StopwatchController>();
        _dayTimeController = GameObject.Find("DayTimeController").GetComponent<DayTimeController>();
        _perkController = GameObject.Find("PerkController").GetComponent<PerkController>();
        _soundController = GameObject.Find("FXSoundController").GetComponent<FXSoundController>();
        _musicController = GameObject.Find("MusicController").GetComponent<MusicController>();

        _root.Q<Button>("quit-game-button").clicked += () => LeaveFromRoom();
        _root.Q<Button>("people-joined-button").clicked += () => TogglePanel(_playersAwaitingListPanel);
//        _root.Q<Button>("settings-button").clicked += () => { };
        _interactivePanelScrollViewer.Q<Button>("play-laugh-animation-button").clicked += () => PlayAnimation("Laugh");
        _interactivePanelScrollViewer.Q<Button>("play-surprised-animation-button").clicked += () => PlayAnimation("Surprised");
        _interactivePanelScrollViewer.Q<Button>("play-applause-animation-button").clicked += () => PlayAnimation("Applause");
        _interactivePanelScrollViewer.Q<Button>("play-anger-animation-button").clicked += () => PlayAnimation("Anger");
        _interactivePanelScrollViewer.Q<Button>("play-tease-animation-button").clicked += () => PlayAnimation("Tease");
        _interactivePanelScrollViewer.Q<Button>("play-weep-animation-button").clicked += () => PlayAnimation("Weep");
        _interactivePanelScrollViewer.Q<Button>("perks-button").clicked += ShowAvailablePerks;
        _interactivePanelScrollViewer.Q<Button>("vote-button").clicked += ShowVotingButtons;
        _interactivePanelScrollViewer.Q<Button>("perks-button").clicked += ShowAvailablePerks;
        _interactivePanelScrollViewer.Q<Button>("exit-button").clicked += RestoreLastUIState;

        _root.Q<Button>("send-button").clicked += SendMyChatMessage;

        PerksAcknowledged = false;
        
        _gameOverStopwatch = new Stopwatch();
        
        _rolesWithCards = new Dictionary<Role, VisualElement?>
        {
            { Civilian, _civilianCard },
            { PoliceOfficer, _policeOfficerCard },
            { Doctor, _doctorCard },
            { Witness, _witnessCard },
            { Mafia, _mafiaCard },
            { MafiaBoss, _mafiaBossCard },
            { Maniac, _maniacCard },
        };

        _discussionButtonNames = new[]
        {
            "play-laugh-animation-button",
            "play-surprised-animation-button",
            "play-applause-animation-button",
            "play-anger-animation-button",
            "play-tease-animation-button",
            "play-weep-animation-button",
            "perks-button",
            "vote-button",
            "exit-button"
        };

        _lastUiState = new();


        for (var i = 6; i < _discussionButtonNames.Length; i++){

            _interactivePanelScrollViewer.Q(_discussionButtonNames[i]).style.display = DisplayStyle.None;
        
        }
        ShowWarning(GetLocalizedString("unused_perks"));
        StartPerkPulsing();
    }
    private async Task<AllPerksStorage> SavedPerksAsync(){
        User user =await database.ReadDataURLAsync(PlayerPrefs.GetString("UserID"));
        return user.perks;
    }
   
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return) && !_chatMessageTextField.text.Equals(string.Empty))
            SendMyChatMessage();

        if (_gameOverStopwatch.IsRunning && _gameOverStopwatch.Elapsed.TotalSeconds > 5)
        {
            _soundController.Stop();
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }
    }
    private void LeaveFromRoom(){
        Debug.Log(Me.PhotonPlayer);
        CallbackRoom.LeaveRoom(Me.PhotonPlayer);
        PhotonNetwork.LeaveRoom();
    }
    
    /*
     * The following section below contains methods that
     * activate UI animations made with timelines.
     */
    public void StartPerkPulsing() => StartPulsing("PulsePerks");

    public void StopPerkPulsing() => StopPulsing("PulsePerks");

    public void StartActionPulsing() {

     //   education2.StartEducationInGame2();
        StartPulsing("PulseAction");
    } 

    public void StopActionPulsing() {
     //   education2.NextStage();
        StopPulsing("PulseAction");
    } 

    public void StartVotePulsing() {
            //education.StartEducationInGame();
            StartPulsing("PulseVoting");
    } 
    
    public void StopVotingPulsing() => StopPulsing("PulseVoting");
    
    public void ShowWarning(string text)
    {
        _warningMessageLabel.text = text;
        ResetShowWarningDirector();
        var showWarningDirector = GameObject.Find("ShowWarning").GetComponent<PlayableDirector>();
        showWarningDirector.Play();
        _soundController.PlayNotificationWarningSound();
    }
    
    public void HideWarning()
    {
        ResetHideWarningDirector();
        var hideWarningDirector = GameObject.Find("HideWarning").GetComponent<PlayableDirector>();
        hideWarningDirector.Play();
    }

    public void EnableDiscussionButtons()
    {
        foreach (var elementName in _discussionButtonNames.Where(n => !n.Equals("exit-button")))
            EnableElement(_interactivePanelScrollViewer.Q(elementName));
    }

    public void ShowNewChatMessage(string assetName, string header, string message)
    {
        var messageElement = InstantiateVisualElement<VisualElement>(assetName);
        
        messageElement.Q<Label>("nickname-header").text = header;
        messageElement.Q<Label>("message").text = message;
        
        _chatScrollViewer.Add(messageElement);
        DelayedScroll(_chatScrollViewer, messageElement);
    }
    
    public void ShowActionButtons()
    {
        FetchPlayers();
        
        DisableElement(_rolePanelTile);
        DisableAllInteractionButtons();
        
        switch (Me.Controller.Role)
        {
            case Mafia or Maniac:
            {
                GetActionButton(GetLocalizedString("select_kill")).clicked += () => 
                    ShowPlayerSelectionButtons("kill-button", nameof(OnKill), KillQuery);
                
                AddSkipButton();
                break;
            }
            case PoliceOfficer:
            {
                GetActionButton(GetLocalizedString("select_check")).clicked += () => 
                    ShowPlayerSelectionButtons("investigate-button", nameof(OnInvestigate), BaseQuery);
                
                AddSkipButton();
                break;
            }
            case Doctor:
            {
                GetActionButton(GetLocalizedString("select_rescue")).clicked += () => 
                    ShowPlayerSelectionButtons("heal-button", nameof(OnHeal), OthersQuery);
                
                AddSkipButton();
                break;
            }
            case MafiaBoss:
            {
                GetActionButton(GetLocalizedString("select_paralyze")).clicked += () => 
                    ShowPlayerSelectionButtons("paralyze-button", nameof(OnParalyze), KillQuery);
                
                AddSkipButton();
                break;
            }
            case Witness:
            {
                GetActionButton(GetLocalizedString("select_witness")).clicked += () => 
                    ShowPlayerSelectionButtons("visit-button", nameof(OnVisit), BaseQuery);
                
                AddSkipButton();
                break;
            }
            default:
            {
                OnAwaiting();
                break;
            }
        }
        
        ShowWarning(GetLocalizedString("player_selection_available"));
        StartActionPulsing();
        _stopwatchController.StartActionStopwatch();
    }

    public void ShowGameOver()
    {
        FetchPlayers();
        
        DisableElement(_upperHeaderPanel);
        DisableElement(_upperWarningPanel);
        DisableElement(_centerPanel);
        DisableElement(_interactivePanel);
        EnableElement(_gameOverPanel);
        
        _musicController.Stop();
        
        if (Players.Any(n => n.Controller.Role == Mafia && n.Controller.Alive))
        {
             if(Players.First(n => n.IsMine).Controller.Role == Mafia){
                 BetManager.WinGame(Players.Count());
            }
            else{
                BetManager.LoseGame();
            }
            _soundController.PlaySadMusic();
            _gameOverLabel.text = GetLocalizedString("mafia_win");
        }
        else
        {
            _soundController.PlayHappyMusic();
            if(Players.First(n => n.IsMine).Controller.Role != Mafia){
                 BetManager.WinGame(Players.Count());
            }
             else{
                BetManager.LoseGame();

            }
           
            _gameOverLabel.text = GetLocalizedString("civilians_win");
        }
        
        _gameOverStopwatch.Start();
    }
    
    public void UpdateAwaitPlayerList()
    {
        FetchPlayers();
        
        foreach (var child in _awaitingPlayersScrollViewer.Children().ToList()) 
            _awaitingPlayersScrollViewer.Remove(child);

        foreach (var player in Players.ToList()) 
            AddAwaitingPlayerTile(player.Nickname);
    }

    public void AddDeadMessage()
    {
        DisableAllInteractionButtons();
        AddTextToInteractionBar("actions-dead-tile", GetLocalizedString("you_are_dead"));
    }

    public void AddParalyzedMessage()
    {
        DisableAllInteractionButtons();
        AddTextToInteractionBar("actions-paralyzed-tile", GetLocalizedString("you_are_paralyzed"));
    }

    public void OnAwaiting() => EnableElement(_interactivePanelScrollViewer.Q<Button>("perks-button"));

    public void UpdateGoodVsBadPanelContents()
    {
        FetchPlayers();
        
        var goodPlayers = Players.Where(n => n.Controller is {Role: not Mafia and not Maniac and not MafiaBoss});
        var badPlayers = Players.Where(n => n.Controller is {Role: Mafia or Maniac or MafiaBoss});
        
        _goodPlayers.Clear();
        _badPlayers.Clear();

        foreach (var player in goodPlayers) 
            AddNewMiniIcon(player.GameObject, _goodPlayers);

        _goodVsBadCount.text = goodPlayers.Count() + ":" + badPlayers.Count();
        
        foreach (var player in badPlayers)
            AddNewMiniIcon(player.GameObject, _badPlayers);
    }

    public void ShowInteractionBarDiscussionButtons()
    {
        DisableAllInteractionButtons();
        EnableDiscussionButtons();
        StartVotePulsing();
        
        var voteButton = _interactivePanelScrollViewer.Q<Button>("vote-button");
        DelayedScroll(_interactivePanelScrollViewer, voteButton);
    }
    [Button("покажи")]
    public void ShowVotingButtons() {
      //  education.NextStage();

        ShowPlayerSelectionButtons("vote-button", nameof(OnVote), VotingQuery);

    } 
    
    public void EnableBigText(string message)
    {
        _bigTextLabel.text = message;
        
        DisableElement(_rolePanelTile);
        EnableElement(_bigTextPanel);
    }
    
    public int GetPlayerMessageAmount(int actorNumber) => 
        _chatScrollViewer.Children().Count(n => n.Q<Label>("nickname-header").text.Equals($"Player {actorNumber}"));
    
      
    public void AddNewPerkMiniIcon(string playerHeader, string perkName)
    {
        var targetElement = _awaitingPlayersScrollViewer?.Children()
            .First(n => n.Q<Label>("player-name-label").text.Equals(playerHeader));
        
        var hasAnyPerkPanels = targetElement.Children().Any(n => n.name.Equals("perk-icon-panel"));
        
        if (!hasAnyPerkPanels)
        {
            var newPanel = GetNewPerkIconPanel();
            targetElement.Add(newPanel);
            AppendPerkMiniIcon(perkName, newPanel);
            
            return;
        }
        
        var lastPerkPanel = targetElement.Children().Last(n => n.name.Equals("perk-icon-panel"));
        var lastPerkPanelChildrenCount = lastPerkPanel.Children().Count();

        if (lastPerkPanelChildrenCount < 3)
        {
            AppendPerkMiniIcon(perkName, lastPerkPanel);
        }
        else
        {
            var newPanel = GetNewPerkIconPanel();
            targetElement.Add(newPanel);
            AppendPerkMiniIcon(perkName, newPanel);
        }
    }
    
    public void RemovePerkMiniIcon(string playerHeader, string perkName)
    {
        var targetElement = _awaitingPlayersScrollViewer.Children()
            .First(n => n.Children().Any(q => q is Label label 
                                              && label.name.Equals("player-name-label") 
                                              && label.text.Equals(playerHeader)));

        var targetPerkPanel = targetElement.Children().First(n => n.Children().Any(q => q.name.Equals(perkName)));
        var targetPerkIcon = targetPerkPanel.Children().First(n => n.name.Equals(perkName));
        targetPerkPanel.Remove(targetPerkIcon);
    }

    private void HandleQuickMessage(QuickMessage quickMessage)
    {
        FetchPlayers();
        DisableAllInteractionButtons();
        
        if (quickMessage == QuickMessage.MafiaIs)
        {
            foreach (var player in Players.Where(n => n.Controller.Alive))
            {
                GetPlayerSelectionButton("quick-chat-message-button", player.Nickname).clicked += () =>
                {
                    foreach (var playerInner in Players)
                    {
                        var args = ChatMessageArgumentsService.GetQuickMessageArguments(player.Nickname, Me.Nickname);
                        playerInner.PhotonView.RpcSecure("DisplayThinkMafiaIsMessage", playerInner.PhotonPlayer, true, args);    
                    }
                    
                    DisableAllInteractionButtons();
                    EnableDiscussionButtons();
                };
            }
        }
        else if (quickMessage == QuickMessage.Suspect)
        {
            foreach (var player in Players.Where(n => n.Controller.Alive))
            {
                GetPlayerSelectionButton("quick-chat-message-button", player.Nickname).clicked += () =>
                {
                    foreach (var playerInner in Players)
                    {
                        var args = ChatMessageArgumentsService.GetQuickMessageArguments(player.Nickname, Me.Nickname);
                        playerInner.PhotonView.RpcSecure("DisplaySuspiciousMessage", playerInner.PhotonPlayer, true, args);    
                    }
                    
                    DisableAllInteractionButtons();
                    EnableDiscussionButtons();
                };
            }
        }
        else if (quickMessage == QuickMessage.NotMe)
        {
            foreach (var playerInner in Players)
            {
                playerInner.PhotonView.RpcSecure("DisplayItWasntMeMessage", playerInner.PhotonPlayer, true, Me.Nickname);    
            }
                    
            EnableDiscussionButtons();
        }
    }
    
    public void DisableAllInteractionButtons()
    {
        foreach (var child in _interactivePanelScrollViewer.Children()) 
            child.style.display = DisplayStyle.None;
    }
    
    // This method overload is for actions
    private void ShowPlayerSelectionButtons(string assetName, string methodName, IEnumerable<PlayerData> query, object[]? parameters = null)
    {
        DisableAllInteractionButtons();
        HideWarning();
        StopActionPulsing();
        StopVotingPulsing();
        StopPerkPulsing();

        foreach (var player in query)
        {
            GetPlayerSelectionButton(assetName, player.Nickname).clicked += () =>
            {
                Me?.PhotonView.RpcSecure("SetActionExecuted", RpcTarget.AllBuffered, true, true);
                CallMethod(methodName, parameters ?? new object[] { player });
                StartCoroutine(DisablePlayerSelectionButtonsDelayed(assetName));
            };
        }
    }

    // This method overload is for perks
    private void ShowPlayerSelectionButtons(Type perkType, IEnumerable<PlayerData> query)
    {
        DisableAllInteractionButtons();
        HideWarning();
        
        foreach (var player in query)
        {
            GetPerkPlayerApplySelectionButton(perkType, player.Nickname).clicked += () =>
            {
                CallMethod(nameof(OnPerkInstantiate), new object[] { perkType, player });
                RestoreLastUIState();
            };
        }
    }
        
    private IEnumerator DisablePlayerSelectionButtonsDelayed(string assetName)
    {
        yield return new WaitForSeconds(1);
        
        if (assetName.Equals("vote-button") 
            && Me.Controller.CanVoteTwice 
            && Me.Controller.VotingActorNumbers.Length == 1)
        {
            ShowVotingButtons();
            yield break;
        }
                
        DisableAllInteractionButtons();
        OnAwaiting();
    }
    
    private void AddAwaitingPlayerTile(string name, Sprite avatar = null)
    {
        var newUiElement = InstantiateVisualElement<VisualElement>("player-icon");
        newUiElement.Q<Label>("player-name-label").text = name;
        _awaitingPlayersScrollViewer.Add(newUiElement);
    }
    
    private Button GetActionButton(string actionText, string buttonElementName = "action-director-button")
    {
        var newUiElement = InstantiateVisualElement<VisualElement>("action-button");
        var button = newUiElement.Q<Button>("action-button");
        button.text = actionText;
        button.name = buttonElementName;
        _interactivePanelScrollViewer.Add(newUiElement);
        return button;
    }
    
    private Button GetPlayerSelectionButton(string assetName, string nickname, string buttonElementName = "player-selection-button")
    {
        var newUiElement = InstantiateVisualElement<VisualElement>(assetName);
        var button = newUiElement.Q<Button>(assetName);
        button.Q<Label>("player-name-label").text = nickname;
        button.name = buttonElementName;
        _interactivePanelScrollViewer.Add(button);
        return button;
    }
    
    private Button GetPerkSelectionButton(Type perkType)
    {
        var newUiElement = InstantiateVisualElement<VisualElement>("perk-button");
        var button = newUiElement.Q<Button>("perk-button");
        var texture2D = Resources.Load<Texture2D>("Perks/" + perkType.Name);
        button.Q<Label>("name").text =""; //GetLocalizedString(_perkLocalizedStrings[perkType]
        button.Q<VisualElement>("sprite").style.backgroundImage = new StyleBackground(texture2D);
        _interactivePanelScrollViewer.Add(newUiElement);
        return button;  
    }
    
    private Button GetPerkPlayerApplySelectionButton(Type perkType, string nickname)
    {
        var newUiElement = InstantiateVisualElement<VisualElement>("player-apply-perk-button");
        var button = newUiElement.Q<Button>("player-apply-perk-button");
        var texture2D = Resources.Load<Texture2D>("Perks/" + perkType.Name);
        button.Q<Label>("name").text ="" ;//nickname
        button.Q<VisualElement>("sprite").style.backgroundImage = new StyleBackground(texture2D);
        _interactivePanelScrollViewer.Add(newUiElement);
        return button;  
    }
    
    private void AddSkipButton(string additionalMessage = null)
    {
        GetActionButton(additionalMessage == null 
            ? $"{GetLocalizedString("skip")}" 
            : $"{GetLocalizedString("skip")} ({additionalMessage})", "action-button").clicked += () =>
        {
            _stopwatchController.StopActionStopwatch();
            DisableAllInteractionButtons();
            OnAwaiting();
            FetchPlayers();
            Master.PhotonView.RpcSecure("ActionQueueForward", Master.PhotonPlayer, true);
        };
        
        HideWarning();
        StopActionPulsing();
    }
    
    private void SendMyChatMessage()
    {
        if (_chatMessageTextField.text == string.Empty)
        {
            Me.Controller.DisplayGameHostMessage("message_field_empty");
            return;
        }

        if (!Me.Controller.Intact)
        {
            Me.Controller.DisplayGameHostMessage("dead_cant_talk");
            return;
        }

        if (_dayTimeController.CurrentDayTime == TimeOfDay.Night)
        {
            Me.Controller.DisplayGameHostMessage(Me.Controller.Role == Mafia
                ? "night_message_prohibited_mafia"
                : "night_message_prohibited_no_mafia");
            
            return;
        }

        var arguments = ChatMessageArgumentsService.GetPlayerMessageArguments(Me.Nickname, _chatMessageTextField.text);
        Me.PhotonView.RpcSecure("DisplayMessage", RpcTarget.All, true, arguments);

        _chatMessageTextField.value = string.Empty;
    }
    
    private static void TogglePanel(VisualElement? panel) => panel.visible = !panel.visible;
    public void PlayAnimation(string animationName)
    {
        if (!Me.Controller.Intact) return;
        Me.PhotonView.RpcSecure("PlayAnimation", RpcTarget.AllBuffered, true, animationName);
    }
    
    private static IEnumerator ScrollLater(ScrollView? list, VisualElement item)
    {
        yield return new WaitForSeconds(0.1f);
        list?.ScrollTo(item);
    }

    private void DelayedScroll(ScrollView? list, VisualElement item) => StartCoroutine(ScrollLater(list, item));
    
    private void AddNewMiniIcon(GameObject player, VisualElement? parent)
    {
        var playerController = player.GetComponent<PlayerController>();
        var texture2D = Resources.Load<Texture2D>(playerController.Role.ToString());
        var newUiElement = InstantiateVisualElement<VisualElement>("good-player-icon");
        var darkenBackgroundElement = newUiElement.Q<VisualElement>("darken-background");
        
        if (playerController.Alive)
            DisableElement(darkenBackgroundElement);
        else 
            EnableElement(darkenBackgroundElement);
        
        newUiElement.Q<VisualElement>("icon-image").style.backgroundImage = new StyleBackground(texture2D);
        parent.Add(newUiElement);
    }

    private static void AppendPerkMiniIcon(string perkName, VisualElement perkPanel)
    {
        var newIcon = InstantiateVisualElement<VisualElement>($"PerksMini/{perkName}");
        perkPanel.Add(newIcon);
    }

    private void AddExitButton() => EnableElement(_interactivePanelScrollViewer.Q<Button>("exit-button"));

    public void OnKill(PlayerData receiver) => _gameplayController.Kill(Me, receiver);

    public void OnInvestigate(PlayerData receiver) => _gameplayController.Investigate(receiver);

    public void OnParalyze(PlayerData receiver)
    {
        _gameplayController.Paralyze(receiver);
        Master.PhotonView.RpcSecure("ActionQueueForward", Master.PhotonPlayer, true);
    }
    
    public void OnHeal(PlayerData receiver)
    {
        _gameplayController.Heal(receiver);
        Master.PhotonView.RpcSecure("ActionQueueForward", Master.PhotonPlayer, true);
    }
    
    public void OnVisit(PlayerData receiver)
    {
        _gameplayController.Witness(receiver);
        Master.PhotonView.RpcSecure("ActionQueueForward", Master.PhotonPlayer, true);
    }
    
    public void OnVote(PlayerData receiver)
    {
        FreezeVotingButtons();
        _gameplayController.Vote(Me, receiver);
    }

    public void OnPerkInstantiate(Type perkType, PlayerData targetPlayer) => 
        _perkController.Add(perkType, Me, targetPlayer, _perkLocalizedStrings);

    private void CallMethod(string method, object[] parameters)
    {
        var type = typeof(GameplayUIBehaviour);
        var methodInfo = type.GetMethod(method);
        
        if (methodInfo != null) 
            methodInfo.Invoke(this, parameters);
        else 
            Debug.Log("Error when invoking method on LocationOne!");
    }
    
    private void GrabLastUIState()
    {
        _lastUiState?.Clear();

        foreach (var child in _interactivePanelScrollViewer.Children())
            _lastUiState.Add(child, child.style.display.value.ToString());
    }
    
    private void RestoreLastUIState()
    {
        DisableAllInteractionButtons();

        foreach (var (element, display) in _lastUiState)
        {
            Debug.Log($"Restoring {element.name}, display = {display}");
            Enum.TryParse<DisplayStyle>(display, false, out var enumStyle);
            _interactivePanelScrollViewer.Children().First(n => n.name == element.name).style.display = enumStyle;
        }

        _interactivePanelScrollViewer.Q<Button>("exit-button").style.display = DisplayStyle.None;
    }

    private static T InstantiateVisualElement<T>(string assetName) where T : VisualElement => 
        Resources.Load<VisualTreeAsset>(assetName).Instantiate().Q<T>();
    
    private void AddTextToInteractionBar(string assetName, string text)
    {
        var tile = InstantiateVisualElement<VisualElement>(assetName);
        tile.Q<Label>().text = text;
        _interactivePanelScrollViewer.Add(tile);
    }
     
    private static void StartPulsing(string directorName)
    {
        
        var director = GameObject.Find(directorName).GetComponent<PlayableDirector>();
        director.Play();
    }
    
    private static void StopPulsing(string directorName)
    {
        var director = GameObject.Find(directorName).GetComponent<PlayableDirector>();
        director.Stop();
        
        director.time = 0;
        director.Stop();
        director.Evaluate(); 
    }

    private static void ResetShowWarningDirector()
    {
        var showWarningDirector = GameObject.Find("ShowWarning").GetComponent<PlayableDirector>();
        
        showWarningDirector.time = 0;
        showWarningDirector.Stop();
        showWarningDirector.Evaluate();    
    }
    
    private static void ResetHideWarningDirector()
    {
        var hideWarningDirector = GameObject.Find("HideWarning").GetComponent<PlayableDirector>();
        
        hideWarningDirector.time = 0;
        hideWarningDirector.Stop();
        hideWarningDirector.Evaluate();
    }
    
    private IEnumerator ShowPerksDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        
        DisableAllInteractionButtons();
        AddExitButton();

        if (!_gameplayController.GameRunning)
            ShowPreGamePerks();
        else if (_dayTimeController.CurrentDayTime == TimeOfDay.Night || _stopwatchController.PreGameProceduresRunning)
            ShowNightPerks();
        else if (_dayTimeController.CurrentDayTime == TimeOfDay.Day) 
            ShowDayPerks();
    }
    
    private void ShowPreGamePerks()
    {
        if (_perksStorage.ActiveRoleCount > 0)
        {

            GetPerkSelectionButton(typeof(ActiveRole)).clicked += () =>
            {
                
                OnPerkInstantiate(typeof(ActiveRole), Me);
                _perksStorage.ActiveRoleCount--;
                User user = new User(PlayerPrefs.GetString("UserID"), "", JsonUtility.FromJson<PlayerCustomization>(PlayerPrefs.GetString("Customization")), PlayerPrefs.GetInt("money"), _perksStorage, PlayerPrefs.GetInt("gems"));
                database.SaveDataURL(user);     
               RestoreLastUIState();
                
            };
        }
    }
    
    private void ShowNightPerks()
    {
        AddPerkSelectionButton(typeof(Wiretapping), _perksStorage.WiretappingCount);
        AddPerkSelectionButton(typeof(Radio), _perksStorage.RadioCount);
        AddPerkSelectionButton(typeof(Disguise), _perksStorage.DisguiseCount);
    }

    private void ShowDayPerks()
    {
        AddPerkSelectionButton(typeof(Revenge), _perksStorage.RevengeCount);
        AddPerkSelectionButton(typeof(DoubleVoice), _perksStorage.DoubleVoiceCount);
        AddPerkSelectionButton(typeof(LieDetector), _perksStorage.LieDetectorCount);
        AddPerkSelectionButton(typeof(Helicopter), _perksStorage.HelicopterCount);
        AddPerkSelectionButton(typeof(MineDetector), _perksStorage.MineDetectorCount);
        AddPerkSelectionButton(typeof(Mine), _perksStorage.MineCount);
    }
    private void AddPerkSelectionButton(Type perkType, int count)
    {
        if (count > 0)
        {
            GetPerkSelectionButton(perkType).clicked += () =>
            {
                if (perkType == typeof(Wiretapping) || perkType == typeof(LieDetector))
                {
                    ShowPlayerSelectionButtons(perkType, LieDetectorQuery);
                }
                else
                {
                    OnPerkInstantiate(perkType, Me);
                }
                switch (perkType.Name)
            {
                case nameof(ActiveRole):
                    _perksStorage.ActiveRoleCount--;
                    break;
                case nameof(Wiretapping):
                    _perksStorage.WiretappingCount--;
                    break;
                case nameof(LieDetector):
                    _perksStorage.LieDetectorCount--;
                    break;
                case nameof(Revenge):
                    _perksStorage.RevengeCount--;
                    break;
                case nameof(Radio):
                    _perksStorage.RadioCount--;
                    break;
                case nameof(DoubleVoice):
                    _perksStorage.DoubleVoiceCount--;
                    break;
                case nameof(Disguise):
                    _perksStorage.DisguiseCount--;
                    break;
                case nameof(Helicopter):
                    _perksStorage.HelicopterCount--;
                    break;
                case nameof(MineDetector):
                    _perksStorage.MineDetectorCount--;
                    break;
                case nameof(Mine):
                    _perksStorage.MineCount--;
                    break;
            }
            User user = new User(PlayerPrefs.GetString("UserID"), "", JsonUtility.FromJson<PlayerCustomization>(PlayerPrefs.GetString("Customization")), PlayerPrefs.GetInt("money"), _perksStorage,PlayerPrefs.GetInt("gems"));
            database.SaveDataURL(user);                
                 RestoreLastUIState();
            };
        }
    }
    private void ShowAvailablePerks()
    {
        if (!PerksAcknowledged)
        {
            HideWarning();
            StopPerkPulsing();
            PerksAcknowledged = true;
        }
        
        GrabLastUIState();
        FetchPlayers();
        StartCoroutine(ShowPerksDelayed());
    }
    
    private static VisualElement GetNewPerkIconPanel() => new()
    {
        name = "perk-icon-panel",
        style =
        {
            flexDirection = FlexDirection.Row,
            flexGrow = 1,
            maxWidth = 258,
            alignSelf = Align.Center
        }
    };
    
    /*
     * Since a voting button delayed coroutine was introduced
     * in order sync VotingActorNumbers via RPC
     * (see "DisablePlayerSelectionButtonsDelayed" method),
     * but Voting players selection buttons might STILL be
     * available to click, it's necessary to freeze them after voting.
     * If CanVoteTwice perk is enabled, a new set of buttons will be added
     * to the interaction bar, which will not be inoperative.
     * 
     */
    private void FreezeVotingButtons()
    {
        foreach (var playerButton in _interactivePanelScrollViewer.Children().Where(n => n.name.Equals("player-selection-button"))) 
            playerButton.SetEnabled(false);
    }
}
[Serializable]
public struct AllPerksStorage{
    public int ActiveRoleCount;
    public int WiretappingCount;
    public int LieDetectorCount;
    public int RevengeCount;
    public int RadioCount;
    public int DoubleVoiceCount;
    public int DisguiseCount;
    public int HelicopterCount;
    public int MineDetectorCount;
    public int MineCount;
    public AllPerksStorage(int initialCount = 0)
    {
        ActiveRoleCount = initialCount;
        WiretappingCount = initialCount;
        LieDetectorCount = initialCount;
        RevengeCount = initialCount;
        RadioCount = initialCount;
        DoubleVoiceCount = initialCount;
        DisguiseCount = initialCount;
        HelicopterCount = initialCount;
        MineDetectorCount = initialCount;
        MineCount = initialCount;
    }

    
}