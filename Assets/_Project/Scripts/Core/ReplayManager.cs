using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    private async void Start()
    {
        // 유니티 클라우드 서비스 초기화 및 로그인
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    public async void Host(TextMeshProUGUI text) 
    {
        await CreateRelay(text, 2);
    }
    public void InputRoomNumber(TextMeshProUGUI text) 
    {
        string code = text.text;
        code = code.Trim();
        code = Regex.Replace(code, @"[^a-zA-Z0-9]", "");
        JoinRelay(code);
    }
    //호스트 방을 만들고 6자리 코드를 반환하는 함수
    public async Task<string> CreateRelay(TextMeshProUGUI text, int maxPlayers = 2)
    {
        try
        {
            // 릴레이 서버에 방 할당 요청
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

            // 접속에 필요한 6자리 Join Code 가져오기
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            text.text = "room: "+joinCode;

            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData serverData = new RelayServerData(allocation, "dtls");
            utp.SetRelayServerData(serverData);

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"릴레이 생성 실패: {e.Message}");
            return null;
        }
    }

    //클라이언트 6자리 코드를 입력해서 방에 접속하는 함수
    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log($"코드로 접속 시도 중: {joinCode}");

            // 입력받은 코드로 릴레이 서버에서 방 정보 가져오기
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData serverData = new RelayServerData(joinAllocation, "dtls");
            utp.SetRelayServerData(serverData);

            // 클라이언트 시작
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"릴레이 접속 실패: {e.Message}");
        }
    }
}