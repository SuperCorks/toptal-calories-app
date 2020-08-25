using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Calories.App.Models;
using Calories.App.Services;
using Calories.App.Entities.Users;

namespace Calories.App.Managers
{
    [Injector.Singleton]
    internal class AppManager
    {
        private AppModel AppModel => Injector.Get<AppModel>();
        private MealService MealService => Injector.Get<MealService>();
        private UserService UserService => Injector.Get<UserService>();
        private AuthenticationManager AuthManager => Injector.Get<AuthenticationManager>();

        public void InitializeApplication()
        {
            this.ExecuteSafely(async () =>
            {
                AppModel.Reset();

                await this.Login();
            });
        }

        public async Task Login()
        {
            AppModel.IsLoggingIn = true;

            try
            {
                var session = await this.AuthManager.Login();

                if (session != null)
                {
                    BaseService.Headers["auth-token"] = session.Uuid;

                    var user = AppModel.CurrentUser = session.User;

                    if (user.Role == UserRoles.Member)
                    {
                        AppModel.Meals = (await MealService.Search(user.Id)).ToList();
                    }
                    else if (user.Role == UserRoles.Manager)
                    {
                        AppModel.AllUsers = (await UserService.Search(true, false, false)).ToList();
                    }
                    else if (user.Role == UserRoles.Admin)
                    {
                        AppModel.Meals = (await MealService.Search()).ToList();
                        AppModel.AllUsers = (await UserService.Search(true, true, true)).ToList();
                    }
                }
            }
            finally
            {
                AppModel.IsLoggingIn = false;
            }
        }

        /// <summary>
        /// Executes the provided action safely by wrapping the its execution in a try/catch block. Caught errors
        /// will be displayed in the console.
        /// </summary>
        /// 
        /// <param name="endpointFunction">The action to safely await.</param>
        public void ExecuteSafely(Action endpointFunction)
        {
            try
            {
                endpointFunction();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);
            }
        }

        /// <summary>
        /// Executes the provided async function safely by wrapping the its execution in a try/catch block. Caught errors
        /// will be displayed in the console.
        /// </summary>
        /// 
        /// <param name="endpointFunction">The async function to safely await.</param>
        public async void ExecuteSafely(Func<Task> endpointFunction)
        {
            try
            {
                await endpointFunction();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);
            }
        }

        /// <summary>
        /// Awaits the provided task safely by wrapping the <see langword="await"/> a try/catch block. Caught errors
        /// will be displayed in the console.
        /// </summary>
        /// 
        /// <param name="taskToAwait">The task to safely await.</param>
        public async void AwaitSafely(Task taskToAwait)
        {
            try
            {
                await taskToAwait;
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);
            }
        }

        /// <summary>
        /// Generates a safe <see cref="Command"/>. Safe because the command's execution is wrapped in
        /// <see cref="AppManager.ExecuteSafely(Action)"/>.
        /// </summary>
        /// 
        /// <param name="asyncCommandFn">The action to invoke when the command is executed.</param>
        /// 
        /// <returns>A command that invokes the provided action when executed.</returns>
        public Command SafeCommand(Action commandFn)
        {
            return new Command(() => ExecuteSafely(commandFn));

        }

        /// <summary>
        /// Generates a safe <see cref="Command{T}"/>. Safe because the command's execution is wrapped in
        /// <see cref="AppManager.ExecuteSafely(Action)"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The Type of the parameter of the command.</typeparam>
        /// 
        /// <param name="asyncCommandFn">The action to invoke when the command is executed.</param>
        /// 
        /// <returns>A command that invokes the provided action when executed.</returns>
        public Command<T> SafeCommand<T>(Action<T> commandFn)
        {
            return new Command<T>((commandParam) => this.ExecuteSafely(() => commandFn(commandParam)));

        }

        /// <summary>
        /// Generates a safe <see cref="Command"/>. Safe because the command's execution is wrapped in
        /// <see cref="AppManager.ExecuteSafely(Action)"/>.
        /// </summary>
        /// 
        /// <param name="asyncCommandFn">The function to invoke when the command is executed.</param>
        /// 
        /// <returns>A command that invokes the provided function when executed.</returns>
        public Command SafeCommand(Func<Task> asyncCommandFn)
        {
            return new Command(() => this.ExecuteSafely(asyncCommandFn));
        }

        /// <summary>
        /// Generates a safe <see cref="Command{T}"/>. Safe because the command's execution is wrapped in
        /// <see cref="AppManager.ExecuteSafely(Action)"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The Type of the parameter of the command.</typeparam>
        /// 
        /// <param name="asyncCommandFn">The function to invoke when the command is executed.</param>
        /// 
        /// <returns>A command that invokes the provided action when executed.</returns>
        public Command<T> SafeCommand<T>(Func<T, Task> asyncCommandFn)
        {
            return new Command<T>((commandParam) => this.ExecuteSafely(async () => await asyncCommandFn(commandParam)));

        }
    }

    public static class AppManagerExtensions
    {
        /// <summary>
        /// Subscribes an safe element handler to an observable sequence. Safe because the element handler is
        /// wrapped in <see cref="AppManager.ExecuteSafely(Action)"/>. 
        /// Warning, DO NOT MISTAKE WITH <see cref="ObservableExtensions.SubscribeSafe{T}(IObservable{T}, IObserver{T})"/>
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// 
        /// <param name="observable">Observable sequence to subscribe to.</param>
        /// 
        /// <param name="observer">Action to invoke for each element in the observable sequence.</param>
        /// 
        /// <returns>
        /// <see cref="IDisposable"/> object used to unsubscribe from the observable sequence.
        /// </returns>
        public static IDisposable SubscribeSafely<T>(this IObservable<T> observable, Action<T> observer)
        {
            return observable.Subscribe((_value) =>
            {
                Injector.Get<AppManager>().ExecuteSafely(() => observer.Invoke(_value));
            });
        }

        /// <summary>
        /// Subscribes an safe async element handler to an observable sequence. Safe because the element handler is
        /// wrapped in <see cref="AppManager.ExecuteSafely(Func{Task})"/>.
        /// Warning, DO NOT MISTAKE WITH <see cref="ObservableExtensions.SubscribeSafe{T}(IObservable{T}, IObserver{T})"/>
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// 
        /// <param name="observable">Observable sequence to subscribe to.</param>
        /// 
        /// <param name="asyncObserver">Async action to invoke for each element in the observable sequence.</param>
        /// 
        /// <returns>
        /// <see cref="IDisposable"/> object used to unsubscribe from the observable sequence.
        /// </returns>
        public static IDisposable SubscribeSafely<T>(this IObservable<T> observable, Func<T, Task> asyncObserver)
        {
            return observable.Subscribe((_value) =>
            {
                Injector.Get<AppManager>().ExecuteSafely(async () => await asyncObserver.Invoke(_value));
            });
        }
    }
}
