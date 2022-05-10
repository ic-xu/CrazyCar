﻿using LitJson;
using System.Text;
using UnityEngine;
using Utils;
using QFramework;
public class EnableStandAloneCommand : AbstractCommand {
    protected override void OnExecute() {
        this.GetModel<IGameControllerModel>().StandAlone.Value = true;
        TextAsset ta = Resources.Load<TextAsset>(Util.baseStandAlone + RequestUrl.loginUrl);
        JsonData data = JsonMapper.ToObject(ta.text);
        this.GetModel<IGameControllerModel>().Token.Value = (string)data["token"];
        this.GetSystem<IDataParseSystem>().ParseSelfUserInfo(data);

        this.SendEvent<ShowWarningAlertEvent>(new ShowWarningAlertEvent(text: this.GetSystem<II18NSystem>().GetText("Login Success"),
            callback: () => {
                Util.LoadingScene(SceneID.Index);
            }));
    }
}
