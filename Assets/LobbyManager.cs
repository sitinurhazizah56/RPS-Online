using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField newRoomInputField;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] GameObject roomPanel;
    [SerializeField] Button startGameButton;

    // room
    [SerializeField] GameObject roomListObject;
    [SerializeField] RoomItem roomItemPrefab;
    List<RoomItem> roomItemList = new List<RoomItem>();

    // player
    [SerializeField] GameObject PlayerListObject;
    [SerializeField] PlayerItem playerItemPrefab;
    List<PlayerItem> playerItemList = new List<PlayerItem>();
    Dictionary<string, RoomInfo> roomInfoCache = new Dictionary<string, RoomInfo>();

    private void Start()
    {
        roomPanel.SetActive(false);
        Application.runInBackground = true;

    }
    public void ClickCreateRoom()
    {
        if (newRoomInputField.text.Length < 3)
        {
            feedbackText.text = "Room Name min 3 characters";
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(newRoomInputField.text, roomOptions);

        // untuk join random
        // PhotonNetwork.JoinOrCreateRoom(newRoomInputField.text);
        // PhotonNetwork.JoinRandomRoom();
    }
    public void ClickStartGame(string levelName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(levelName);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public override void OnCreatedRoom()
    {
        feedbackText.text = "Created room: " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnJoinedRoom()
    {
        feedbackText.text = "Joined room: " + PhotonNetwork.CurrentRoom.Name;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);

        // update player list
        UpdatePlayerList();

        // atur start game button
        SetStartGameButton();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // update player list
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // update player list
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        // atur start game button
        SetStartGameButton();
    }

    private void SetStartGameButton()
    {
        // tampilkan hanya di master client
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        // bisa diklik / interactable hanya ketika jumlah player >= 2 
        startGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 1;
    }

    private void UpdatePlayerList()
    {
        // destroy dulu semua player item yang sudah ada
        foreach (var item in this.playerItemList)
        {
            Destroy(item.gameObject);
        }

        this.playerItemList.Clear();

        // bikin ulang player list
        // foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) (alternative)
        foreach (var (id, player) in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, PlayerListObject.transform);
            newPlayerItem.Set(player);
            this.playerItemList.Add(newPlayerItem);

            if (player == PhotonNetwork.LocalPlayer)
                newPlayerItem.transform.SetAsFirstSibling();
        }

        // start game hanya bisa diklik ketika jumlah pemain tertentu
        // jadi  atur juga di sini
        SetStartGameButton();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode + ", " + message);
        feedbackText.text = returnCode.ToString() + ", " + message;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var roomInfo in roomList)
        {
            roomInfoCache[roomInfo.Name] = roomInfo;
        }

        foreach (var item in this.roomItemList)
        {
            Destroy(item.gameObject);
        }

        this.roomItemList.Clear();

        var roomInfoList = new List<RoomInfo>(roomInfoCache.Count);

        // Sort yang open di add duluan
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen)
                roomInfoList.Add(roomInfo);
        }

        // kemudian yang close
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen == false)
                roomInfoList.Add(roomInfo);
        }


        foreach (var roomInfo in roomInfoList)
        {
            if (roomInfo.IsVisible == false || roomInfo.MaxPlayers == 0)
                continue;

            RoomItem newRoomItem = Instantiate(roomItemPrefab, roomListObject.transform);
            newRoomItem.Set(this, roomInfo);
            this.roomItemList.Add(newRoomItem);
        }
    }
}
