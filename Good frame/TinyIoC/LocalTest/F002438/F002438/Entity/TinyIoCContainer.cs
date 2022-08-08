using F002438.CoreException;
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


        public void AutoRegister()
        {
            AutoRegisterInternal(new Assembly[] { this.GetType().Assembly }, DuplicateImplementationActions.RegisterSingle, null);
        }

        public void AutoRegister(Func<Type, bool> registrationPredicate)
        {
            AutoRegisterInternal(new Assembly[] { this.GetType().Assembly }, DuplicateImplementationActions.RegisterSingle, registrationPredicate);
        }

        public void AutoRegister(DuplicateImplementationActions duplicateAction)
        {
            AutoRegisterInternal(new Assembly[] { this.GetType().Assembly }, duplicateAction, null);
        }

        public void AutoRegister(DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate)
        {
            AutoRegisterInternal(new Assembly[] { this.GetType().Assembly }, duplicateAction, registrationPredicate);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies)
        {
            AutoRegisterInternal(assemblies, DuplicateImplementationActions.RegisterSingle, null);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies, Func<Type, bool> registrationPredicate)
        {
            AutoRegisterInternal(assemblies, DuplicateImplementationActions.RegisterSingle, registrationPredicate);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction)
        {
            AutoRegisterInternal(assemblies, duplicateAction, null);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate)
        {
            AutoRegisterInternal(assemblies, duplicateAction, registrationPredicate);
        }

        public RegisterOptions Register(Type registerType)
        {
            return RegisterInternal(registerType, string.Empty, GetDefaultObjectFactory(registerType, registerType));
        }

        public RegisterOptions Register(Type registerType, string name)
        {
            return RegisterInternal(registerType, name, GetDefaultObjectFactory(registerType, registerType));

        }


        public RegisterOptions Register(Type registerType, Type registerImplementation)
        {
            return this.RegisterInternal(registerType, string.Empty, GetDefaultObjectFactory(registerType, registerImplementation));
        }

        public RegisterOptions Register(Type registerType, Type registerImplementation, string name)
        {
            return this.RegisterInternal(registerType, name, GetDefaultObjectFactory(registerType, registerImplementation));
        }

        public RegisterOptions Register(Type registerType, object instance)
        {
            return RegisterInternal(registerType, string.Empty, new InstanceFactory(registerType, registerType, instance));
        }
        public RegisterOptions Register(Type registerType, object instance, string name)
        {
            return RegisterInternal(registerType, name, new InstanceFactory(registerType, registerType, instance));
        }

        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance)
        {
            return RegisterInternal(registerType, string.Empty, new InstanceFactory(registerType, registerImplementation, instance));
        }

        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name)
        {
            return RegisterInternal(registerType, name, new InstanceFactory(registerType, registerImplementation, instance));
        }

        public RegisterOptions Register(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory)
        {
            return RegisterInternal(registerType, string.Empty, new DelegateFactory(registerType, factory));
        }

        public RegisterOptions Register(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory, string name)
        {
            return RegisterInternal(registerType, name, new DelegateFactory(registerType, factory));
        }

        public RegisterOptions Register<RegisterType>()
            where RegisterType : class
        {
            return this.Register(typeof(RegisterType));
        }

        public RegisterOptions Register<RegisterType>(string name)
            where RegisterType : class
        {
            return this.Register(typeof(RegisterType), name);
        }

        public RegisterOptions Register<RegisterType, RegisterImplementation>()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return this.Register(typeof(RegisterType), typeof(RegisterImplementation));
        }

        public RegisterOptions Register<RegisterType, RegisterImplementation>(string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return this.Register(typeof(RegisterType), typeof(RegisterImplementation), name);
        }

        public RegisterOptions Register<RegisterType>(RegisterType instance)
           where RegisterType : class
        {
            return this.Register(typeof(RegisterType), instance);
        }

        public RegisterOptions Register<RegisterType>(RegisterType instance, string name)
            where RegisterType : class
        {
            return this.Register(typeof(RegisterType), instance, name);
        }

        public RegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return this.Register(typeof(RegisterType), typeof(RegisterImplementation), instance);
        }

        public RegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance, string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return this.Register(typeof(RegisterType), typeof(RegisterImplementation), instance, name);
        }

        public RegisterOptions Register<RegisterType>(Func<TinyIoCContainer, NamedParameterOverloads, RegisterType> factory)
            where RegisterType : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            return this.Register(typeof(RegisterType), (c, o) => factory(c, o));
        }

        public RegisterOptions Register<RegisterType>(Func<TinyIoCContainer, NamedParameterOverloads, RegisterType> factory, string name)
            where RegisterType : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            return this.Register(typeof(RegisterType), (c, o) => factory(c, o), name);
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
                    throw new ArgumentException(String.Format("types: The type {0} is not assignable from {1}", registrationType.FullName, type.FullName));

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
                registerOptions.Add(Register(registrationType, type, type.FullName));
            }

            return new MultiRegisterOptions(registerOptions);
        }

        #region Unregistration

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

        public bool Unregister(Type registerType, string name)
        {
            var typeRegistration = new TypeRegistration(registerType, name);

            return RemoveRegistration(typeRegistration);
        }

        #endregion

        #region Resolution

        public object Resolve(Type resolveType)
        {
            return ResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, ResolveOptions.Default);
        }


        public object Resolve(Type resolveType, ResolveOptions options)
        {
            return ResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, options);
        }

        public object Resolve(Type resolveType, string name)
        {
            return ResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default, ResolveOptions.Default);
        }

        public object Resolve(Type resolveType, string name, ResolveOptions options)
        {
            return ResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default, options);
        }

        public object Resolve(Type resolveType, NamedParameterOverloads parameters)
        {
            return ResolveInternal(new TypeRegistration(resolveType), parameters, ResolveOptions.Default);
        }

        public object Resolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return ResolveInternal(new TypeRegistration(resolveType), parameters, options);
        }

        public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters)
        {
            return ResolveInternal(new TypeRegistration(resolveType, name), parameters, ResolveOptions.Default);
        }

        public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return ResolveInternal(new TypeRegistration(resolveType, name), parameters, options);
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
                get
                {
                    return new SingletonFactory(this.registerType, this.registerImplementation);
                }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                return new CustomObjectLifetimeFactory(this.registerType, this.registerImplementation, lifetimeProvider, errorString);
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get
                {
                    return this;
                }
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
                get
                {
                    return new DelegateSingletonFactory(registerType, factory);
                }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get
                {
                    return new WeakDelegateFactory(this.registerType, factory);
                }
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    return this;
                }
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
                get
                {
                    return this;
                }
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
                get
                {
                    return new WeakInstanceFactory(this.registerType, this.registerImplementation, this.instance);
                }
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    return this;
                }
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
                get
                {
                    return new MultiInstanceFactory(this.registerType, this.registerImplementation);
                }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get
                {
                    return this;
                }
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

                if (_Current != null) return _Current;

                lock (SingletonLock)
                    if (current == null)
                        current = container.ConstructType(requestedType, this.registerImplementation, Constructor, options);

                return current;
            }

            public override ObjectFactoryBase SingletonVariant
            {
                get
                {
                    return this;
                }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                return new CustomObjectLifetimeFactory(this.registerType, this.registerImplementation, lifetimeProvider, errorString);
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get
                {
                    return new MultiInstanceFactory(this.registerType, this.registerImplementation);
                }
            }

            public override ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent, TinyIoCContainer child)
            {
                GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
                return this;
            }

            public void Dispose()
            {
                if (this.current == null)
                    return;

                IDisposable disposable = this.current as IDisposable;
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
                get { return this.registerImplementation; }
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
        #region Singleton Container

        private static readonly TinyIoCContainer _Current = new TinyIoCContainer();

        static TinyIoCContainer()
        {
        }

        public static TinyIoCContainer Current
        {
            get
            {
                return _Current;
            }
        }

        #endregion

        /// <summary>
        /// 类型 Type 和 名称Name
        /// 
        /// 的数据结构封装
        /// </summary>
        public sealed class TypeRegistration
        {
            private int hashCode;
            public Type Type { get; private set; }
            public string Name { get; private set; }

            public TypeRegistration(Type type)
                : this(type, string.Empty)
            {
            }

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
        private readonly object AutoRegisterLock = new object();
        private void AutoRegisterInternal(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate)
        {
            Type typeOfThis = this.GetType();
            lock (AutoRegisterLock)
            {
                List<Type> types = assemblies.SelectMany(a => a.SafeGetTypes()).Where(t => !IsIgnoredType(t, registrationPredicate)).ToList();
                List<Type> concreteTypes = types
                    .Where(type => type.IsClass && (type.IsAbstract == false) && (type != typeOfThis && (type.DeclaringType != typeOfThis) && (!type.IsGenericTypeDefinition())) && !type.IsNestedPrivate())
                    .ToList();

                foreach (var type in concreteTypes)
                {
                    try
                    {
                        RegisterInternal(type, string.Empty, GetDefaultObjectFactory(type, type));
                    }
                    catch (MethodAccessException)

                    {
                    }
                }

                IEnumerable<Type> abstractInterfaceTypes = from type in types
                                                           where ((type.IsInterface || type.IsAbstract) && (type.DeclaringType != typeOfThis) && (!type.IsGenericTypeDefinition))
                                                           select type;

                foreach (Type type in abstractInterfaceTypes)
                {
                    Type localType = type;
                    IEnumerable<Type> implementations = from implementationType in concreteTypes
                                                        where localType.IsAssignableFrom(implementationType)
                                                        select implementationType;

                    if (implementations.Skip(1).Any())
                    {
                        if (duplicateAction == DuplicateImplementationActions.Fail)
                            throw new TinyIoCAutoRegistrationException(type, implementations);

                        if (duplicateAction == DuplicateImplementationActions.RegisterMultiple)
                        {
                            RegisterMultiple(type, implementations);
                        }
                    }

                    var firstImplementation = implementations.FirstOrDefault();
                    if (firstImplementation != null)
                    {
                        try
                        {
                            RegisterInternal(type, string.Empty, GetDefaultObjectFactory(type, firstImplementation));
                        }
                        catch (MethodAccessException)
                        {

                        }
                    }
                }
            }
        }

        private static readonly IReadOnlyList<Func<Assembly, bool>> ignoredAssemlies = new List<Func<Assembly, bool>>()
        {
            asm => asm.FullName.StartsWith("Microsoft.", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("System.", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("System,", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("CR_ExtUnitTest", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("mscorlib,", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("CR_VSTest", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("DevExpress.CodeRush", StringComparison.Ordinal),
            asm => asm.FullName.StartsWith("xunit.", StringComparison.Ordinal),
        };

        private bool IsIgnoredAssembly(Assembly assembly)
        {
            for (int i = 0; i < ignoredAssemlies.Count; i++)
            {
                if (ignoredAssemlies[i].Invoke(assembly))
                    return true;
            }

            return false;
        }

        private static readonly IReadOnlyList<Func<Type, bool>> ignoreChecks = new List<Func<Type, bool>>()
        {
            t => t.FullName.StartsWith("System.", StringComparison.Ordinal),
            t => t.FullName.StartsWith("Microsoft.", StringComparison.Ordinal),
            t => t.IsPrimitive,
            t => t.IsGenericTypeDefinition,
            t => (t.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length == 0) && !(t.IsInterface || t.IsAbstract),
        };

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

        private bool RemoveRegistration(TypeRegistration typeRegistration)
        {
            return RegisteredTypes.Remove(typeRegistration);
        }

        private ObjectFactoryBase GetDefaultObjectFactory(Type registerType, Type registerImplementation)
        {
            if (registerType.IsInterface || registerType.IsAbstract)
                return new SingletonFactory(registerType, registerImplementation);

            return new MultiInstanceFactory(registerType, registerImplementation);
        }

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

            if (!string.IsNullOrEmpty(name) && options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
                return (parentContainer != null) ? parentContainer.CanResolveInternal(registration, parameters, options) : false;

            if (!string.IsNullOrEmpty(name) && options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
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

        private object ResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options)
        {
            ObjectFactoryBase factory;
            if (RegisteredTypes.TryGetValue(registration, out factory))
            {
                try
                {
                    return factory.GetObject(registration.Type, this, parameters, options);
                }
                catch (TinyIoCResolutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new TinyIoCResolutionException(registration.Type, ex);
                }
            }

            ObjectFactoryBase bubbledObjectFactory = GetParentObjectFactory(registration);
            if (bubbledObjectFactory != null)
            {
                try
                {
                    return bubbledObjectFactory.GetObject(registration.Type, this, parameters, options);
                }
                catch (TinyIoCResolutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new TinyIoCResolutionException(registration.Type, ex);
                }
            }

            if (!string.IsNullOrEmpty(registration.Name) && options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
                throw new TinyIoCResolutionException(registration.Type);

            if (!string.IsNullOrEmpty(registration.Name) && options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
            {
                if (RegisteredTypes.TryGetValue(new TypeRegistration(registration.Type, string.Empty), out factory))
                {
                    try
                    {
                        return factory.GetObject(registration.Type, this, parameters, options);
                    }
                    catch (TinyIoCResolutionException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new TinyIoCResolutionException(registration.Type, ex);
                    }
                }
            }

            if (IsIEnumerableRequest(registration.Type))
                return GetIEnumerableRequest(registration.Type);

            if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) || (registration.Type.IsGenericType && options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly))
            {
                if (!registration.Type.IsAbstract && !registration.Type.IsInterface)
                    return ConstructType(null, registration.Type, parameters, options);
            }

            throw new TinyIoCResolutionException(registration.Type);
        }

        private object GetIEnumerableRequest(Type type)
        {
            MethodInfo genericResolveAllMethod = this.GetType().GetGenericMethod(BindingFlags.Public | BindingFlags.Instance, "ResolveAll", type.GetGenericArguments(), new[] { typeof(bool) });
            return genericResolveAllMethod.Invoke(this, new object[] { false });
        }

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

        private ConstructorInfo GetBestConstructor(Type type, NamedParameterOverloads parameters, ResolveOptions options)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            if (type.IsValueType)
                return null;
            IEnumerable<ConstructorInfo> ctors = TinyIoCReflectionCache.GetUsableConstructors(type);
            foreach (ConstructorInfo ctor in ctors)
            {
                if (this.CanConstruct(ctor, parameters, options))
                    return ctor;
            }

            return null;
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
