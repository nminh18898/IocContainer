using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IocContainer
{
    class IocContainer
    {
        private Dictionary<Type, Type> typesMap = new Dictionary<Type, Type>();

        //private Dictionary<Type, Object> singletonTypesMap = new Dictionary<Type, Object>();

       // LifeCycleManager lifeCycle;


        public enum LifeCycleManager { Singleton, Transient }


        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public void Register<from, to>()
            where from : class
            where to : class
        {
            if (typesMap.ContainsKey(typeof(from)))
            {
                typesMap[typeof(from)] = typeof(to);
            }
            else
            {
                typesMap.Add(typeof(from), typeof(to));
            }
        }

        private object Resolve(Type type)
        {
            Type resolvedType = findDependency(type);

            if (resolvedType != null)
            {
                ConstructorInfo constructor = resolvedType.GetConstructors().First();
                ParameterInfo[] parameters = constructor.GetParameters();

                if (!parameters.Any())
                {
                    return Activator.CreateInstance(resolvedType);
                }
                else
                {
                    return constructor.Invoke(
                        ResolveParameters(parameters)
                            .ToArray());
                }
            }
            else 
            {
                /*ConstructorInfo baseConstructor = type.GetConstructors().First();
                ParameterInfo[] baseParameters = baseConstructor.GetParameters();

                Type parameterType = baseParameters[0].ParameterType;
               
                resolvedType = findDependency(parameterType);

                if (resolvedType != null)
                {
                    object parameterObject = Activator.CreateInstance(resolvedType);
                    return Activator.CreateInstance(resolvedType, parameterObject);

                }
                else {
                    return null;
                }*/

                ConstructorInfo[] baseConstructor = type.GetConstructors();

                foreach (var ci in baseConstructor) {
                    ParameterInfo[] baseParameters = ci.GetParameters();

                    if (baseParameters.Length == 1) {
                        Type parameterType = baseParameters[0].ParameterType;
                        resolvedType = findDependency(parameterType);

                        if (resolvedType != null) {
                            object interfaceInstance = Activator.CreateInstance(resolvedType);
                            return ci.Invoke(new object[] { interfaceInstance });
                        }
                    }
                }

                var methods = type.GetMethods();

                foreach (var method in methods) // iterate through all found methods
                {
                    var attribute = method.GetCustomAttribute(typeof(IocAttributes));

                    if (attribute != null)
                    {
                        var parameterTypes = method.GetParameters();

                        if (parameterTypes.Length != 1)
                        {
                            return null;
                        }
                        else 
                        {
                            resolvedType = findDependency(parameterTypes[0].ParameterType);

                            if (resolvedType == null) 
                            {
                                return null;
                            }

                            var interfaceInstance = Activator.CreateInstance(resolvedType);

                            object objectInstance = Activator.CreateInstance(type);
                           
                            method.Invoke(objectInstance, new object[] { interfaceInstance });

                            return objectInstance;
                        }
                    }
                 
                }

                return null;


            }
        }

        private IEnumerable<object> ResolveParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters
                .Select(p => Resolve(p.ParameterType))
                .ToList();
        }

        private Type findDependency(Type type)
        {
            if (typesMap.ContainsKey(type))
            {

                return typesMap[type];
            }

            return null;
           
        }

    }


}
