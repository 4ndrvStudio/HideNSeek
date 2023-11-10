using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace HS4
{
    public class LobbyResult
    {
        public bool IsSuccess;
        public string Message;
        public object Data;
    }

    public class LobbyPlayerData
    {
        public bool IsHost;
        public bool IsReady;
        public string DisplayName;
        public int CharacterType;
    }

    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;
        public Lobby CurrentLobby = default;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private Dictionary<string, PlayerDataObject> CreateInitialPlayerData(LobbyPlayerData user)
        {
            return new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "IsHost", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: user.IsHost.ToString())
                    },
                    {
                        "IsReady", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: user.IsReady.ToString())
                    },
                    {
                        "DisplayName", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: user.DisplayName)
                    },
                    {
                        "CharacterType", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: user.CharacterType.ToString())
                    }
                };
        }

        public async Task<LobbyResult> CreateLobby(string lobbyName, int maxPlayers, LobbyPlayerData playerData, bool isPrivate)
        {
            try
            {
                string uasId = AuthenticationService.Instance.PlayerId;

                RelayHostData relayHostData = await RelayManager.Instance.SetupHost(maxPlayers);

                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions();

                createLobbyOptions.Data = new Dictionary<string, DataObject> {
                { "JoinCodeKey", new DataObject(DataObject.VisibilityOptions.Public, relayHostData.JoinCode) }
            };

                createLobbyOptions.Player = new Player(
                            id: AuthenticationService.Instance.PlayerId,
                            data: CreateInitialPlayerData(playerData)
                );

                CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

                StartCoroutine(LoadGamePlayScene(true));

                StartCoroutine(HeartbeatLobbyCoroutine(CurrentLobby.Id, 15));

                return new LobbyResult
                {
                    IsSuccess = true,
                    Data = CurrentLobby
                }; ;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e.Message);
                return new LobbyResult
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }

        }


        public async Task<LobbyResult> JoinLobby(string lobbyId, LobbyPlayerData playerData)
        {
            try
            {
                JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();

                options.Player = new Player(
                        id: AuthenticationService.Instance.PlayerId,
                        data: CreateInitialPlayerData(playerData)
                );

                CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
                StartCoroutine(HeartbeatLobbyCoroutine(CurrentLobby.Id, 15));

                string joinCodeKey = CurrentLobby.Data["JoinCodeKey"].Value;
                await RelayManager.Instance.JoinRelay(joinCodeKey);

                StartCoroutine(LoadGamePlayScene(false));

                return new LobbyResult
                {
                    IsSuccess = true,
                    Data = CurrentLobby
                };

            }
            catch (LobbyServiceException e)
            {
                return new LobbyResult
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }

        IEnumerator LoadGamePlayScene(bool isHost)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("scene_gameplay", LoadSceneMode.Additive); ;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            
            if(isHost) {
                NetworkManager.Singleton.StartHost();
            }else {
                NetworkManager.Singleton.StartClient();
            }


        }
        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            Transform target = GameController.Instance.GetPointToSpawn();
            response.Position = target.position;
            response.Rotation = target.rotation;
            response.Approved = true;
            response.CreatePlayerObject = true;
        }

        public async Task<LobbyResult> RetrieveLobbyList()
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 100;

                // Filter for open lobbies only
                options.Filters = new List<QueryFilter>()
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                };

                // Order by newest lobbies first
                options.Order = new List<QueryOrder>()
                {
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created)
                };

                QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

                return new LobbyResult
                {
                    IsSuccess = true,
                    Data = lobbies
                };
                //...
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return new LobbyResult
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }

        private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);
            while (true && CurrentLobby != null)
            {
                Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return delay;
            }
        }

        public void LeaveLobby()
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.UnloadSceneAsync("scene_gameplay");
            StopAllCoroutines();
        }

    }

}
