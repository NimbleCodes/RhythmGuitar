using UnityEngine;
using System.Collections.Generic;

public class EventHandle
{
    string _eventId;
    public string eventId{
        get{
            return _eventId;
        }
    }
    List<Observer.EventHandler> handlerList;

    public EventHandle(string __eventId, List<Observer.EventHandler> _handlerList){
        _eventId = __eventId;
        handlerList = _handlerList;
    }
    public void Invoke(params System.Object[] parameters){
        handlerList.ForEach((h)=>{
            h.Invoke(parameters);
        });
    }
}
public class Observer
{
    public delegate void EventHandler(params System.Object[] parameters);
    Dictionary<string, List<EventHandler>> handlerLists;

    public Observer(){
        handlerLists = new Dictionary<string, List<EventHandler>>();
    }
    public EventHandle Register(string eventId){
        string _eventId = eventId;
        int i = 0;
        while(handlerLists.ContainsKey(_eventId)){
            _eventId = eventId + "_" + i;
        }
        List<EventHandler> handlerList = new List<EventHandler>();
        handlerLists.Add(_eventId, handlerList);
        return new EventHandle(_eventId, handlerList);
    }
    public void Subscribe(string eventId, EventHandler handler){
        if(handlerLists.ContainsKey(eventId)){
            handlerLists[eventId].Add(handler);
        }
    }
    public void Unsubscribe(string eventId, EventHandler handler){
        if(handlerLists.ContainsKey(eventId)){
            handlerLists[eventId].Remove(handler);
        }
    }
    public bool tostring(){
        return handlerLists.ContainsKey("update");
    }
}
