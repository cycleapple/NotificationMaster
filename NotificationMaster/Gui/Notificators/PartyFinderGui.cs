using NotificationMaster.Notificators;

namespace NotificationMaster;

internal partial class ConfigGui
{
    internal void DrawPartyFinderConfig()
    {
        if(ImGui.Checkbox("啟用", ref p.cfg.partyFinder_Enable))
        {
            PartyFinder.Setup(p.cfg.partyFinder_Enable, p);
        }
        if(p.cfg.partyFinder_Enable)
        {
            ImGui.Checkbox("只在隊伍滿員時通知", ref p.cfg.partyFinder_OnlyWhenFilled);
            ImGui.Checkbox("招募被取消時通知", ref p.cfg.partyFinder_Delisted);

            if(p.cfg.partyFinder_OnlyWhenFilled)
            {
                if(p.cfg.partyFinder_Delisted)
                {
                    ImGui.Text("隊伍滿員或招募被取消時");
                }
                else
                {
                    ImGui.Text("隊伍滿員時");
                }
            }
            else
            {
                if(p.cfg.partyFinder_Delisted)
                {
                    ImGui.Text("有人加入或離開隊伍，或招募被取消時");
                }
                else
                {
                    ImGui.Text("有人加入或離開隊伍時");
                }
            }

            ImGui.Text("若 FFXIV 在背景執行，進行下列動作：");

            ImGui.Checkbox("顯示系統匣通知", ref p.cfg.partyFinder_ShowToastNotification);
            ImGui.Checkbox("閃爍工作列圖示", ref p.cfg.partyFinder_FlashTrayIcon);
            ImGui.Checkbox("將 FFXIV 切換到前景", ref p.cfg.partyFinder_AutoActivateWindow);
            ForegroundWarning(p.cfg.partyFinder_AutoActivateWindow);
            DrawSoundSettings(ref p.cfg.partyFinder_SoundSettings);
        }
    }
}
