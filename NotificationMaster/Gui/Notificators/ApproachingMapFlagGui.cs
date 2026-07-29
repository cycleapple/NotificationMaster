namespace NotificationMaster;

internal unsafe partial class ConfigGui
{
    internal void DrawMapFlagConfig()
    {
        /*ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
        ImGui.TextWrapped("Warning: this feature is EXPERIMENTAL!");
        ImGui.PopStyleColor();*/
        var curPosEnable = ImGui.GetCursorPos();
        if(ImGui.Checkbox("啟用", ref p.cfg.mapFlag_Enable))
        {
            ApproachingMapFlag.Setup(p.cfg.mapFlag_Enable, p);
        }
        if(p.cfg.mapFlag_Enable)
        {
            var curPosCont = ImGui.GetCursorPos();
            var distance = 0f;
            ImGui.SetCursorPos(new Vector2(500f, curPosEnable.Y));
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
            ImGui.Text("Debug info: ");
            ImGui.SetCursorPosX(500f);
            ImGui.Text($"Flag state: {p.mapFlag.isFlagSet}");
            ImGui.SetCursorPosX(500f);
            ImGui.Text($"Flag territory: {p.mapFlag.flagTerritory}");
            ImGui.SetCursorPosX(500f);
            ImGui.Text($"Flag X: {p.mapFlag.flagX}");
            ImGui.SetCursorPosX(500f);
            ImGui.Text($"Flag Y: {p.mapFlag.flagY}");
            if(Svc.ClientState.LocalPlayer != null)
            {
                ImGui.SetCursorPosX(500f);
                ImGui.Text($"Player X: {Svc.ClientState.LocalPlayer.Position.X}");
                ImGui.SetCursorPosX(500f);
                ImGui.Text($"Player Y: {Svc.ClientState.LocalPlayer.Position.Z}");
                ImGui.SetCursorPosX(500f);
                ImGui.Text($"Territory: {Svc.ClientState.TerritoryType}");
                ImGui.SetCursorPosX(500f);
                distance = Vector2.Distance(new Vector2(p.mapFlag.flagX, p.mapFlag.flagY),
                    new Vector2(Svc.ClientState.LocalPlayer.Position.X, Svc.ClientState.LocalPlayer.Position.Z));
                ImGui.Text($"Distance: {distance}");
            }
            ImGui.PopStyleColor();
            ImGui.SetCursorPos(curPosCont);
            ImGui.Text("FFXIV 在背景執行且接近地圖標記時：");
            ImGui.Checkbox("顯示系統匣通知", ref p.cfg.mapFlag_ShowToastNotification);
            ImGui.Checkbox("閃爍工作列圖示", ref p.cfg.mapFlag_FlashTrayIcon);
            ImGui.Checkbox("將 FFXIV 切換到前景", ref p.cfg.mapFlag_AutoActivateWindow);
            ImGui.Checkbox("即使遊戲在前景也執行", ref p.cfg.mapFlag_AlwaysExecute);
            ForegroundWarning(p.cfg.mapFlag_AutoActivateWindow);
            ImGui.SetNextItemWidth(100f);
            ImGui.DragInt("標記觸發距離", ref p.cfg.mapFlag_TriggerDistance);
            ImGui.Text("注意：此數值使用遊戲世界座標距離，而非地圖座標距離。");
            if(p.mapFlag.isFlagSet && Svc.ClientState.TerritoryType == p.mapFlag.flagTerritory)
            {
                ImGui.Text($"目前距離標記 {distance:0} 米。");
            }
            else
            {
                ImGui.Text("請先在地圖設定標記，以顯示目前距離");
            }
            ImGui.Checkbox("尚未到達設定距離，但越過標記的 X/Y 軸時也觸發", ref p.cfg.mapFlag_TriggerOnCross);
            ImGui.SetNextItemWidth(100f);
            ImGui.DragInt("越軸容許值", ref p.cfg.mapFlag_CrossDelta);
            DrawSoundSettings(ref p.cfg.mapFlag_SoundSettings);
            DrawHttpMaster(p.cfg.mapFlag_HttpRequests, ref p.cfg.mapFlag_HttpRequestsEnable,
                "無可用占位符");
        }
    }
}
