using System.Collections;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MainMenuBehaviour : UIController
{
    private VisualElement _roomsScrollView;
    private VisualElement _mainMenu;
    private VisualElement _roomsListMenu;
    private VisualElement _createRoomMenu;

    private RoomMonitor _roomMonitor;
    public Transform panel;
    public Canvas OurCanvas;
    public managerMenu menu;
    public BetMoney Bet;

    public void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        

        // Tiles
        _mainMenu = root.Q<VisualElement>("main-menu-buttons");
        _roomsListMenu = root.Q<VisualElement>("rooms-list");
        _createRoomMenu = root.Q<VisualElement>("create-room-menu");
        
        // MainMenuButtons
        var customizationButton = root.Q<Button>("customization");
        var roomsButton = root.Q<Button>("rooms");
        var createRoomMainMenuButton = root.Q<Button>("play_create");
        
        // Rooms menu buttons
        var exitRoomsButton = _roomsListMenu.Q<Button>("exit-button");
        var refreshRoomsButton = _roomsListMenu.Q<Button>("exit-button");
        
        // Create room menu buttons
        var exitCreateRoomMenuButton = _createRoomMenu.Q<Button>("exit-button");
        var createRoomButton = _createRoomMenu.Q<Button>("create-button");
            
        // Room list
        _roomsScrollView = root.Q<ScrollView>("rooms-scroll-view");

        // Main menu click events
        
        
        
        // Room selection click events
        exitRoomsButton.clicked += () => EnableSingleMenu(_mainMenu);
        refreshRoomsButton.clicked += () =>
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinLobby();
            StartCoroutine(UpdateRoomListLater());
        };
        createRoomButton.clicked += CreateRoomButtonOnClicked;

        // Room creation click events
        exitCreateRoomMenuButton.clicked += () => EnableSingleMenu(_mainMenu);
        exitCreateRoomMenuButton.clicked += () => Bet.ready();
        EnableSingleMenu(_mainMenu);
    }
    public void switchLang(int lan){
        PlayerPrefs.SetInt("language",lan);

        TranslatorManager.TranslateAll();
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[lan];
        


         PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinLobby();
    }
    public void ShowAvailableRooms(){
            EnableSingleMenu(_roomsListMenu);
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinLobby();
            FillRooms();
    }
    public void ShowSingleMenu(){
        EnableSingleMenu(_createRoomMenu);
    }
    private void CreateRoomButtonOnClicked()
    {
        var name = _createRoomMenu.Q<TextField>("name-field").text;
        
        if (RuntimeVariables.Rooms.Any(n => n.Name.Equals(name)))
            return;
        
        var participantsQuantityIndex = _createRoomMenu.Q<RadioButtonGroup>("player-number-selection").value;

        var participantsAmount = participantsQuantityIndex switch
        {
            0 => 4,
            1 => 6,
            2 => 8,
            3 => 10,
            4 => 12,
            _ => 4
        };
        
        var availabilityIndex = _createRoomMenu.Q<RadioButtonGroup>("availability-radiobuttons").value;
        var isOpen = availabilityIndex == 0;
        
        CreateRoom(name, participantsAmount, isOpen);
    }

    private void CreateRoom(string name, int maxPlayers, bool isOpen)
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayers,
            IsVisible = true, 
            IsOpen = isOpen
        };

        PhotonNetwork.JoinOrCreateRoom(name, roomOptions, null);
    }

    private void AddRoomTile(string name, string playerQuantity)
    {
        var resource = Resources.Load<VisualTreeAsset>("room-tile");
        var newUiElement = resource.Instantiate();
    
        newUiElement.Q<Label>("room-name-label").text = name;
        newUiElement.Q<Label>("player-quantity-label").text = $"{GetLocalizedString("players")}: " + playerQuantity;
        var joinButton = newUiElement.Q<Button>("join-button"); 
        joinButton.Q<Label>().text = GetLocalizedString("join");
        joinButton.clicked += () =>
        {
            ColorUtility.TryParseHtmlString("#EDFFFF" , out var textColour );
            ColorUtility.TryParseHtmlString("#D6F3FF" , out var buttonColour );
            joinButton.Q<Label>().text = GetLocalizedString("connecting");
            joinButton.Q<Label>().style.color = textColour;
            joinButton.style.backgroundColor = buttonColour;
            DisableElement(joinButton.Q<VisualElement>("join-icon"));
            EnableElement(joinButton.Q<VisualElement>("connecting-icon"));
            
            EnterRoom(name);
        };
        
        _roomsScrollView.Add(newUiElement);
    }

    private void EnterRoom(string name)
    {
        var delayTime = Random.Range(0.0f, 1.0f);
        Debug.Log($"delayTime = {delayTime}");
        StartCoroutine(JoinRoomDelayed(name, delayTime));
    }

    private void DisableAllMenus()
    {
        DisableElement(_roomsListMenu);
        DisableElement(_mainMenu);
        DisableElement(_createRoomMenu);
    }
    
    private void EnableSingleMenu(VisualElement targetMenu)
    {
        if (targetMenu == _mainMenu){
            panel.DOScale(0,0.5f);
            menu.ChangeOrderToTop(OurCanvas);
        }
        else{
            menu.ChangeOrderToBottom(OurCanvas);
        }
        DisableAllMenus();
        EnableElement(targetMenu);
    }

    public void FillRooms()
    {
        for (var i = 0; i < _roomsScrollView.childCount; i++) 
            _roomsScrollView.RemoveAt(i);

        foreach (var room in RuntimeVariables.Rooms.ToArray().Where(n => n.IsOpen && n.PlayerCount > 0))
            AddRoomTile(room.Name, $"{room.PlayerCount}/{room.MaxPlayers}");
    }
    
    private IEnumerator UpdateRoomListLater()
    {
        yield return new WaitForSeconds(1);
        FillRooms();
    }
    
    /*
     * This coroutine is created for players not to interfere
     * with each other when entering a room simultaneously.
     * This significantly decreases a chance of two players
     * spawning in the same position and thus freezing further gameplay.
     */
    private static IEnumerator JoinRoomDelayed(string roomName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        PhotonNetwork.JoinRoom(roomName);
    }
}