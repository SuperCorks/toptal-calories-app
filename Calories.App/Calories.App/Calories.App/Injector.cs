using Ninject;
using Ninject.Modules;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Calories.App
{
    public enum ActivationTypes
    {
        /// <summary>Activated the first time <see cref="Injector.Get"/> is called for that service.</summary>
        Lazy,

        /// <summary>Activated when <see cref="Injector.PerformEagerActivations"/> is called.</summary>
        Eager
    }

    public static class Injector
    {
        public static bool IsInitialized { get; set; } = false;

        private static bool EagerActivationHasBeenPerformed = false;

        private readonly static List<Type> EagerSingletons = new List<Type>();

        private static IKernel Kernel { get; set; } = new StandardKernel(new NinjectSettings());

        /// <summary>Registers a new dependency in the injector.</summary>
        /// <typeparam name="T">The type to override with the factory.</typeparam>
        /// <returns>Ninject's fluent syntax.</returns>
        public static IBindingToSyntax<T> Bind<T>()
        {
            return Kernel.Bind<T>();
        }

        /// <summary>
        /// Verifies that a module of the provided type is loaded. The type comparison is exact.
        /// </summary>
        /// 
        /// <param name="moduleType">The type of the module to check.</param>
        /// 
        /// <returns><see langword="true"/> if the module is loaded. <see cref="false"/> otherwise.</returns>
        public static bool IsLoaded(Type moduleType)
        {
            foreach (var module in Kernel.GetModules())
            {
                if (module.GetType() == moduleType) return true;
            }

            return false;
        }

        /// <summary>Alias of <see cref="IsLoaded(Type)"/>.</summary>
        public static bool IsLoaded<ModuleType>()
        {
            return IsLoaded(typeof(ModuleType));
        }

        /// <summary>Gets an instance of the specified service.</summary>
        /// 
        /// <typeparam name="T">The type of the service to resolve.</typeparam>
        /// 
        /// <returns>An instance of the service.</returns>
        /// 
        /// <seealso cref="Get(Type)"/>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        /// <summary>Gets an instance of the specified service.</summary>
        /// 
        /// <param name="type">The service to resolve.</param>
        /// 
        /// <returns>An instance of the service.</returns>
        /// 
        /// <seealso cref="Get{T}"/>
        public static object Get(Type type)
        {
            return Kernel.Get(type);
        }

        /// <summary>Loads the module into the injector.</summary>
        /// 
        /// <param name="module">The module to load.</param>
        public static void LoadModule(INinjectModule module)
        {
            Kernel.Load(module);
        }

        /// <summary>
        /// Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the service to rebind.</typeparam>
        /// 
        /// <returns>Ninject's fluent syntax.</returns>
        /// 
        /// <example>
        /// <code>
        /// Injector.Rebind{MyServiceInterface}().To{MyConcreteService}(); // Rebind interfaces and abstract classes
        /// Injector.Rebind{MyServiceClass}().ToSelf().InSingletonScope(); // Rebind as singletons
        /// Injector.Rebind{MyServiceClass}().ToConstant(new MyServiceClass(someParam)); // Rebind as constant
        /// </code>
        /// </example>
        public static IBindingToSyntax<T> Rebind<T>()
        {
            return Kernel.Rebind<T>();
        }

        /// <summary>
        /// Shortcut function to define a singleton service. This method has to be called before 
        /// <see cref="PerformEagerActivations"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the service to rebind as singleton.</typeparam>
        /// 
        /// <param name="type">The type of the service.</param>
        /// 
        /// <param name="activation">The service's activation type.</param>
        /// 
        /// <exception cref="ArgumentException">Raised if the singleton type has already been registered.</exception>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Raised if this has been called after <see cref="PerformEagerActivations"/>.
        /// </exception>
        public static void Singleton<T>(ActivationTypes activation = ActivationTypes.Lazy)
        {
            if (EagerActivationHasBeenPerformed)
                throw new InvalidOperationException("Eager activation has already been performed!");

            Rebind<T>().ToSelf().InSingletonScope();

            if (activation == ActivationTypes.Eager) EagerSingletons.Add(typeof(T));
        }

        /// <summary>
        /// Shortcut function to define a singleton service. This method has to be called before 
        /// <see cref="PerformEagerActivations"/>.
        /// </summary>
        /// 
        /// <param name="type">The type of the service.</param>
        /// 
        /// <param name="activation">The service's activation type.</param>
        /// 
        /// <exception cref="ArgumentException">Raised if the singleton type has already been registered.</exception>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Raised if this has been called after <see cref="PerformEagerActivations"/>.
        /// </exception>
        public static void Singleton(Type type, ActivationTypes activation = ActivationTypes.Lazy)
        {
            if (EagerActivationHasBeenPerformed)
                throw new InvalidOperationException("Eager activation has already been performed!");

            Kernel.Rebind(type).ToSelf().InSingletonScope();

            if (activation == ActivationTypes.Eager) EagerSingletons.Add(type);
        }

        /// <summary>
        /// Activates singletons that have been marked with the <see cref="ActivationTypes.Eager"/> activation type.
        /// </summary>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Raised if this method is called twice (without <see cref="Reset"/> being called in between).
        /// </exception>
        public static void PerformEagerActivations()
        {
            if (EagerActivationHasBeenPerformed)
                throw new InvalidOperationException("Eager activation has already been performed!");

            foreach (var type in EagerSingletons)
            {
                Injector.Get(type);
            }
        }

        /// <summary>
        /// Resets the injector and all its bindings.
        /// </summary>
        public static void Reset()
        {
            EagerSingletons.Clear();
            EagerActivationHasBeenPerformed = false;

            var oldKernel = Kernel;

            Kernel = new StandardKernel();

            oldKernel.Dispose();
        }

        public static void GetSingletonsIn(Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.GetCustomAttribute<SingletonAttribute>() is SingletonAttribute attribute)
                {
                    Injector.Singleton(type, attribute.Activation);
                }
            }
        }

        internal static void GetSingletonsIn(object assembly)
        {
            throw new NotImplementedException();
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class SingletonAttribute : Attribute
        {
            public readonly ActivationTypes Activation;

            public SingletonAttribute(ActivationTypes activation = ActivationTypes.Lazy)
            {
                this.Activation = activation;
            }
        }
    }
}
