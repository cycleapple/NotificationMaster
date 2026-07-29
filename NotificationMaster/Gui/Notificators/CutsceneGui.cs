namespace NotificationMaster;

internal partial class ConfigGui
{
    internal void DrawCutsceneConfig()
    {
        if(ImGui.Checkbox("啟用", ref p.cfg.cutscene_Enable))
        {
            CutsceneEnded.Setup(p.cfg.cutscene_Enable, p);
        }
        if(p.cfg.cutscene_Enable)
        {
            ImGui.Text("過場動畫結束且 FFXIV 在背景執行時：");
            ImGui.Checkbox("顯示系統匣通知", ref p.cfg.cutscene_ShowToastNotification);
            ImGui.Checkbox("閃爍工作列圖示", ref p.cfg.cutscene_FlashTrayIcon);
            ImGui.Checkbox("將 FFXIV 切換到前景", ref p.cfg.cutscene_AutoActivateWindow);
            ImGui.Checkbox("即使遊戲在前景也執行", ref p.cfg.cutscene_AlwaysExecute);
            ForegroundWarning(p.cfg.cutscene_AutoActivateWindow);
            DrawSoundSettings(ref p.cfg.cutscene_SoundSettings);
            ImGui.Text("區域限制：");
            ImGui.Checkbox("只在主線任務隨機任務副本中觸發", ref p.cfg.cutscene_OnlyMSQ);
            DrawHttpMaster(p.cfg.cutscene_HttpRequests, ref p.cfg.cutscene_HttpRequestsEnable,
                "None available");
        }
    }
}
