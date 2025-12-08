using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Logic
{
  /**
  *  @brief   Handles the events comming from somware going to somware else
  *  @details the aggregator is maximum flexible it dosent know wher the information
  *           is comming from and wher it goes. With the Subsribe function called by
  *           the event reseiving class the target gets nown
  *           The sender class will use Publish to call something.
  *           The available events are found in Logic/Events as separt record
  */
  public class EventAggregator : MonoBehaviour, IEventAggregator
  {
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new(); /**< THe dictonary to store all subscritions: Eventtype and delegated function*/

    /**
    *  @brief   handles the subsciption of new events
    *  @details The event is the delegated function of Type T (the event type) is given by the parameter
    *           T is defining which type it is. The function will extract the actual type and
    *           if the type is not already in the dictonary adds a new list in the dictionary
    *           at the end the actual function is added to the list inside the dictionary
    *  @param   handler the actual delegate.
    */
    public void Subscribe<T>(Action<T> handler)
    {
      var type = typeof(T);
      
      if (!_subscribers.ContainsKey(type))
      {
        _subscribers[type] = new List<Delegate>();
      }
      _subscribers[type].Add(handler);
    }

    /**
    *  @brief   handles the execution of the events
    *  @details cheks of the requested event is part of dictionary.
    *           If it is execute all function from tzhe assigned list
    *  @param   eventData the acttual trigger event
    */
    public void Publish<T>(T eventData)
    {
      var type = typeof(T);
      if (_subscribers.TryGetValue(type, out var handlers))
      {
        foreach (var handler in handlers.Cast<Action<T>>())
        {
          handler(eventData);
        }
      }
    }
  }
}