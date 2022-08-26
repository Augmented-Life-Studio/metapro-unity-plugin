using GeneralCommand;
using GrpcForUnity.Core;
using GrpcForUnity.DemoScenes.GeneralCommand.ModelDemo;
using UnityEngine;
using UnityEngine.UI;

namespace GrpcForUnity.DemoScenes.GeneralCommand
{
    public class GeneralCmdView : MonoBehaviour
    {
        public Button StartServerBtn;
        public Button StopServerBtn;
        public Button ConnectServerBtn;

        public Dropdown CharacterDropdown;
        public Dropdown WeaponDropdown;
        public Dropdown EquipDropdown;

        public Button CharacterRpcBtn;
        public Button WeaponRpcBtn;
        public Button SkillRpcBtn;

        [Header("Game Image")] public Image CharacterImg;
        public Image WeaponImg;
        public Image EquipmentImg;
        public Sprite[] CharacterSprite;
        public Sprite[] WeaponSprite;
        public Sprite[] EquipmentSprite;

        private IGrpcServer GrpcServer = new GrpcServerGeneralCmd();
        private IGrpcClient GrpcClient = new GrpcClientGeneralCmd();

        private static GeneralCmdView _instance = null;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        public static void SetGameImage(string cmd, string data)
        {
            switch (cmd)
            {
                case nameof(HeroInfo):
                    var heroInfo = JsonUtility.FromJson<HeroInfo>(data);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        _instance.CharacterImg.sprite = _instance.CharacterSprite[(int)heroInfo.Type];
                    });
                    break;

                case nameof(EquipmentInfo):
                    var equipmentInfo = JsonUtility.FromJson<EquipmentInfo>(data);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        _instance.EquipmentImg.sprite = _instance.EquipmentSprite[(int)equipmentInfo.Type];
                    });
                    break;

                case nameof(WeaponInfo):
                    var weaponInfo = JsonUtility.FromJson<WeaponInfo>(data);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        _instance.WeaponImg.sprite = _instance.WeaponSprite[(int)weaponInfo.Type];
                    });
                    break;

                default:
                    Debug.Log("Class Not Define");
                    break;
            }
        }

        private void Start()
        {
            StartServerBtn.onClick.AddListener(StartServer);
            StopServerBtn.onClick.AddListener(StopServer);
            ConnectServerBtn.onClick.AddListener(ConnectServer);

            CharacterRpcBtn.onClick.AddListener(SendCharacterRpc);
            WeaponRpcBtn.onClick.AddListener(SendWeaponRpc);
            SkillRpcBtn.onClick.AddListener(SendEquipRpc);
        }

        private void StartServer()
        {
            GrpcServer.StartServer();
        }

        private void StopServer()
        {
            GrpcServer.StopServer();
        }

        private void ConnectServer()
        {
            GrpcClient.ConnectServer(GrpcServer.ip, GrpcServer.port);
        }

        private void SendCharacterRpc()
        {
            GrpcClient.SendGrpc(new CmdRequest
            {
                CmdName = nameof(HeroInfo),
                CmdData = JsonUtility.ToJson(new HeroInfo
                {
                    Name = CharacterDropdown.options[CharacterDropdown.value].ToString(),
                    Hp = 100,
                    Type = (HeroType)CharacterDropdown.value
                })
            });
        }

        private void SendWeaponRpc()
        {
            GrpcClient.SendGrpc(new CmdRequest
            {
                CmdName = nameof(WeaponInfo),
                CmdData = JsonUtility.ToJson(new WeaponInfo
                {
                    Name = WeaponDropdown.options[WeaponDropdown.value].ToString(),
                    Damage = 1000,
                    Type = (WeaponType)WeaponDropdown.value
                })
            });
        }

        private void SendEquipRpc()
        {
            GrpcClient.SendGrpc(new CmdRequest
            {
                CmdName = nameof(EquipmentInfo),
                CmdData = JsonUtility.ToJson(new EquipmentInfo
                {
                    Name = EquipDropdown.options[EquipDropdown.value].ToString(),
                    Value = 500,
                    Type = (EquipmentType)EquipDropdown.value
                })
            });
        }
    }
}