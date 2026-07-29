using Dalamud.Game.ClientState.Objects.Enums;

namespace NotificationMaster;

internal unsafe partial class ConfigGui
{
    internal void DrawGpNotify()
    {
        var curPosEnable = ImGui.GetCursorPos();
        if(ImGui.Checkbox("啟用##gpn", ref p.cfg.gp_Enable))
        {
            GpNotify.Setup(p.cfg.gp_Enable, p);
        }
        if(p.cfg.gp_Enable)
        {
            var curPosCont = ImGui.GetCursorPos();
            ImGui.SetCursorPos(new Vector2(500f, curPosEnable.Y));
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
            ImGui.Text("除錯資訊：");
            ImGui.SetCursorPosX(500f);
            ImGui.Text($"附近採集點：{Svc.Objects.Count(x => x.ObjectKind == ObjectKind.GatheringPoint)}");
            ImGui.SetCursorPosX(500f);
            ImGui.Text($"Potion cooldown: {FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetRecastGroupDetail(GpNotify.PotionCDGroup)->IsActive}");
            ImGui.PopStyleColor();
            ImGui.SetCursorPos(curPosCont);
            ImGui.SetNextItemWidth(100f);
            ImGui.DragInt("採集力達到此數值時通知", ref p.cfg.gp_GPTreshold, 1f, 0, 10000);
            ImGui.Text("可使用 /gp <數值> 快速變更門檻");
            ImGui.SetNextItemWidth(100f);
            ImGui.DragInt("恢復藥恢復量", ref p.cfg.gp_PotionCapacity, 1f, 0, 1000);
            ImGui.SetNextItemWidth(100f);
            ImGui.DragInt("容許差值", ref p.cfg.gp_Tolerance, 1f, 0, 100);
            ImGui.Checkbox("附近沒有採集點時不通知", ref p.cfg.gp_SuppressIfNoNodes);
            if(ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("採集力僅低於門檻此數值以內時，恢復後不會再次傳送通知。");
            }
            ImGui.Text("通知選項：");
            ImGui.Checkbox("顯示系統匣通知", ref p.cfg.gp_ShowToastNotification);
            ImGui.Checkbox("閃爍工作列圖示", ref p.cfg.gp_FlashTrayIcon);
            ImGui.Checkbox("將 FFXIV 切換到前景", ref p.cfg.gp_AutoActivateWindow);
            ImGui.Checkbox("即使遊戲在前景也執行", ref p.cfg.gp_AlwaysExecute);
            ForegroundWarning(p.cfg.gp_AutoActivateWindow);
            DrawSoundSettings(ref p.cfg.gp_SoundSettings);
            DrawHttpMaster(p.cfg.gp_HttpRequests, ref p.cfg.gp_HttpRequestsEnable,
                "$G - 目前採集力");
        }
    }
}
