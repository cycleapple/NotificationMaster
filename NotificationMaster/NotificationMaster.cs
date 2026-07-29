using Dalamud.Game.Command;
using ECommons.Configuration;
using ECommons.EzIpcManager;
using NotificationMaster.Notificators;
using NotificationMasterAPI;

namespace NotificationMaster;

public class NotificationMaster : IDalamudPlugin
{
    internal bool IsDisposed = false;
    internal Configuration cfg;
    internal ConfigGui configGui;

    internal GpNotify gpNotify = null;
    internal CutsceneEnded cutsceneEnded = null;
    internal ChatMessage chatMessage = null;
    internal CfPop cfPop = null;
    internal LoginError loginError = null;
    internal ApproachingMapFlag mapFlag = null;
    internal MobPulled mobPulled = null;
    internal PartyFinder partyFinder = null;

    internal HttpMaster httpMaster;
    public ThreadUpdateActivatedState ThreadUpdActivated;
    internal AudioSelector fileSelector = new();
    internal AudioPlayer audioPlayer;

    internal long PauseUntil = 0;
    internal static NotificationMaster P;

    internal IPC IPC;
    internal NotificationMasterApi NotificationMasterApi;

    [EzIPC("AutoDuty.IsStopped", false)] internal Func<bool> AutoDutyIsStopped;

    public string Name => "NotificationMaster";

    public NotificationMaster(IDalamudPluginInterface pluginInterface)
    {
        P = this;
        ECommonsMain.Init(pluginInterface, this, Module.DalamudReflector);
        EzConfig.PluginConfigDirectoryOverride = "NotificationMaster";
        new TickScheduler(() =>
        {
            EzConfig.Migrate<Configuration>();
            cfg = EzConfig.Init<Configuration>();
            cfg.Initialize(Svc.PluginInterface);
            httpMaster = new();
            ThreadUpdActivated = new();
            audioPlayer = new(this);

            configGui = new(this);
            Svc.PluginInterface.UiBuilder.OpenConfigUi += delegate { configGui.open = true; };

            if(cfg.gp_Enable) GpNotify.Setup(true, this);
            if(cfg.cutscene_Enable) CutsceneEnded.Setup(true, this);
            if(cfg.chatMessage_Enable) ChatMessage.Setup(true, this);
            if(cfg.cfPop_Enable) CfPop.Setup(true, this);
            if(cfg.loginError_Enable) LoginError.Setup(true, this);
            if(cfg.mapFlag_Enable) ApproachingMapFlag.Setup(true, this);
            if(cfg.mobPulled_Enable) MobPulled.Setup(true, this);
            if(cfg.partyFinder_Enable) PartyFinder.Setup(true, this);

            if(Svc.PluginInterface.Reason == PluginLoadReason.Installer)
            {
                configGui.open = true;
                Notify.Warning(
                    "NotificationMaster 已安裝完成，預設不會啟用任何通知模組。\n" +
                    "設定視窗已開啟，請依需求完成設定。");
            }
            Svc.Commands.AddHandler("/pnotify", new CommandInfo(OnCommand)
            {
                HelpMessage = "開啟／關閉設定\n" +
                "/pnotify shutup|s [分鐘] - 暫停指定分鐘數；未指定時間則暫停至重新啟動\n" +
                "/pnotify resume|r - 恢復插件運作"
            });
            IPC = new();
            NotificationMasterApi = new(Svc.PluginInterface);
            EzIPC.Init(this);
        });
    }

    private void OnCommand(string command, string arguments)
    {
        if(arguments == "")
        {
            configGui.open = !configGui.open;
        }
        else
        {
            var args = arguments.Split(' ');
            if(args[0].Equals("shutup", StringComparison.OrdinalIgnoreCase) || args[0].Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                if(args.Length == 1)
                {
                    PauseUntil = long.MaxValue;
                    Notify.Success("插件已暫停，直到重新啟動");
                }
                else
                {
                    if(uint.TryParse(args[1], out var minutes))
                    {
                        PauseUntil = Environment.TickCount64 + minutes * 60 * 1000;
                        Notify.Success($"插件已暫停 {minutes} 分鐘");
                    }
                    else
                    {
                        Notify.Error("請輸入要暫停的分鐘數");
                    }
                }
            }
            else if(args[0].Equals("resume", StringComparison.OrdinalIgnoreCase) || args[0].Equals("r", StringComparison.OrdinalIgnoreCase))
            {
                PauseUntil = 0;
                Notify.Success("插件已恢復運作");
            }
            else
            {
                Notify.Error("無效的指令");
            }
        }
    }

    public void Dispose()
    {
        TrayIconManager.DestroyIcon();
        GpNotify.Setup(false, this);
        CutsceneEnded.Setup(false, this);
        ChatMessage.Setup(false, this);
        CfPop.Setup(false, this);
        LoginError.Setup(false, this);
        ApproachingMapFlag.Setup(false, this);
        MobPulled.Setup(false, this);
        ThreadUpdActivated.Dispose();
        audioPlayer.Dispose();
        cfg.Save();
        configGui.Dispose();
        Svc.Commands.RemoveHandler("/pnotify");
        GenericHelpers.Safe(() => IPC.Dispose());
        IsDisposed = true;
        ECommonsMain.Dispose();
        P = null;
    }
}
