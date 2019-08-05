using System;
 
namespace DelegateEvent
{
    /**//// <summary>
    ///     此抽象类无抽象方法,主要是为了不能实例化该类对象,确保模式完整性.
    ///     具体实施:
    ///     1.声明委托
    ///     2.声明委托类型事件
    ///     3.提供触发事件的方法
    /// </summary>
    public abstract class ModelBase
    {
        public ModelBase()
        {
        }
        /**//// <summary>
        /// 声明一个委托,用于代理一系列"无返回"及"不带参"的自定义方法
        /// </summary>
        public delegate void SubEventHandler(long id, string type , object obj ); 
        /**//// <summary>
        /// 声明一个绑定于上行所定义的委托的事件
        /// </summary>
        public event SubEventHandler SubEvent;
 
        /**//// <summary>
        /// 封装了触发事件的方法
        /// 主要为了规范化及安全性,除观察者基类外,其派生类不直接触发委托事件
        /// </summary>
        protected void Notify(long id, string type, object obj)
        {
            //提高执行效率及安全性
            if(this.SubEvent!=null)
                this.SubEvent(id,type,obj);               
        }
    }
}