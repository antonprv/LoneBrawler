// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Buffs;
using Code.Gameplay.Features.Player.Buffs.Interfaces;
using Code.Gameplay.Utils.ActorComponents;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;

using Cysharp.Threading.Tasks;

using Zenjex.Extensions.Core;

namespace Code.Gameplay.Features.Player.Buffs
{
  public class BuffComponent : AsyncStartMonoBehaviour, IBuffConsumer, IBuffReceiver
  {
    private IBuffFactory _buffFactory;
    private IBuffTrackerService _buffTracker;

    protected override void AsyncStart()
    {
      base.AsyncStart();
      AddAsService();
      InjectDependencies();
    }

    private void AddAsService() =>
      RootContext.Runtime.Bind<BuffComponent>()
      .FromInstance(this)
      .BindInterfacesAndSelf()
      .AsSingle();

    private void InjectDependencies()
    {
      _buffFactory = RootContext.Resolve<IBuffFactory>();
      _buffTracker = RootContext.Resolve<IBuffTrackerService>();
    }

    public async UniTaskVoid ReceiveBuff(BuffClassName className, int amount)
    {
      for (int i = 0; i < amount; i++)
      {
        var buff = await _buffFactory.CreateBuff(className, gameObject);
        _buffTracker.AddBuff(buff, className);
      }
    }

    public void ConsumeBuff(BuffBase buff) => buff.Activate();
  }
}
