using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System;
using Sirenix.OdinInspector;
using Utils;
using DebugTools;

namespace EditorTool.PlayerEditor {
    public class PlayerCloudServices : MonoBehaviour {

        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn &&
            AuthenticationService.Instance.SessionTokenExists;

        public event Action OnSignedIn = delegate { };
        public event Action<string> OnSignedInFailed = delegate { };
        public event Action OnSignedOut = delegate { };

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private string _environment = "development";
#else
        private string _environment = "production";
#endif

        private string _accessToken = null;

        private async void Awake() {
            var hasInternet = await InternetConnection.HasInternet();
            if (!hasInternet) {
                DebugLog.Log("No internet connection");
                Destroy(this);
                return;
            }

            if (UnityServices.State == ServicesInitializationState.Uninitialized) {
                var options = new InitializationOptions().SetEnvironmentName(_environment);
                DebugLog.Log($"UnityServices InitializeAsync. Environment: {_environment}");
                await UnityServices.InitializeAsync(options);
            }

            AuthenticationService.Instance.SignedIn += HandleSignedIn;
            AuthenticationService.Instance.SignedOut += HandleSignedOut;
            AuthenticationService.Instance.SignInFailed += HandleSignInFailed;

            await SignIn();
        }



        private void OnDestroy() {
            if (UnityServices.State == ServicesInitializationState.Initialized) {
                AuthenticationService.Instance.SignedIn -= HandleSignedIn;
                AuthenticationService.Instance.SignedOut -= HandleSignedOut;
                AuthenticationService.Instance.SignInFailed -= HandleSignInFailed;
            }
        }

        [Button]
        private async void TestSignIn() {
            await SignIn();
        }

        public async UniTask SignIn() {
            if (!IsSignedIn) {
                try {
                    // #if UNITY_EDITOR
                    //                     // identify if it's a parrelsync clone or not
                    //                     if (ParrelSync.ClonesManager.IsClone()) {
                    //                         // Automatically connect to local host if this is the clone editor
                    //                         var username = "beto_clone";
                    //                         var password = "Aa!123456";
                    //                         DebugLog.Log($"Username: {username} Password: {password}");
                    //                         // await SignUp(username, password);
                    //                         await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                    //                     }
                    //                     else {
                    //                         await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    //                     }
                    // #else
                    //                     await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    // #endif
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                catch (AuthenticationException ex) {
                    DebugLog.LogException(ex);
                }
                catch (RequestFailedException ex) {
                    DebugLog.LogException(ex);
                }
            }
        }

        [Button]
        public void SignOut() {
            AuthenticationService.Instance.SignOut();
        }

        private void HandleSignInFailed(RequestFailedException exception) {
            DebugLog.Log(exception);
            OnSignedInFailed?.Invoke(exception.Message);
        }

        private void HandleSignedIn() {
            _accessToken = AuthenticationService.Instance.AccessToken;
            DebugLog.Log($"Sign in PlayerId: {AuthenticationService.Instance.PlayerId}");
            OnSignedIn?.Invoke();
        }

        private void HandleSignedOut() {
            DebugLog.Log($"Sign out PlayerId: {AuthenticationService.Instance.PlayerId}");
            OnSignedOut?.Invoke();
        }

    }
}