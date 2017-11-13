using strange.extensions.context.api;
using strange.extensions.context.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext : MVCSContext {

    public GameContext()
    {

    }
    public GameContext(MonoBehaviour view,bool autoMapping) : base(view, autoMapping)
    {

    }
    protected override void mapBindings()
    {
        Debug.Log("Start MapBindings...");
        MapModel();
        MapView();
        MapController();
        commandBinder.Bind(ContextEvent.START).To<StartUpCommand>().Once();
    }
    private void MapModel()
    {
        injectionBinder.Bind<IGameData>().To<GameDataCenter>().ToSingleton();
        injectionBinder.Bind<IClientService>().To<SendDataService>().ToSingleton();
    }
    private void MapView()
    {
        mediationBinder.Bind<ClientService>().To<ClientServiceMediator>();
        mediationBinder.Bind<OptionMenu_View>().To<OptionMenu_Mediator>();
        mediationBinder.Bind<Player3_View>().To<Player3_Mediator>();
        mediationBinder.Bind<Player2_View>().To<Player2_Mediator>();
        mediationBinder.Bind<Player1_View>().To<Player1_Mediator>();
        mediationBinder.Bind<Status_View>().To<StatusMediator>();
        mediationBinder.Bind<CurrentCard_View>().To<CurrentCardMediator>();
        mediationBinder.Bind<GameStartUI_View>().To<GameStartUI_Mediator>();
        mediationBinder.Bind<GameLobbyUI_View>().To<GameLobbyUI_Mediator>();
        mediationBinder.Bind<GameOverUI_View>().To<GameOverUI_Mediator>();
        mediationBinder.Bind<LocalService_View>().To<LocalService_Mediator>();
    }
    private void MapController()
    {
        commandBinder.Bind(NotificationConst.Noti_CallLandlord).To<CallLandlordCommand>();
        commandBinder.Bind(NotificationConst.Noti_NotCall).To<NotCallCommand>();
        commandBinder.Bind(NotificationConst.Noti_Discard).To<DiscardCommand>();
        commandBinder.Bind(NotificationConst.Noti_Claim).To<ClaimCommand>();
        commandBinder.Bind(NotificationConst.Noti_NotClaim).To<NotClaimCommand>();
        commandBinder.Bind(NotificationConst.Noti_Pass).To<PassCommand>();
        commandBinder.Bind(NotificationConst.Noti_MatchSuccess).To<MatchSuccessCommand>();
        commandBinder.Bind(NotificationConst.Noti_Player2).To<Player2Command>();
        commandBinder.Bind(NotificationConst.Noti_Player3).To<Player3Command>();
        commandBinder.Bind(NotificationConst.Noti_StartPlayer).To<StartPlayerCommand>();
        commandBinder.Bind(NotificationConst.Noti_SendRecData).To<SendRecDataCommand>();
        commandBinder.Bind(NotificationConst.Noti_UpdateSelectedCards).To<UpdateSelectedCardsCommand>();
        commandBinder.Bind(NotificationConst.Noti_DealCards).To<DealCardsCommand>();
        commandBinder.Bind(NotificationConst.Noti_BaseCards).To<BaseCardsCommand>();
        commandBinder.Bind(NotificationConst.Noti_GameTurn).To<GameTurnCommand>();
        commandBinder.Bind(NotificationConst.Noti_UpdatetTimer).To<UpdateTimerCommand>();
        commandBinder.Bind(NotificationConst.Noti_ShowStartGameUI).To<ShowGameStartUICommand>();
        commandBinder.Bind(NotificationConst.Noti_ShowMainGameUI).To<ShowMainUICommand>();
        commandBinder.Bind(NotificationConst.Noti_ShowGameLobbyUI).To<ShowGameLobbyUICommand>();
        commandBinder.Bind(NotificationConst.Noti_UpdatePlayerIdentity).To<UpdatePlayerIdentityCommand>();
        commandBinder.Bind(NotificationConst.Noti_GameOver).To<GameOverCommand>();
        commandBinder.Bind(NotificationConst.Noti_StartPVE).To<StartPVECommand>();
        commandBinder.Bind(NotificationConst.Noti_Tip).To<TipsCommand>();
    }
}
