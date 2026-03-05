// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Linq;

using Code.Common.Extensions.Logging;
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
    private IGameLog _logger;

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
      _logger = RootContext.Resolve<IGameLog>();
    }

    public async UniTaskVoid ReceiveBuff(BuffClassName className, int amount)
    {
      for (int i = 0; i < amount; i++)
      {
        var buff = await _buffFactory.CreateBuff(className, gameObject);
        _buffTracker.AddBuff(buff, className);
      }
    }

    public void ConsumeBuff(BuffClassName buffClass)
    {
      BuffBase buff = _buffTracker.GetPlayerBuffs(buffClass).FirstOrDefault();
      buff?.Activate();
      _logger.Log($"Consumed buff {buffClass}");
    }
  }
}
