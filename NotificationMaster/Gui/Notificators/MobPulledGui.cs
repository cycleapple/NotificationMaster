namespace NotificationMaster;

internal partial class ConfigGui
{
    private string mobToDelete = null;
    private bool mobAllowDeleting = false;
    private string mobsToAdd = "";
    private (string filter, bool onlyWorld, bool onlySelected) terrSearchOptions = ("", true, false);
    private int tCounter = 0;
    internal void DrawMobPulledConfig()
    {
        if(ImGui.Checkbox("啟用", ref p.cfg.mobPulled_Enable))
        {
            MobPulled.Setup(p.cfg.mobPulled_Enable, p);
        }
        if(p.cfg.mobPulled_Enable)
        {
            tCounter = 0;
            if(mobToDelete != null)
            {
                p.cfg.mobPulled_Names.Remove(mobToDelete);
                mobToDelete = null;
                p.mobPulled.RebuildMobNames();
                p.mobPulled.ClearIgnoredMobs();
            }
            ImGui.TextColored(ImGuiColors.DalamudOrange, "插件必須能看見目標，才能偵測目標進入戰鬥。\n" +
                "區域過度擁擠時，A 級惡名精英可能不會顯示。\n" +
                "S／SS 級通常仍會顯示，後續版本將再改善此限制。");
            ImGui.Text($"清單中的目標於指定區域進入戰鬥時執行下列動作{(p.cfg.mobPulled_AlwaysExecute ? "" : "（僅限 FFXIV 在背景執行時）")}：");
            ImGui.Checkbox("顯示系統匣通知", ref p.cfg.mobPulled_ShowToastNotification);
            ImGui.Checkbox("閃爍工作列圖示", ref p.cfg.mobPulled_FlashTrayIcon);
            ImGui.Checkbox("將 FFXIV 切換到前景", ref p.cfg.mobPulled_AutoActivateWindow);
            ForegroundWarning(p.cfg.mobPulled_AutoActivateWindow);
            ImGui.Checkbox("在聊天視窗顯示警告", ref p.cfg.mobPulled_ChatMessage);
            ImGui.Checkbox("顯示遊戲內通知", ref p.cfg.mobPulled_Toast);
            DrawSoundSettings(ref p.cfg.mobPulled_SoundSettings);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudOrange);
            ImGui.Checkbox("即使遊戲在前景也執行", ref p.cfg.mobPulled_AlwaysExecute);
            ImGui.PopStyleColor();
            DrawHttpMaster(p.cfg.mobPulled_HttpRequests, ref p.cfg.mobPulled_HttpRequestsEnable,
                "$M - 目標名稱");
            if(ImGui.CollapsingHeader($"監看目標清單（目前 {p.cfg.mobPulled_Names.Count} 個）###MPListmobs"))
            {
                ImGui.Checkbox("允許刪除項目", ref mobAllowDeleting);
                if(p.cfg.mobPulled_Names.Count > 0)
                {
                    ImGui.SameLine();
                    if(ImGui.Button("將目標名稱匯出至剪貼簿###Export mob names to clipboard"))
                    {
                        ImGui.SetClipboardText(string.Join("\n", p.cfg.mobPulled_Names));
                    }
                }
                foreach(var s in p.cfg.mobPulled_Names)
                {
                    if(mobAllowDeleting)
                    {
                        if(ImGui.SmallButton($"刪除##{s.GetHashCode()}"))
                        {
                            mobToDelete = s;
                        }
                        ImGui.SameLine();
                    }
                    ImGui.Text(s);
                }
                ImGui.TextColored(ImGuiColors.DalamudOrange, "新增目標（每行一個；區分大小寫；會移除多餘空白及重複項目）");
                ImGui.InputTextMultiline("##addMobs", ref mobsToAdd, 100000,
                    new Vector2(ImGui.GetContentRegionAvail().X, Math.Min((mobsToAdd.Split('\n').Length + 1) * ImGui.CalcTextSize("AAAAAAAA").Y, 300f)));
                if(ImGui.Button("新增目標###Add mobs"))
                {
                    foreach(var mob in mobsToAdd.Split("\n"))
                    {
                        var trimmed = mob.Trim();
                        if(trimmed.Length > 0)
                        {
                            p.cfg.mobPulled_Names.Add(trimmed);
                        }
                    }
                    mobsToAdd = "";
                    p.mobPulled.RebuildMobNames();
                    p.mobPulled.ClearIgnoredMobs();
                }
            }

            if(ImGui.CollapsingHeader($"啟用模組的區域清單（目前 {p.cfg.mobPulled_Territories.Count} 個）###MPListOfTerr"))
            {
                ImGui.SetNextItemWidth(200f);
                ImGui.InputTextWithHint("##terrSearch", "篩選……", ref terrSearchOptions.filter, 100);
                ImGui.SameLine();
                ImGui.Checkbox("只顯示野外區域", ref terrSearchOptions.onlyWorld);
                ImGui.SameLine();
                ImGui.Checkbox("只顯示已選取區域", ref terrSearchOptions.onlySelected);
                if(p.mobPulled.territories.TryGetValue(Svc.ClientState.TerritoryType, out var v))
                {
                    MPPrintZone(Svc.ClientState.TerritoryType, v);
                }
                foreach(var k in p.mobPulled.territories)
                {
                    MPPrintZone(k.Key, k.Value);
                }
            }
        }
    }

    private void MPPrintZone(uint territoryType, (string name, bool isWorld) v)
    {
        tCounter++;
        var cname = $"{territoryType} | {v.name}{(v.isWorld ? "（野外區域）" : "")}";
        if(terrSearchOptions.filter.Length > 0 && !cname.Contains(terrSearchOptions.filter, StringComparison.OrdinalIgnoreCase)) return;
        if(terrSearchOptions.onlyWorld && !v.isWorld) return;
        if(terrSearchOptions.onlySelected && !p.cfg.mobPulled_Territories.Contains(territoryType)) return;
        if(v.isWorld) ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
        var chk = p.cfg.mobPulled_Territories.Contains(territoryType);
        if(ImGui.Checkbox(cname + "##" + tCounter, ref chk))
        {
            if(chk)
            {
                p.cfg.mobPulled_Territories.Add(territoryType);
            }
            else
            {
                p.cfg.mobPulled_Territories.Remove(territoryType);
            }
            if(Svc.ClientState.IsLoggedIn) p.mobPulled.TerritoryChanged(Svc.ClientState.TerritoryType);
        }
        if(v.isWorld) ImGui.PopStyleColor();
    }
}
