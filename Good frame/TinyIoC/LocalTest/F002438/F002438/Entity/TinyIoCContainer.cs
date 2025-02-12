﻿using F002438.CoreException;
using F002438.Entity.Enum;
using F002438.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace F002438.Entity
{
    public sealed partial class TinyIoCContainer : IDisposable
    {
        /// <summary>
        /// 封装Type 与 获得 其实例工厂 的字典
        /// </summary>
        private readonly SafeDictionary<TypeRegistration, ObjectFactoryBase> RegisteredTypes;

        #region 单例容器

        /// <summary>
        /// 单例容器
        /// </summary>
        private static readonly TinyIoCContainer instanceContainer = new TinyIoCContainer();

        static TinyIoCContainer()
        {
        }

        /// <summary>
        /// 单例容器
        /// </summary>
        public static TinyIoCContainer InstanceContainer
        {
            get { return instanceContainer; }
        }

        #endregion

        public TinyIoCContainer()
        {
            RegisteredTypes = new SafeDictionary<TypeRegistration, ObjectFactoryBase>();
            RegisterDefaultTypes();
        }

        private TinyIoCContainer parentContainer;

        private TinyIoCContainer(TinyIoCContainer parent)
            : this()
        {
            parentContainer = parent;
        }


        #region "Fluent" API

        /// <summary>
        /// 封装 TinyIoCContainer 和  TypeRegistration 的数据类型。往IOC注册类型的接口
        /// </summary>
        public sealed class RegisterOptions
        {
            private TinyIoCContainer container;
            private TypeRegistration registration;

            public RegisterOptions(TinyIoCContainer container, TypeRegistration registration)
            {
                this.container = container;
                this.registration = registration;
            }

            public RegisterOptions AsSingleton()
            {
                ObjectFactoryBase currentFactory = container.GetCurrentFactory(registration);

                if (currentFactory == null)
                    throw new TinyIoCRegistrationException(registration.Type, "singleton");

                return container.AddUpdateRegistration(registration, currentFactory.SingletonVariant);
            }

            public RegisterOptions AsMultiInstance()
            {
                ObjectFactoryBase currentFactory = container.GetCurrentFactory(registration);
                if (currentFactory == null)
                    throw new TinyIoCRegistrationException(registration.Type, "multi-instance");

                return container.AddUpdateRegistration(registration, currentFactory.MultiInstanceVariant);
            }


            public RegisterOptions WithWeakReference()
            {
                ObjectFactoryBase currentFactory = container.GetCurrentFactory(registration);
                if (currentFactory == null)
                    throw new TinyIoCRegistrationException(registration.Type, "weak reference");

                return container.AddUpdateRegistration(registration, currentFactory.WeakReferenceVariant);
            }


            public RegisterOptions WithStrongReference()
            {
                ObjectFactoryBase currentFactory = container.GetCurrentFactory(registration);
                if (currentFactory == null)
                    throw new TinyIoCRegistrationException(registration.Type, "strong reference");

                return container.AddUpdateRegistration(registration, currentFactory.StrongReferenceVariant);
            }

            public static RegisterOptions ToCustomLifetimeManager(
                RegisterOptions instance,
                ITinyIoCObjectLifetimeProvider lifetimeProvider,
                string errorString)
            {
                if (instance == null)
                    throw new ArgumentNullException("instance", "instance is null.");

                if (lifetimeProvider == null)
                    throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");

                if (string.IsNullOrEmpty(errorString))
                    throw new ArgumentException("errorString is null or empty.", "errorString");

                ObjectFactoryBase currentFactory = instance.container.GetCurrentFactory(instance.registration);

                if (currentFactory == null)
                    throw new TinyIoCRegistrationException(instance.registration.Type, errorString);

                return instance.container.AddUpdateRegistration(
                    instance.registration,
                    currentFactory.GetCustomObjectLifetimeVariant(lifetimeProvider, errorString));
            }
        }

        /// <summary>
        /// 封装批量注册Type到容器的对象 IEnumerable【RegisterOptions】
        /// 
        /// 可以实现多个 RegisterOptions  以 不同方式
        /// AsSingleton
        /// AsMultiInstance
        /// ToCustomLifetimeManager【用户自定义】
        /// 
        /// 注入到IOC 容器
        /// </summary>
        public sealed class MultiRegisterOptions
        {
            private IEnumerable<RegisterOptions> registerOptions;

            public MultiRegisterOptions(IEnumerable<RegisterOptions> registerOptions)
            {
                this.registerOptions = registerOptions;
            }

            public MultiRegisterOptions AsSingleton()
            {
                registerOptions = ExecuteOnAllRegisterOptions(registerOption => registerOption.AsSingleton());
                return this;
            }

            public MultiRegisterOptions AsMultiInstance()
            {
                registerOptions = ExecuteOnAllRegisterOptions(registerOption => registerOption.AsMultiInstance());
                return this;
            }


            public static MultiRegisterOptions ToCustomLifetimeManager(
                MultiRegisterOptions instance,
                ITinyIoCObjectLifetimeProvider lifetimeProvider,
                string errorString)
            {
                if (instance == null)
                    throw new ArgumentNullException("instance", "instance is null.");

                if (lifetimeProvider == null)
                    throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");

                if (string.IsNullOrEmpty(errorString))
                    throw new ArgumentException("errorString is null or empty.", "errorString");

                instance.registerOptions = instance.ExecuteOnAllRegisterOptions(
                       registerOption => RegisterOptions.ToCustomLifetimeManager(registerOption, lifetimeProvider, errorString)
                    );

                return instance;
            }

            private IEnumerable<RegisterOptions> ExecuteOnAllRegisterOptions(Func<RegisterOptions, RegisterOptions> action)
            {
                List<RegisterOptions> newRegisterOptions = new List<RegisterOptions>();
                foreach (RegisterOptions registerOption in registerOptions)
                {
                    newRegisterOptions.Add(action(registerOption));
                }

                return newRegisterOptions;
            }
        }


        #endregion

        /// <summary>
        /// 创建一个子容器。子容器的父容器为该容器
        /// </summary>
        public TinyIoCContainer GetChildContainer()
        {
            return new TinyIoCContainer(this);
        }

        #region Register 

        #region 1. AutoRegister

        public void AutoRegister()
        {
            AutoRegisterInternal(
                inputAssemblies: new Assembly[] { GetType().Assembly },
                duplicateAction: DuplicateImplementationActions.RegisterSingle,
                registrationPredicate: null);
        }

        public void AutoRegister(Func<Type, bool> registrationPredicate)
        {
            AutoRegisterInternal(
                inputAssemblies: new Assembly[] { GetType().Assembly },
                duplicateAction: DuplicateImplementationActions.RegisterSingle,
                registrationPredicate: registrationPredicate);
        }

        public void AutoRegister(DuplicateImplementationActions duplicateAction)
        {
            AutoRegisterInternal(
                inputAssemblies: new Assembly[] { GetType().Assembly },
                duplicateAction: duplicateAction,
                registrationPredicate: null);
        }

        public void AutoRegister(DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate)
        {
            AutoRegisterInternal(
                inputAssemblies: new Assembly[] { GetType().Assembly },
                duplicateAction: duplicateAction,
                registrationPredicate);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies)
        {
            AutoRegisterInternal(
               inputAssemblies: assemblies,
               duplicateAction: DuplicateImplementationActions.RegisterSingle,
               registrationPredicate: null);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies, Func<Type, bool> registrationPredicate)
        {
            AutoRegisterInternal(
                inputAssemblies: assemblies,
                duplicateAction: DuplicateImplementationActions.RegisterSingle,
                registrationPredicate: registrationPredicate);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction)
        {
            AutoRegisterInternal(
                inputAssemblies: assemblies,
                duplicateAction: duplicateAction,
                registrationPredicate: null);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate)
        {
            AutoRegisterInternal(
                inputAssemblies: assemblies,
                duplicateAction: duplicateAction,
                registrationPredicate: registrationPredicate);
        }

        #endregion

        #region 2. Internal Register

        /// <summary>
        /// 注入容器   registerType 。Name 为  string.Empty ; Factory 为 默认的  GetDefaultObjectFactory(registerType, registerType));
        /// </summary>
        public RegisterOptions Register(Type registerType)
        {
            return RegisterInternal(
                registerType: registerType,
                name: string.Empty,
                factory: GetDefaultObjectFactory(registerType, registerType));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为  name ; Factory 为 默认的  GetDefaultObjectFactory(registerType, registerType));
        /// </summary>
        public RegisterOptions Register(Type registerType, string name)
        {
            return RegisterInternal(
                registerType: registerType,
                name: name,
                factory: GetDefaultObjectFactory(registerType, registerType));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为  string.Empty  ; Factory 为 默认的  GetDefaultObjectFactory(registerType, registerImplementation));
        /// </summary>
        public RegisterOptions Register(Type registerType, Type registerImplementation)
        {
            return RegisterInternal(
                registerType: registerType,
                name: string.Empty,
                factory: GetDefaultObjectFactory(registerType, registerImplementation));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为  name ; Factory 为 默认的  GetDefaultObjectFactory(registerType, registerImplementation));
        /// </summary>
        public RegisterOptions Register(Type registerType, Type registerImplementation, string name)
        {
            return RegisterInternal(
                registerType: registerType,
                name: name,
                factory: GetDefaultObjectFactory(registerType, registerImplementation));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为  string.Empty ; Factory 为   InstanceFactory(registerType, registerType, instance));
        /// 当通过 Factory获取对象时得到的是 instance
        /// </summary>
        public RegisterOptions Register(Type registerType, object instance)
        {
            return RegisterInternal(
                registerType: registerType,
                name: string.Empty,
                factory: new InstanceFactory(registerType, registerType, instance));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为  name ; Factory 为   InstanceFactory(registerType, registerType, instance));
        /// 当通过 Factory获取对象时得到的是 instance
        /// </summary>
        public RegisterOptions Register(Type registerType, object instance, string name)
        {
            return RegisterInternal(
                registerType: registerType,
                name: name,
                factory: new InstanceFactory(registerType, registerType, instance));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为  string.Empty ; Factory 为   InstanceFactory(registerType, registerImplementation, instance));
        /// 当通过 Factory获取对象时得到的是 instance
        /// </summary>
        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance)
        {
            return RegisterInternal(
                registerType: registerType,
                name: string.Empty,
                factory: new InstanceFactory(registerType, registerImplementation, instance));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为 name ; Factory 为   InstanceFactory(registerType, registerImplementation, instance));
        /// 当通过 Factory获取对象时得到的是 instance
        /// </summary>
        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name)
        {
            return RegisterInternal(
                registerType: registerType,
                name: name,
                factory: new InstanceFactory(registerType, registerImplementation, instance));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为 string.Empty ; Factory 为   DelegateFactory(registerType, factory));
        /// 当通过 Factory获取对象时得到的是 委托执行的结果
        /// </summary>
        public RegisterOptions Register(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory)
        {
            return RegisterInternal(
                registerType: registerType,
                name: string.Empty,
                factory: new DelegateFactory(registerType, factory));
        }

        /// <summary>
        /// 注入容器   registerType 。Name 为 name ; Factory 为   DelegateFactory(registerType, factory));
        /// 当通过 Factory获取对象时得到的是 委托执行的结果
        /// </summary>
        public RegisterOptions Register(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory, string name)
        {
            return RegisterInternal(
                registerType: registerType,
                name: name,
                factory: new DelegateFactory(registerType, factory));
        }

        #endregion

        #region 3. Register注入。 封装上面的方法，简化注入

        public RegisterOptions Register<RegisterType>()
            where RegisterType : class
        {
            return Register(typeof(RegisterType));
        }

        public RegisterOptions Register<RegisterType>(string name)
            where RegisterType : class
        {
            return Register(typeof(RegisterType), name);
        }

        public RegisterOptions Register<RegisterType, RegisterImplementation>()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return Register(typeof(RegisterType), typeof(RegisterImplementation));
        }

        public RegisterOptions Register<RegisterType, RegisterImplementation>(string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return Register(typeof(RegisterType), typeof(RegisterImplementation), name);
        }

        public RegisterOptions Register<RegisterType>(RegisterType instance)
           where RegisterType : class
        {
            return Register(typeof(RegisterType), instance);
        }

        public RegisterOptions Register<RegisterType>(RegisterType instance, string name)
            where RegisterType : class
        {
            return Register(typeof(RegisterType), instance, name);
        }

        public RegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return Register(typeof(RegisterType), typeof(RegisterImplementation), instance);
        }

        public RegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance, string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return Register(typeof(RegisterType), typeof(RegisterImplementation), instance, name);
        }

        public RegisterOptions Register<RegisterType>(Func<TinyIoCContainer, NamedParameterOverloads, RegisterType> factory)
            where RegisterType : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            return Register(typeof(RegisterType), (ioc, pars) => factory(ioc, pars));
        }

        public RegisterOptions Register<RegisterType>(Func<TinyIoCContainer, NamedParameterOverloads, RegisterType> factory, string name)
            where RegisterType : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            return Register(typeof(RegisterType), (ioc, pars) => factory(ioc, pars), name);
        }

        public MultiRegisterOptions RegisterMultiple<RegisterType>(IEnumerable<Type> implementationTypes)
        {
            return RegisterMultiple(typeof(RegisterType), implementationTypes);
        }

        public MultiRegisterOptions RegisterMultiple(Type registrationType, IEnumerable<Type> implementationTypes)
        {
            if (implementationTypes == null)
                throw new ArgumentNullException("types", "types is null.");

            foreach (Type type in implementationTypes)
                if (!registrationType.IsAssignableFrom(type))
                    throw new ArgumentException(string.Format("types: The type {0} is not assignable from {1}", registrationType.FullName, type.FullName));

            // 去重并抛出异常
            if (implementationTypes.Count() != implementationTypes.Distinct().Count())
            {
                IEnumerable<string> queryForDuplicatedTypes = from i in implementationTypes
                                                              group i by i
                                                              into j
                                                              where j.Count() > 1
                                                              select j.Key.FullName;

                string fullNamesOfDuplicatedTypes = string.Join(",\n", queryForDuplicatedTypes.ToArray());
                string multipleRegMessage = string.Format("types: The same implementation type cannot be specified multiple times for {0}\n\n{1}", registrationType.FullName, fullNamesOfDuplicatedTypes);
                throw new ArgumentException(multipleRegMessage);
            }

            List<RegisterOptions> registerOptions = new List<RegisterOptions>();
            foreach (Type type in implementationTypes)
            {
                registerOptions.Add(Register(
                    registerType: registrationType,
                    registerImplementation: type,
                    name: type.FullName));
            }

            return new MultiRegisterOptions(registerOptions);
        }

        #endregion

        /// <summary>
        /// 注入锁
        /// </summary>
        private readonly object AutoRegisterLock = new object();

        /// <summary>
        /// 注入 inputAssemblies 里面满足条件的Type 对象
        /// 根据 duplicateAction 判断是否可以注入实现接口或抽象类的集合。 RegisterMultiple 为可以注入集合
        /// 第一个实现抽象类或接口的元素 可以注入到容器
        /// </summary>
        private void AutoRegisterInternal(
            IEnumerable<Assembly> inputAssemblies,
            DuplicateImplementationActions duplicateAction,
            Func<Type, bool> registrationPredicate)
        {
            Type typeOfThis = GetType();
            lock (AutoRegisterLock)
            {
                List<Type> types = inputAssemblies.SelectMany(a => a.SafeGetTypes())
                    .Where(t => !IsIgnoredType(t, registrationPredicate))
                    .ToList();

                // concrete：具体的  注入具体的类型
                List<Type> concreteTypes = types
                    .Where(
                    type => type.IsClass &&
                    (type.IsAbstract == false) &&
                    (type != typeOfThis && (type.DeclaringType != typeOfThis) && (!type.IsGenericTypeDefinition())) && !type.IsNestedPrivate())
                    .ToList();

                foreach (Type type in concreteTypes)
                {
                    try
                    {
                        RegisterInternal(
                            registerType: type,
                            name: string.Empty,
                            factory: GetDefaultObjectFactory(registerType: type, registerImplementation: type));
                    }
                    catch (MethodAccessException) { }
                }

                // 查找 Type  集合里面的 抽象类 和 接口
                IEnumerable<Type> abstractInterfaceTypes = from type in types
                                                           where ((type.IsInterface || type.IsAbstract) && (type.DeclaringType != typeOfThis) && (!type.IsGenericTypeDefinition))
                                                           select type;

                foreach (Type type in abstractInterfaceTypes)
                {
                    Type localType = type;
                    IEnumerable<Type> implementations = from implementationType in concreteTypes
                                                        where localType.IsAssignableFrom(implementationType)
                                                        select implementationType;

                    // 判断是否存在大于1的实现 及 根据 duplicateAction 判断是否可以重复注入IOC，如果可以这注入实现的数组
                    if (implementations.Skip(1).Any())
                    {
                        if (duplicateAction == DuplicateImplementationActions.Fail)
                            throw new TinyIoCAutoRegistrationException(type, implementations);

                        if (duplicateAction == DuplicateImplementationActions.RegisterMultiple)
                        {
                            RegisterMultiple(
                                registrationType: type,
                                implementationTypes: implementations);
                        }
                    }

                    // 将第一个元素单独注入IOC，如果存在
                    Type firstImplementation = implementations.FirstOrDefault();
                    if (firstImplementation != null)
                    {
                        try
                        {
                            RegisterInternal(
                                registerType: type,
                                name: string.Empty,
                                factory: GetDefaultObjectFactory(registerType: type, registerImplementation: firstImplementation));
                        }
                        catch (MethodAccessException) { }
                    }
                }
            }
        }

        /// <summary>
        /// 命名空间以 Microsoft.  /  System. /  System / CR_ExtUnitTest /  mscorlib / CR_VSTest / DevExpress.CodeRush / xunit
        /// </summary> 
        private static readonly IReadOnlyList<Func<Assembly, bool>> ignoredAssemlies = new List<Func<Assembly, bool>>()
        {
            asm => asm.FullName.StartsWith("Microsoft.", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("System.", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("CR_ExtUnitTest", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("mscorlib,", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("CR_VSTest", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("DevExpress.CodeRush", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("xunit.", StringComparison.Ordinal),
        };

        /// <summary>
        /// 忽略注入的类型 委托
        /// 1. 命名空间以 System. 开始
        /// 2. 命名空间以 Microsoft. 开始
        /// 3. 类型为泛型
        /// 4. public 构造函数的个数为0 
        /// 5. 为接口或抽象类型
        /// </summary>
        private static readonly IReadOnlyList<Func<Type, bool>> ignoreChecks = new List<Func<Type, bool>>()
        {
            t => t.FullName.StartsWith("System.", StringComparison.Ordinal),
            t => t.FullName.StartsWith("Microsoft.", StringComparison.Ordinal),
            t => t.IsPrimitive,
            t => t.IsGenericTypeDefinition,
            t => (t.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length == 0) && !(t.IsInterface || t.IsAbstract),
        };

        /// <summary>
        /// 是否为可以忽略的Type 类型。返回 true表示应该忽略注入到容器
        /// </summary>
        private bool IsIgnoredType(Type type, Func<Type, bool> registrationPredicate)
        {
            if (ignoreChecks.Any(c => c(type)))
                return true;

            return registrationPredicate != null && !registrationPredicate(type);
        }

        private void RegisterDefaultTypes()
        {
            Register<TinyIoCContainer>(this);
        }

        #endregion

        #region Unregistration 从字典中移除类型。名字很重要在TypeRegistration中有一起生成Hashcode

        public bool Unregister<RegisterType>()
        {
            return Unregister(typeof(RegisterType), string.Empty);
        }

        public bool Unregister<RegisterType>(string name)
        {
            return Unregister(typeof(RegisterType), name);
        }

        public bool Unregister(Type registerType)
        {
            return Unregister(registerType, string.Empty);
        }

        /// <summary>
        /// TypeRegistration 重写了 Equal 函数，且定义了HashCode方法，则移除字典可以通过新建类型
        /// </summary>
        public bool Unregister(Type registerType, string name)
        {
            TypeRegistration typeRegistration = new TypeRegistration(registerType, name);
            return RemoveRegistration(typeRegistration);
        }

        #endregion

        #region Resolve 从IOC容器中解析出 object 对象

        /// <summary>
        /// 从容器中解析  Type  类型实例。
        /// 传入的Name     为  string.Empty 
        /// 传入的参数     为  NamedParameterOverloads.Default
        /// 传入的options  为  ResolveOptions.Default
        /// </summary>
        public object Resolve(Type resolveType)
        {
            return ResolveInternal(
                registration: new TypeRegistration(resolveType),
                parameters: NamedParameterOverloads.Default,
                options: ResolveOptions.Default);
        }

        /// <summary>
        /// 从容器中解析  Type  类型实例。
        /// 传入的Name     为  string.Empty 
        /// 传入的参数     为  NamedParameterOverloads.Default
        /// 传入的options  为  options
        /// </summary>
        public object Resolve(Type resolveType, ResolveOptions options)
        {
            return ResolveInternal(
                registration: new TypeRegistration(resolveType),
                parameters: NamedParameterOverloads.Default,
                options: options);
        }

        /// <summary>
        /// 从容器中解析  Type  类型实例。
        /// 传入的Name     为  name 
        /// 传入的参数     为  NamedParameterOverloads.Default
        /// 传入的options  为  ResolveOptions.Default
        /// </summary>
        public object Resolve(Type resolveType, string name)
        {
            return ResolveInternal(
                registration: new TypeRegistration(resolveType, name),
                parameters: NamedParameterOverloads.Default,
                options: ResolveOptions.Default);
        }

        /// <summary>
        /// 从容器中解析  Type  类型实例。
        /// 传入的Name     为  name 
        /// 传入的参数     为  NamedParameterOverloads.Default
        /// 传入的options  为  options
        /// </summary>
        public object Resolve(Type resolveType, string name, ResolveOptions options)
        {
            return ResolveInternal(
                registration: new TypeRegistration(resolveType, name),
                parameters: NamedParameterOverloads.Default,
                options: options);
        }

        /// <summary>
        /// 从容器中解析  Type  类型实例。
        /// 传入的Name     为  string.Empty 
        /// 传入的参数     为  parameters
        /// 传入的options  为  ResolveOptions.Default
        /// </summary>
        public object Resolve(Type resolveType, NamedParameterOverloads parameters)
        {
            return ResolveInternal(
                registration: new TypeRegistration(resolveType),
                parameters: parameters,
                options: ResolveOptions.Default);
        }

        /// <summary>
        /// 从容器中解析  Type  类型实例。
        /// 传入的Name     为  string.Empty 
        /// 传入的参数     为  parameters
        /// 传入的options  为  ResolveOptions.Default
        /// </summary>
        public object Resolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return ResolveInternal(
                registration: new TypeRegistration(resolveType),
                parameters: parameters,
                options: options);
        }

        public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters)
        {
            return ResolveInternal(
                registration: new TypeRegistration(resolveType, name),
                parameters: parameters,
                options: ResolveOptions.Default);
        }

        public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return ResolveInternal(
                registration: new TypeRegistration(resolveType, name),
                parameters: parameters,
                options: options);
        }

        public ResolveType Resolve<ResolveType>()
            where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType));
        }

        public ResolveType Resolve<ResolveType>(ResolveOptions options)
            where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), options);
        }

        public ResolveType Resolve<ResolveType>(string name)
            where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), name);
        }

        public ResolveType Resolve<ResolveType>(string name, ResolveOptions options)
            where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), name, options);
        }

        public ResolveType Resolve<ResolveType>(NamedParameterOverloads parameters)
            where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), parameters);
        }

        public ResolveType Resolve<ResolveType>(NamedParameterOverloads parameters, ResolveOptions options)
            where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), parameters, options);
        }

        public ResolveType Resolve<ResolveType>(string name, NamedParameterOverloads parameters)
            where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), name, parameters);
        }


        public ResolveType Resolve<ResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options)
            where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), name, parameters, options);
        }

        /// <summary>
        /// 通过 TypeRegistration 为 Key 在字典中找到创建者  ObjectFactoryBase ，通过传递参数给 ObjectFactoryBase 来创建对象实例
        /// 1. 可以从子容器中创建
        /// 2. 可以构建Name为空的 TypeRegistration 来寻找  ObjectFactoryBase 来创建
        /// </summary>
        private object ResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options)
        {
            ObjectFactoryBase factory;
            if (RegisteredTypes.TryGetValue(registration, out factory))
            {
                try
                {
                    return factory.GetObject(
                       requestedType: registration.Type,
                       container: this,
                       parameters: parameters,
                       options: options);
                }
                catch (TinyIoCResolutionException) { throw; }
                catch (Exception ex) { throw new TinyIoCResolutionException(registration.Type, ex); }
            }

            ObjectFactoryBase bubbledObjectFactory = GetParentObjectFactory(registration);
            if (bubbledObjectFactory != null)
            {
                try
                {
                    return bubbledObjectFactory.GetObject(
                        requestedType: registration.Type,
                        container: this,
                        parameters: parameters,
                        options: options);
                }
                catch (TinyIoCResolutionException) { throw; }
                catch (Exception ex) { throw new TinyIoCResolutionException(registration.Type, ex); }
            }

            if (!string.IsNullOrEmpty(registration.Name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
                throw new TinyIoCResolutionException(registration.Type);

            if (!string.IsNullOrEmpty(registration.Name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
            {
                if (RegisteredTypes.TryGetValue(new TypeRegistration(registration.Type, string.Empty), out factory))
                {
                    try
                    {
                        return factory.GetObject(
                             requestedType: registration.Type,
                             container: this,
                             parameters: parameters,
                             options: options);
                    }
                    catch (TinyIoCResolutionException) { throw; }
                    catch (Exception ex) { throw new TinyIoCResolutionException(registration.Type, ex); }
                }
            }

            if (IsIEnumerableRequest(registration.Type))
                return GetIEnumerableRequest(registration.Type);

            if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) ||
                (registration.Type.IsGenericType && options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly))
            {
                if (!registration.Type.IsAbstract && !registration.Type.IsInterface)
                    return ConstructType(null, registration.Type, parameters, options);
            }

            throw new TinyIoCResolutionException(registration.Type);
        }

        private ObjectFactoryBase GetParentObjectFactory(TypeRegistration registration)
        {
            if (parentContainer == null)
                return null;

            ObjectFactoryBase factory;
            if (parentContainer.RegisteredTypes.TryGetValue(registration, out factory))
            {
                return factory.GetFactoryForChildContainer(registration.Type, parentContainer, this);
            }

            return parentContainer.GetParentObjectFactory(registration);
        }

        #endregion


        #region CanResolve / TryResolve 判断是否可以从容器中解析Type 

        public bool CanResolve(Type resolveType)
        {
            return CanResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, ResolveOptions.Default);
        }

        private bool CanResolve(Type resolveType, string name)
        {
            return CanResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default, ResolveOptions.Default);
        }

        public bool CanResolve(Type resolveType, ResolveOptions options)
        {
            return CanResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, options);
        }

        public bool CanResolve(Type resolveType, string name, ResolveOptions options)
        {
            return CanResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default, options);
        }

        public bool CanResolve(Type resolveType, NamedParameterOverloads parameters)
        {
            return CanResolveInternal(new TypeRegistration(resolveType), parameters, ResolveOptions.Default);
        }

        public bool CanResolve(Type resolveType, string name, NamedParameterOverloads parameters)
        {
            return CanResolveInternal(new TypeRegistration(resolveType, name), parameters, ResolveOptions.Default);
        }

        public bool CanResolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return CanResolveInternal(new TypeRegistration(resolveType), parameters, options);
        }

        public bool CanResolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return CanResolveInternal(new TypeRegistration(resolveType, name), parameters, options);
        }

        public bool CanResolve<ResolveType>()
            where ResolveType : class
        {
            return CanResolve(typeof(ResolveType));
        }

        public bool CanResolve<ResolveType>(string name)
            where ResolveType : class
        {
            return CanResolve(typeof(ResolveType), name);
        }

        public bool CanResolve<ResolveType>(ResolveOptions options)
            where ResolveType : class
        {
            return CanResolve(typeof(ResolveType), options);
        }

        public bool CanResolve<ResolveType>(string name, ResolveOptions options)
            where ResolveType : class
        {
            return CanResolve(typeof(ResolveType), name, options);
        }

        public bool CanResolve<ResolveType>(NamedParameterOverloads parameters)
            where ResolveType : class
        {
            return CanResolve(typeof(ResolveType), parameters);
        }

        public bool CanResolve<ResolveType>(string name, NamedParameterOverloads parameters)
            where ResolveType : class
        {
            return CanResolve(typeof(ResolveType), name, parameters);
        }

        public bool CanResolve<ResolveType>(NamedParameterOverloads parameters, ResolveOptions options)
            where ResolveType : class
        {
            return CanResolve(typeof(ResolveType), parameters, options);
        }

        public bool CanResolve<ResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options)
            where ResolveType : class
        {
            return CanResolve(typeof(ResolveType), name, parameters, options);
        }

        /// <summary>
        /// 查看类型Type中是否存在满足条件的构造函数。包括参数也要匹配
        /// </summary>
        private bool CanResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            Type checkType = registration.Type;
            string name = registration.Name;

            ObjectFactoryBase factory;
            if (RegisteredTypes.TryGetValue(new TypeRegistration(checkType, name), out factory))
            {
                if (factory.AssumeConstruction)
                    return true;

                if (factory.Constructor == null)
                    return GetBestConstructor(factory.CreatesType, parameters, options) != null;
                else
                    return CanConstruct(factory.Constructor, parameters, options);
            }

            if (!string.IsNullOrEmpty(name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
                return (parentContainer != null) ? parentContainer.CanResolveInternal(registration, parameters, options) : false;

            if (!string.IsNullOrEmpty(name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
            {
                if (RegisteredTypes.TryGetValue(new TypeRegistration(checkType), out factory))
                {
                    if (factory.AssumeConstruction)
                        return true;

                    return GetBestConstructor(factory.CreatesType, parameters, options) != null;
                }
            }

            if (IsAutomaticLazyFactoryRequest(checkType))
                return true;

            if (IsIEnumerableRequest(registration.Type))
                return true;

            if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) || (checkType.IsGenericType && options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly))
                return (GetBestConstructor(checkType, parameters, options) != null) ? true : (parentContainer != null) ? parentContainer.CanResolveInternal(registration, parameters, options) : false;

            if (parentContainer != null)
                return parentContainer.CanResolveInternal(registration, parameters, options);

            return false;
        }

        /// <summary>
        /// 寻找最适合的匹配的构造函数
        /// </summary>
        private ConstructorInfo GetBestConstructor(Type type, NamedParameterOverloads parameters, ResolveOptions options)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            if (type.IsValueType)
                return null;
            // 从字典缓存中查找type对应的构造函数【构造函数必须有TinyIoCConstructorAttribute特性修饰】
            IEnumerable<ConstructorInfo> ctors = TinyIoCReflectionCache.GetUsableConstructors(type);
            foreach (ConstructorInfo ctor in ctors)
            {
                if (CanConstruct(ctor, parameters, options))
                    return ctor;
            }

            return null;
        }

        /// <summary>
        /// 判断构造函数是否匹配 
        /// </summary>
        private bool CanConstruct(ConstructorInfo ctor, NamedParameterOverloads parameters, ResolveOptions options)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            foreach (ParameterInfo parameter in ctor.GetParameters())
            {
                if (string.IsNullOrEmpty(parameter.Name))
                    return false;

                bool isParameterOverload = parameters.ContainsKey(parameter.Name);
                if (parameter.ParameterType.IsPrimitive && !isParameterOverload)
                    return false;

                if (!isParameterOverload && !CanResolveInternal(new TypeRegistration(parameter.ParameterType), NamedParameterOverloads.Default, options))
                    return false;
            }

            return true;
        }

        public bool TryResolve(Type resolveType, out object resolvedType)
        {
            try
            {
                resolvedType = Resolve(resolveType);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = null;
                return false;
            }
        }

        public bool TryResolve(Type resolveType, ResolveOptions options, out object resolvedType)
        {
            try
            {
                resolvedType = Resolve(resolveType, options);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = null;
                return false;
            }
        }

        public bool TryResolve(Type resolveType, string name, out object resolvedType)
        {
            try
            {
                resolvedType = Resolve(resolveType, name);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = null;
                return false;
            }
        }

        public bool TryResolve(Type resolveType, string name, ResolveOptions options, out object resolvedType)
        {
            try
            {
                resolvedType = Resolve(resolveType, name, options);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = null;
                return false;
            }
        }

        public bool TryResolve(Type resolveType, NamedParameterOverloads parameters, out object resolvedType)
        {
            try
            {
                resolvedType = Resolve(resolveType, parameters);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = null;
                return false;
            }
        }

        public bool TryResolve(Type resolveType, string name, NamedParameterOverloads parameters, out object resolvedType)
        {
            try
            {
                resolvedType = Resolve(resolveType, name, parameters);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = null;
                return false;
            }
        }

        public bool TryResolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options, out object resolvedType)
        {
            try
            {
                resolvedType = Resolve(resolveType, parameters, options);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = null;
                return false;
            }
        }

        public bool TryResolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options, out object resolvedType)
        {
            try
            {
                resolvedType = Resolve(resolveType, name, parameters, options);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = null;
                return false;
            }
        }

        public bool TryResolve<ResolveType>(out ResolveType resolvedType)
            where ResolveType : class
        {
            try
            {
                resolvedType = Resolve<ResolveType>();
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        public bool TryResolve<ResolveType>(ResolveOptions options, out ResolveType resolvedType)
            where ResolveType : class
        {
            try
            {
                resolvedType = Resolve<ResolveType>(options);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        public bool TryResolve<ResolveType>(string name, out ResolveType resolvedType)
            where ResolveType : class
        {
            try
            {
                resolvedType = Resolve<ResolveType>(name);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        public bool TryResolve<ResolveType>(string name, ResolveOptions options, out ResolveType resolvedType)
            where ResolveType : class
        {
            try
            {
                resolvedType = Resolve<ResolveType>(name, options);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        public bool TryResolve<ResolveType>(NamedParameterOverloads parameters, out ResolveType resolvedType)
            where ResolveType : class
        {
            try
            {
                resolvedType = Resolve<ResolveType>(parameters);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        public bool TryResolve<ResolveType>(string name, NamedParameterOverloads parameters, out ResolveType resolvedType)
            where ResolveType : class
        {
            try
            {
                resolvedType = Resolve<ResolveType>(name, parameters);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        public bool TryResolve<ResolveType>(NamedParameterOverloads parameters, ResolveOptions options, out ResolveType resolvedType)
            where ResolveType : class
        {
            try
            {
                resolvedType = Resolve<ResolveType>(parameters, options);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        public bool TryResolve<ResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options, out ResolveType resolvedType)
            where ResolveType : class
        {
            try
            {
                resolvedType = Resolve<ResolveType>(name, parameters, options);
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        public IEnumerable<object> ResolveAll(Type resolveType, bool includeUnnamed)
        {
            return ResolveAllInternal(resolveType, includeUnnamed);
        }

        public IEnumerable<object> ResolveAll(Type resolveType)
        {
            return ResolveAll(resolveType, true);
        }

        public IEnumerable<ResolveType> ResolveAll<ResolveType>(bool includeUnnamed)
            where ResolveType : class
        {
            return this.ResolveAll(typeof(ResolveType), includeUnnamed).Cast<ResolveType>();
        }

        public IEnumerable<ResolveType> ResolveAll<ResolveType>()
            where ResolveType : class
        {
            return ResolveAll<ResolveType>(true);
        }

        public void BuildUp(object input)
        {
            BuildUpInternal(input, ResolveOptions.Default);
        }

        public void BuildUp(object input, ResolveOptions resolveOptions)
        {
            BuildUpInternal(input, resolveOptions);
        }

        #endregion


        #region Object Factories

        public interface ITinyIoCObjectLifetimeProvider
        {
            object GetObject();
            void SetObject(object value);
            void ReleaseObject();
        }

        /// <summary>
        /// 此抽象类存在本身类型 ObjectFactoryBase的变量:
        /// 
        /// SingletonVariant
        /// MultiInstanceVariant
        /// StrongReferenceVariant
        /// WeakReferenceVariant
        /// GetCustomObjectLifetimeVariant() 函数 实现自定义返回 ObjectFactoryBase
        /// 
        /// 此抽象类包含抽象函数
        /// public abstract object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        /// 
        /// </summary>
        private abstract class ObjectFactoryBase
        {
            public virtual bool AssumeConstruction
            {
                get { return false; }
            }

            public abstract Type CreatesType { get; }

            public ConstructorInfo Constructor { get; protected set; }

            public abstract object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options);

            public virtual ObjectFactoryBase SingletonVariant
            {
                get { throw new TinyIoCRegistrationException(this.GetType(), "singleton"); }
            }

            public virtual ObjectFactoryBase MultiInstanceVariant
            {
                get { throw new TinyIoCRegistrationException(this.GetType(), "multi-instance"); }
            }

            public virtual ObjectFactoryBase StrongReferenceVariant
            {
                get { throw new TinyIoCRegistrationException(this.GetType(), "strong reference"); }
            }

            public virtual ObjectFactoryBase WeakReferenceVariant
            {
                get { throw new TinyIoCRegistrationException(this.GetType(), "weak reference"); }
            }

            public virtual ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                throw new TinyIoCRegistrationException(this.GetType(), errorString);
            }

            public virtual void SetConstructor(ConstructorInfo constructor)
            {
                Constructor = constructor;
            }

            public virtual ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent, TinyIoCContainer child)
            {
                return this;
            }
        }

        private class MultiInstanceFactory : ObjectFactoryBase
        {
            private readonly Type registerType;
            private readonly Type registerImplementation;
            public override Type CreatesType { get { return this.registerImplementation; } }

            public MultiInstanceFactory(Type registerType, Type registerImplementation)
            {
                if (registerImplementation.IsAbstract || registerImplementation.IsInterface)
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "MultiInstanceFactory");
                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "MultiInstanceFactory");

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                try
                {
                    return container.ConstructType(requestedType, this.registerImplementation, Constructor, parameters, options);
                }
                catch (TinyIoCResolutionException ex)
                {
                    throw new TinyIoCResolutionException(this.registerType, ex);
                }
            }

            public override ObjectFactoryBase SingletonVariant
            {
                get { return new SingletonFactory(this.registerType, this.registerImplementation); }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                return new CustomObjectLifetimeFactory(this.registerType, this.registerImplementation, lifetimeProvider, errorString);
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get { return this; }
            }
        }

        private class DelegateFactory : ObjectFactoryBase
        {
            private readonly Type registerType;

            private Func<TinyIoCContainer, NamedParameterOverloads, object> factory;

            public override bool AssumeConstruction { get { return true; } }

            public override Type CreatesType { get { return this.registerType; } }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                try
                {
                    return factory.Invoke(container, parameters);
                }
                catch (Exception ex)
                {
                    throw new TinyIoCResolutionException(this.registerType, ex);
                }
            }

            public DelegateFactory(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory)
            {
                if (factory == null)
                    throw new ArgumentNullException("factory");

                this.factory = factory;
                this.registerType = registerType;
            }

            public override ObjectFactoryBase SingletonVariant
            {
                get { return new DelegateSingletonFactory(registerType, factory); }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get { return new WeakDelegateFactory(this.registerType, factory); }
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get { return this; }
            }

            public override void SetConstructor(ConstructorInfo constructor)
            {
                throw new TinyIoCConstructorResolutionException("Constructor selection is not possible for delegate factory registrations");
            }
        }

        private class WeakDelegateFactory : ObjectFactoryBase
        {
            private readonly Type registerType;

            private WeakReference factory;

            public override bool AssumeConstruction { get { return true; } }

            public override Type CreatesType { get { return this.registerType; } }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                Func<TinyIoCContainer, NamedParameterOverloads, object> funFactory = this.factory.Target as Func<TinyIoCContainer, NamedParameterOverloads, object>;

                if (funFactory == null)
                    throw new TinyIoCWeakReferenceException(this.registerType);

                try
                {
                    return funFactory.Invoke(container, parameters);
                }
                catch (Exception ex)
                {
                    throw new TinyIoCResolutionException(this.registerType, ex);
                }
            }

            public WeakDelegateFactory(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory)
            {
                if (factory == null)
                    throw new ArgumentNullException("factory");

                this.factory = new WeakReference(factory);
                this.registerType = registerType;
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    Func<TinyIoCContainer, NamedParameterOverloads, object> funFactory = factory.Target as Func<TinyIoCContainer, NamedParameterOverloads, object>;

                    if (funFactory == null)
                        throw new TinyIoCWeakReferenceException(this.registerType);

                    return new DelegateFactory(this.registerType, funFactory);
                }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get { return this; }
            }

            public override void SetConstructor(ConstructorInfo constructor)
            {
                throw new TinyIoCConstructorResolutionException("Constructor selection is not possible for delegate factory registrations");
            }
        }

        private class InstanceFactory : ObjectFactoryBase, IDisposable
        {
            private readonly Type registerType;
            private readonly Type registerImplementation;
            private object instance;

            public override bool AssumeConstruction { get { return true; } }

            public InstanceFactory(Type registerType, Type registerImplementation, object instance)
            {
                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "InstanceFactory");

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
                this.instance = instance;
            }

            public override Type CreatesType
            {
                get { return this.registerImplementation; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                return instance;
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get { return new MultiInstanceFactory(this.registerType, this.registerImplementation); }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get { return new WeakInstanceFactory(this.registerType, this.registerImplementation, this.instance); }
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get { return this; }
            }

            public override void SetConstructor(ConstructorInfo constructor)
            {
                throw new TinyIoCConstructorResolutionException("Constructor selection is not possible for instance factory registrations");
            }

            public void Dispose()
            {
                IDisposable disposable = instance as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        private class WeakInstanceFactory : ObjectFactoryBase, IDisposable
        {
            private readonly Type registerType;
            private readonly Type registerImplementation;
            private readonly WeakReference instance;

            public WeakInstanceFactory(Type registerType, Type registerImplementation, object instance)
            {
                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "WeakInstanceFactory");

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
                this.instance = new WeakReference(instance);
            }

            public override Type CreatesType
            {
                get { return this.registerImplementation; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                object instance = this.instance.Target;
                if (instance == null)
                    throw new TinyIoCWeakReferenceException(this.registerType);

                return instance;
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get { return new MultiInstanceFactory(this.registerType, this.registerImplementation); }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get { return this; }
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    object instance = this.instance.Target;
                    if (instance == null)
                        throw new TinyIoCWeakReferenceException(this.registerType);

                    return new InstanceFactory(this.registerType, this.registerImplementation, instance);
                }
            }

            public override void SetConstructor(ConstructorInfo constructor)
            {
                throw new TinyIoCConstructorResolutionException("Constructor selection is not possible for instance factory registrations");
            }

            public void Dispose()
            {
                IDisposable disposable = instance.Target as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        private class SingletonFactory : ObjectFactoryBase, IDisposable
        {
            private readonly Type registerType;
            private readonly Type registerImplementation;
            private readonly object SingletonLock = new object();
            private object current;

            public SingletonFactory(Type registerType, Type registerImplementation)
            {
                if (registerImplementation.IsAbstract || registerImplementation.IsInterface)
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");

                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
            }

            public override bool AssumeConstruction => current != null;

            public override Type CreatesType
            {
                get { return this.registerImplementation; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                if (parameters.Count != 0)
                    throw new ArgumentException("Cannot specify parameters for singleton types");

                if (instanceContainer != null) return instanceContainer;

                lock (SingletonLock)
                    if (current == null)
                        current = container.ConstructType(requestedType, registerImplementation, Constructor, options);

                return current;
            }

            public override ObjectFactoryBase SingletonVariant
            {
                get { return this; }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                return new CustomObjectLifetimeFactory(registerType, registerImplementation, lifetimeProvider, errorString);
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get { return new MultiInstanceFactory(registerType, registerImplementation); }
            }

            public override ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent, TinyIoCContainer child)
            {
                GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
                return this;
            }

            public void Dispose()
            {
                if (current == null)
                    return;

                IDisposable disposable = current as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        private class DelegateSingletonFactory : ObjectFactoryBase, IDisposable
        {
            private readonly Func<TinyIoCContainer, NamedParameterOverloads, object> factory;
            private readonly object singletonLock = new object();
            private object instance;

            public DelegateSingletonFactory(Type creatingType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory)
            {
                this.factory = factory;
                CreatesType = creatingType;
            }

            public override Type CreatesType { get; }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters,
                ResolveOptions options)
            {
                if (instance == null)
                {
                    lock (singletonLock)
                    {
                        if (instance == null)
                            instance = factory(container, parameters);
                    }
                }

                return instance;
            }

            public void Dispose()
            {
                if (instance is IDisposable disp)
                {
                    disp.Dispose();
                    instance = null;
                }
            }
        }

        private class CustomObjectLifetimeFactory : ObjectFactoryBase, IDisposable
        {
            private readonly object SingletonLock = new object();
            private readonly Type registerType;
            private readonly Type registerImplementation;
            private readonly ITinyIoCObjectLifetimeProvider lifetimeProvider;

            public CustomObjectLifetimeFactory(Type registerType, Type registerImplementation, ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorMessage)
            {
                if (lifetimeProvider == null)
                    throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");

                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");

                if (registerImplementation.IsAbstract || registerImplementation.IsInterface)
                    throw new TinyIoCRegistrationTypeException(registerImplementation, errorMessage);

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
                this.lifetimeProvider = lifetimeProvider;
            }

            public override Type CreatesType
            {
                get { return registerImplementation; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                object current;
                lock (SingletonLock)
                {
                    current = lifetimeProvider.GetObject();
                    if (current == null)
                    {
                        current = container.ConstructType(requestedType, this.registerImplementation, Constructor, options);
                        lifetimeProvider.SetObject(current);
                    }
                }

                return current;
            }

            public override ObjectFactoryBase SingletonVariant
            {
                get
                {
                    lifetimeProvider.ReleaseObject();
                    return new SingletonFactory(this.registerType, this.registerImplementation);
                }
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get
                {
                    lifetimeProvider.ReleaseObject();
                    return new MultiInstanceFactory(this.registerType, this.registerImplementation);
                }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                lifetimeProvider.ReleaseObject();
                return new CustomObjectLifetimeFactory(this.registerType, this.registerImplementation, lifetimeProvider, errorString);
            }

            public override ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent, TinyIoCContainer child)
            {
                GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
                return this;
            }

            public void Dispose()
            {
                lifetimeProvider.ReleaseObject();
            }
        }

        #endregion

        /// <summary>
        /// 类型 Type 和 名称Name 
        /// 
        /// 的数据结构封装。 重写了 Equql 函数，且定义了HashCode方法，则移除字典可以通过新建类型
        /// </summary>
        public sealed class TypeRegistration
        {
            private int hashCode;
            public Type Type { get; private set; }
            public string Name { get; private set; }

            /// <summary>
            /// 类型 Type 和 名称Name 
            /// 
            /// 的数据结构封装。 Name 为 string.Empty。 重写了 Equql 函数，且定义了HashCode方法，则移除字典可以通过新建类型
            /// </summary>
            public TypeRegistration(Type type)
                : this(type, string.Empty)
            {
            }

            /// <summary>
            /// 类型 Type 和 名称Name
            /// 
            /// 的数据结构封装。 Name 为 name。 重写了 Equql 函数，且定义了HashCode方法，则移除字典可以通过新建类型
            /// </summary>
            public TypeRegistration(Type type, string name)
            {
                Type = type;
                Name = name;
                hashCode = string.Concat(Type.FullName, "|", Name).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                TypeRegistration typeRegistration = obj as TypeRegistration;
                if (typeRegistration == null)
                    return false;

                if (Type != typeRegistration.Type)
                    return false;

                if (string.Compare(Name, typeRegistration.Name, StringComparison.Ordinal) != 0)
                    return false;

                return true;
            }

            public override int GetHashCode()
            {
                return hashCode;
            }
        }

        #region Internal Methods

        private bool IsIgnoredAssembly(Assembly assembly)
        {
            for (int i = 0; i < ignoredAssemlies.Count; i++)
            {
                if (ignoredAssemlies[i].Invoke(assembly))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 从 SafeDictionary【TypeRegistration, ObjectFactoryBase】 RegisteredTypes
        /// 字典属性中根据 Key 获取 ObjectFactoryBase
        /// </summary>
        private ObjectFactoryBase GetCurrentFactory(TypeRegistration registration)
        {
            ObjectFactoryBase current = null;
            RegisteredTypes.TryGetValue(registration, out current);
            return current;
        }

        /// <summary>
        /// 将 Type 和 ObjectFactoryBase 注入到this的容器的 SafeDictionary【TypeRegistration, ObjectFactoryBase】 字典中
        /// </summary>
        private RegisterOptions RegisterInternal(Type registerType, string name, ObjectFactoryBase factory)
        {
            TypeRegistration typeRegistration = new TypeRegistration(registerType, name);
            return AddUpdateRegistration(typeRegistration, factory);
        }

        /// <summary>
        /// 修改 SafeDictionary【TypeRegistration, ObjectFactoryBase】 RegisteredTypes 字典中
        /// Key 的 值为 对应的 ObjectFactoryBase
        /// 并返回 封装 IOCContainer 和 Key的数据结构 RegisterOptions
        /// </summary>
        private RegisterOptions AddUpdateRegistration(TypeRegistration typeRegistration, ObjectFactoryBase factory)
        {
            RegisteredTypes[typeRegistration] = factory;
            return new RegisterOptions(this, typeRegistration);
        }

        /// <summary>
        /// 移除 SafeDictionary【TypeRegistration, ObjectFactoryBase】 RegisteredTypes 字典中 Key对应的项
        /// </summary>
        private bool RemoveRegistration(TypeRegistration typeRegistration)
        {
            return RegisteredTypes.Remove(typeRegistration);
        }

        /// <summary>
        /// 创建封装  Type registerType 和 Type registerImplementation 的对象
        /// 返回类型为 ObjectFactoryBase
        /// </summary>
        private ObjectFactoryBase GetDefaultObjectFactory(Type registerType, Type registerImplementation)
        {
            if (registerType.IsInterface || registerType.IsAbstract)
                return new SingletonFactory(registerType, registerImplementation);

            return new MultiInstanceFactory(registerType, registerImplementation);
        }

        private bool IsIEnumerableRequest(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericType = type.GetGenericTypeDefinition();

            if (genericType == typeof(IEnumerable<>))
                return true;

            return false;
        }

        private readonly SafeDictionary<Type, object> _LazyAutomaticFactories = new SafeDictionary<Type, object>();
        private bool IsAutomaticLazyFactoryRequest(Type type)
        {
            if (_LazyAutomaticFactories.ContainsKey(type))
                return true;

            if (!type.IsGenericType)
                return false;

            Type genericType = type.GetGenericTypeDefinition();

            if (genericType == typeof(Func<>))
                return true;

            Type[] genericArguments = null;
            if ((genericType == typeof(Func<,>) && (genericArguments = type.GetGenericArguments())[0] == typeof(string)))
                return true;

            if ((genericType == typeof(Func<,,>) && (genericArguments = genericArguments ?? type.GetGenericArguments())[0] == typeof(string) && (genericArguments = genericArguments ?? type.GetGenericArguments())[1] == typeof(IDictionary<String, object>)))
                return true;

            return false;
        }

        private object GetIEnumerableRequest(Type type)
        {
            MethodInfo genericResolveAllMethod = this.GetType().GetGenericMethod(BindingFlags.Public | BindingFlags.Instance, "ResolveAll", type.GetGenericArguments(), new[] { typeof(bool) });
            return genericResolveAllMethod.Invoke(this, new object[] { false });
        }

        private object ConstructType(Type requestedType, Type implementationType, ResolveOptions options)
        {
            return ConstructType(requestedType, implementationType, null, NamedParameterOverloads.Default, options);
        }

        private object ConstructType(Type requestedType, Type implementationType, ConstructorInfo constructor, ResolveOptions options)
        {
            return ConstructType(requestedType, implementationType, constructor, NamedParameterOverloads.Default, options);
        }

        private object ConstructType(Type requestedType, Type implementationType, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return ConstructType(requestedType, implementationType, null, parameters, options);
        }

        private object ConstructType(Type requestedType, Type implementationType, ConstructorInfo constructor, NamedParameterOverloads parameters, ResolveOptions options)
        {
            Type typeToConstruct = implementationType;
            if (constructor == null)
            {
                constructor = GetBestConstructor(typeToConstruct, parameters, options) ?? TinyIoCReflectionCache.GetUsableConstructors(typeToConstruct).LastOrDefault();
            }

            if (constructor == null)
                throw new TinyIoCResolutionException(typeToConstruct);

            ParameterInfo[] ctorParams = constructor.GetParameters();
            object[] args = new object[ctorParams.Length];
            for (int parameterIndex = 0; parameterIndex < ctorParams.Length; parameterIndex++)
            {
                ParameterInfo currentParam = ctorParams[parameterIndex];
                try
                {
                    args[parameterIndex] = parameters.ContainsKey(currentParam.Name) ?
                                                                                        parameters[currentParam.Name] :
                                                                                        ResolveInternal(
                                                                                                new TypeRegistration(currentParam.ParameterType),
                                                                                                NamedParameterOverloads.Default,
                                                                                                options);
                }
                catch (TinyIoCResolutionException ex)
                {
                    throw new TinyIoCResolutionException(typeToConstruct, ex);
                }
                catch (Exception ex)
                {
                    throw new TinyIoCResolutionException(typeToConstruct, ex);
                }
            }

            try
            {
                return constructor.Invoke(args);
            }
            catch (Exception ex)
            {
                throw new TinyIoCResolutionException(typeToConstruct, ex);
            }
        }

        private void BuildUpInternal(object input, ResolveOptions resolveOptions)
        {
            IEnumerable<PropertyInfo> properties = from property in input.GetType().GetProperties()
                                                   where (property.GetGetMethod() != null) && (property.GetSetMethod() != null) && !property.PropertyType.IsValueType()
                                                   select property;

            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(input, null) == null)
                {
                    try
                    {
                        property.SetValue(input, ResolveInternal(new TypeRegistration(property.PropertyType), NamedParameterOverloads.Default, resolveOptions), null);
                    }
                    catch (TinyIoCResolutionException)
                    {
                    }
                }
            }
        }

        private IEnumerable<TypeRegistration> GetParentRegistrationsForType(Type resolveType)
        {
            if (parentContainer == null)
                return new TypeRegistration[] { };

            IEnumerable<TypeRegistration> registrations = parentContainer.RegisteredTypes.Keys.Where(tr => tr.Type == resolveType);
            return registrations.Concat(parentContainer.GetParentRegistrationsForType(resolveType));
        }

        private IEnumerable<object> ResolveAllInternal(Type resolveType, bool includeUnnamed)
        {
            IEnumerable<TypeRegistration> registrations = RegisteredTypes.Keys.Where(tr => tr.Type == resolveType).Concat(GetParentRegistrationsForType(resolveType)).Distinct();
            if (!includeUnnamed)
                registrations = registrations.Where(tr => tr.Name != string.Empty);

            return registrations.Select(registration => this.ResolveInternal(registration, NamedParameterOverloads.Default, ResolveOptions.Default));
        }

        private static bool IsValidAssignment(Type registerType, Type registerImplementation)
        {
            if (!registerType.IsGenericTypeDefinition)
            {
                if (!registerType.IsAssignableFrom(registerImplementation))
                    return false;
            }
            else
            {
                if (registerType.IsInterface)
                {
                    if (!registerImplementation.FindInterfaces((t, o) => t.Name == registerType.Name, null).Any())
                        return false;
                }
                else if (registerType.IsAbstract && registerImplementation.BaseType != registerType)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                RegisteredTypes.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}
