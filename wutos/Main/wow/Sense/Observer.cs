using System;

namespace DelegateEvent
{
    /**//// <summary>
    /// 定义了另一个观察者基类.该观察者类型拥有两个响应行为.
    /// </summary>
    public abstract class Observer
    {
        /**//// <summary>
        /// 构造时通过传入模型对象,把观察者与模型关联,并完成订阅.
        /// 在此确定需要观察的模型对象.
        /// </summary>
        /// <param name="childModel">需要观察的对象</param>
        public Observer(ModelBase childModel)
        {
            //订阅
            //把观察者行为(这里是Response和Response2)注册于委托事件
            //childModel.SubEvent += new ModelBase.SubEventHandler(Response);
            //childModel.SubEvent += new ModelBase.SubEventHandler(Response2);
            
        }
        /**//// <summary>
        /// 规划了观察者的二种行为(方法),所有派生于该观察者基类的具体观察者都
        /// 通过覆盖该方法来实现作出响应的行为.
        /// </summary>
        public abstract void Response();
        public abstract void Response2();
    }
}