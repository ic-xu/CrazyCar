using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;
using Utils;

public class MatchRoomStatusUI : MonoBehaviour, IController {
    public Button closeBtn;
    public Button startBtn;
    public GameObject waitingText;
    public MatchRoomPlayerItem[] playerItems;
    public Button mapBtn;
    public MatchRoomMapUI mapUI;

    private int maxNum = 1;

    private void Awake() {
        this.RegisterEvent<MatchRoomUpdateStatusEvent>(OnMatchRoomUpdateStatus).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<MatchRoomStartEvent>(OnMatchRoomStart).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnEnable() {
        startBtn.gameObject.SetActiveFast(this.GetModel<IMatchModel>().IsHouseOwner);
        mapBtn.gameObject.SetActiveFast(this.GetModel<IMatchModel>().IsHouseOwner);
        waitingText.SetActiveFast(!this.GetModel<IMatchModel>().IsHouseOwner);
        this.GetSystem<IMatchRoomSystem>().MatchRoomStatus();
    }

    private void Start() {
        startBtn.onClick.AddListener(() => {
            if (this.GetModel<IMatchModel>().MemberInfoDic.Count >= maxNum) {
                if (IsLegalMap()) {
                    this.GetSystem<IMatchRoomSystem>().MatchRoomStart();
                } else {
                    this.SendCommand(new ShowWarningAlertCommand(this.GetSystem<II18NSystem>().GetText("This map requires all player vehicles to be able to wade")));
                }
            } else {
                this.SendCommand(new ShowWarningAlertCommand(this.GetSystem<II18NSystem>().GetText("Other players are not in position")));
            }  
        });

        closeBtn.onClick.AddListener(() => {
            gameObject.SetActiveFast(false);
        });

        mapBtn.onClick.AddListener(() => {
            mapUI.gameObject.SetActiveFast(true);
        });
    }

    private bool IsLegalMap() {
        if (this.GetModel<IMatchModel>().SelectInfo.Value.hasWater) {
            foreach (var item in this.GetModel<IMatchModel>().MemberInfoDic) {
                if (!item.Value.canWade) {
                    return false;
                }
            }
            return true;
        } else {
            return true;
        }
    }

    private void OnMatchRoomUpdateStatus(MatchRoomUpdateStatusEvent e) {
        List<MatchRoomMemberInfo> infos = new List<MatchRoomMemberInfo>(this.GetModel<IMatchModel>().MemberInfoDic.Values);
        for (int i = 0; i < playerItems.Length; i++) {
            if (i < infos.Count) {
                playerItems[i].SetContent(infos[i]);
            } else {
                playerItems[i].CleanItem();
            }
        }
    }

    private void OnMatchRoomStart(MatchRoomStartEvent e) {
        var matchInfo = this.GetModel<IMatchModel>().SelectInfo;
        this.GetSystem<INetworkSystem>().EnterRoom(GameType.Match, matchInfo.Value.cid, () => {
            this.SendCommand(new EnterMatchCommand(matchInfo));
        });
    }

    private void OnDisable() {
        this.GetSystem<IMatchRoomSystem>().MatchRoomEixt();
    }

    public IArchitecture GetArchitecture() {
        return CrazyCar.Interface;
    }
}
