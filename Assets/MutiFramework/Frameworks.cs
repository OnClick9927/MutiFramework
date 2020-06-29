using System;
using System.Linq;
using UnityEngine;

namespace MutiFramework
{
    /// <summary>
    /// 运行时主入口
    /// </summary>
    public class Frameworks : MonoBehaviour
    {
//ToDo
		public static ExampleFrame1 ExampleFrame1{ get { return GetFramework("ExampleFrame1") as ExampleFrame1;}} 
		public static ExampleFrame2 ExampleFrame2{ get { return GetFramework("ExampleFrame2") as ExampleFrame2;}} 
//ToDo

        /// <summary>
        /// 容器
        /// </summary>
        public static MutiFrameworkContaner container;
        /// <summary>
        /// 开启
        /// </summary>
        static void Startup()
        {
            container = new MutiFrameworkContaner();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany((a) => { return a.GetTypes(); });
            types
                 .Where((type) => {
                     return !type.IsAbstract && type.IsSubclassOf(typeof(Framework)) &&
         type.IsDefined(typeof(FrameworkAttribute), false) &&
         (type.GetCustomAttributes(typeof(FrameworkAttribute), false).First() as FrameworkAttribute).env.HasFlag(EnvironmentType.Editor);
                 })
                 .Select((type) => {
                     Framework f = Activator.CreateInstance(type) as Framework;
                     f.env = EnvironmentType.Editor;
                     return f;
                 }).ToList()
                 .ForEach((f) => {
                     container.Subscribe(f);
                 });

        }
        /// <summary>
        /// 获取框架
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Framework GetFramework(string name)
        {
            return container.Get(name);
        }
        public void Start()
        {
            Startup();
            container.Startup();
        }
        public void Update()
        {
            container.Update();
        }
        public void OnDisable()
        {
            container.Dispose();
            container = null;
        }
    }
}

