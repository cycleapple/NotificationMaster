using Dalamud.Interface.Utility;
using ECommons.Funding;

namespace NotificationMaster;

internal partial class ConfigGui : IDisposable
{
    internal bool open = false;
    internal NotificationMaster p;
    internal ConfigGui(NotificationMaster p)
    {
        this.p = p;
        Svc.PluginInterface.UiBuilder.Draw += Draw;
        PatreonBanner.IsOfficialPlugin = () => true;
    }

    public void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= Draw;
    }

    internal void Draw()
    {
        if(p.PauseUntil > Environment.TickCount64)
        {
            ImGuiHelpers.ForceNextWindowMainViewport();
            var sb = new StringBuilder("NotificationMaster 已暫停");
            if(p.PauseUntil != long.MaxValue)
            {
                var ts = TimeSpan.FromMilliseconds(p.PauseUntil - Environment.TickCount64);
                sb.Append($"，剩餘 {(ts.Days * 24 + ts.Hours):D2}:{ts.Minutes:D2}:{ts.Seconds:D2}");
            }
            var text = sb.ToString();
            var dims = ImGui.CalcTextSize(text);
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(ImGuiHelpers.MainViewport.Size.X / 2 - dims.X / 2, 10f));
            ImGui.Begin("NotificationMasterPauseWarning", ImGuiWindowFlags.NoNav
            | ImGuiWindowFlags.NoFocusOnAppearing
            | ImGuiWindowFlags.NoTitleBar
            | ImGuiWindowFlags.NoBackground
            | ImGuiWindowFlags.AlwaysAutoResize
            | ImGuiWindowFlags.NoInputs);
            ImGui.TextColored(ImGuiColors.DalamudOrange, text);
            ImGui.End();
        }
        if(open)
        {

            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(650f, 200f));
            if(ImGui.Begin("NotificationMaster 設定", ref open))
            {
                if(p.fileSelector.IsSelecting())
                {
                    ImGui.Text("等待選擇檔案……");
                }
                else
                {
                    PatreonBanner.DrawRight();
                    ImGui.BeginTabBar("##NMtabs");
                    DrawTab("採集力恢復", DrawGpNotify, p.cfg.gp_Enable);
                    DrawTab("過場動畫結束", DrawCutsceneConfig, p.cfg.cutscene_Enable);
                    DrawTab("聊天訊息", DrawChatMessageGui, p.cfg.chatMessage_Enable);
                    DrawTab("任務配對", DrawCfPopConfig, p.cfg.cfPop_Enable);
                    DrawTab("連線錯誤", DrawLoginErrorConfig, p.cfg.loginError_Enable);
                    DrawTab("接近地圖標記", DrawMapFlagConfig, p.cfg.mapFlag_Enable);
                    DrawTab("狩獵目標開戰", DrawMobPulledConfig, p.cfg.mobPulled_Enable);
                    DrawTab("招募隊員", DrawPartyFinderConfig, p.cfg.partyFinder_Enable);
                    PatreonBanner.RightTransparentTab();
                    ImGui.EndTabBar();
                }
            }
            ImGui.End();
            if(!open)
            {
                p.cfg.Save();
                Notify.Success("設定已儲存");
            }
            ImGui.PopStyleVar();
        }
    }

    private void DrawTab(string name, Action function, bool enabled)
    {
        var colored = false;
        if(enabled)
        {
            colored = true;
            ImGui.PushStyleColor(ImGuiCol.Text, 0xff00ff00);
        }
        if(ImGui.BeginTabItem($"{name}"))
        {
            if(colored) ImGui.PopStyleColor();
            ImGui.BeginChild($"##{name}-child");
            function();
            ImGui.EndChild();
            ImGui.EndTabItem();
        }
        else
        {
            if(colored) ImGui.PopStyleColor();
        }
    }

    private void ForegroundWarning(bool display)
    {
        if(display)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "將 FFXIV 切換到前景並非完全可靠。\n若此功能在你的環境無法運作，目前沒有穩定的替代方式。");
        }
    }
}
