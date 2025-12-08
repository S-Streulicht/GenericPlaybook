using System;

namespace PB.Logic
{
  /**
  *  @brief   Interface of the event aggregator
  *  @details simple subscibe making new functioncalls available
  *           and publish: trigger event
  */
  public interface IEventAggregator
  {
    void Subscribe<T>(Action<T> handler);  /**< add a function to the event ahndling */
    void Publish<T>(T eventData);          /**< trigger an event */
  }
}
