using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KusoGame.Signals{
    public class Switch{
        ReadOnlyCollection<Delegate> handlerList;
        
        public Switch(ReadOnlyCollection<Delegate> _handlerList){
            handlerList = _handlerList;
        }
        public List<dynamic> Invoke(params dynamic[] args){
            List<dynamic> retVals = new List<dynamic>();
            foreach(Delegate handler in handlerList){
                retVals.Add(handler.DynamicInvoke(args));
            }
            return retVals;
        }
    }
}