using Albion_Direct;
using System.Linq;

using UnityObject = UnityEngine.Object;

namespace Merlin.AOKore.API
{
    public static class Account
    {
        public static LoginInfo[] CharacterList
        {
            get
            {
                CharacterSelectionDialog chsd = UnityObject.FindObjectsOfType<CharacterSelectionDialog>().FirstOrDefault();
                if (chsd == null)
                    return null;

                return chsd.ParentGui.CharacterList.Select(i => new LoginInfo(i)).ToArray();
            }
        }

        public static bool Error
        {
            get
            {
                LoginGui gui = UnityObject.FindObjectsOfType<LoginGui>().FirstOrDefault();
                if (gui != null)
                {
                    if (gui.ErrorDialog.isActiveAndEnabled)
                        return true;

                    if (gui.ServerStatusDialog.isActiveAndEnabled)
                        return true;
                }
                return false;
            }
        }

        public static string ErrorDesc
        {
            get
            {
                LoginGui gui = UnityObject.FindObjectsOfType<LoginGui>().FirstOrDefault();
                if (gui != null)
                {
                    if (gui.ErrorDialog.isActiveAndEnabled && gui.ServerStatusDialog.isActiveAndEnabled)
                        return "LoginError: " + gui.ErrorDialog.ErrorText.text + " Server: " + gui.ServerStatusDialog.Status.text;

                    if (gui.ErrorDialog.isActiveAndEnabled)
                        return gui.ErrorDialog.ErrorText.text;

                    if (gui.ServerStatusDialog.isActiveAndEnabled)
                        return gui.ServerStatusDialog.Status.text;
                }
                return "null";
            }
        }

        public static bool HasCharacterSelectionDialog => UnityObject.FindObjectsOfType<CharacterSelectionDialog>().Any();

        public static bool HasLoginDialog => UnityObject.FindObjectsOfType<LoginDialog>().Any();

        public static bool HasQueueWaitingBox
        {
            get
            {
                CharacterSelectionDialog chsd = UnityObject.FindObjectsOfType<CharacterSelectionDialog>().FirstOrDefault();
                return chsd != null && chsd.WaitingQueueBox.activeInHierarchy;
            }
        }

        public static string HasWaitingQueueBoxText
        {
            get
            {
                CharacterSelectionDialog chsd = UnityObject.FindObjectsOfType<CharacterSelectionDialog>().FirstOrDefault();
                if (chsd == null)
                    return null;

                if (chsd.WaitingQueueBox.activeInHierarchy && !string.IsNullOrEmpty(chsd.WaitingQueueLabel.text))
                    return chsd.WaitingQueueLabel.text;

                return null;
            }
        }

        public static string ServerName
        {
            get
            {
                LoginDialog ld = UnityObject.FindObjectsOfType<LoginDialog>().FirstOrDefault();
                return ld?.ServerNameInput.value;
            }
        }

        public static void AutoLogin(bool value) => LoginGui.AutoLogin = value;

        public static void Create(string charname)
        {
            CharacterSelectionDialog chsd = UnityObject.FindObjectsOfType<CharacterSelectionDialog>().FirstOrDefault();
            if (chsd != null)
            {
                //TODO: generate API
                chsd.ParentGui.SwitchState(LoginGui.a.e);
                CreateCharacterDialog cchd = UnityEngine.Object.FindObjectsOfType<CreateCharacterDialog>().FirstOrDefault();
                if (cchd != null)
                    cchd.CreateCharacter(charname);
            }
        }

        public static void Delete(string charname)
        {
            CharacterSelectionDialog chsd = UnityObject.FindObjectsOfType<CharacterSelectionDialog>().FirstOrDefault();
            if (chsd != null)
            {
                //TODO: Generate API
                PhotonClient.GetInstance().GetLoginPeer().g(charname);
            }
        }

        public static bool ErrorConfirm()
        {
            LoginGui gui = UnityObject.FindObjectsOfType<LoginGui>().FirstOrDefault();
            if (gui != null)
            {
                if (gui.ErrorDialog.isActiveAndEnabled && gui.ServerStatusDialog.isActiveAndEnabled)
                {
                    gui.ServerStatusDialog.ShowWindow(false);
                    gui.ErrorDialog.OnClickOk();
                    return true;
                }

                if (gui.ErrorDialog.isActiveAndEnabled)
                {
                    gui.ErrorDialog.OnClickOk();
                    return true;
                }

                if (gui.ServerStatusDialog.isActiveAndEnabled)
                {
                    gui.ServerStatusDialog.ShowWindow(false);
                    return true;
                }
            }
            return false;
        }
        public static void LoginAccount(string accountEmail, string accountPassword)
        {
            LoginDialog ld = UnityObject.FindObjectsOfType<LoginDialog>().FirstOrDefault();
            if (ld != null)
            {
                ld.AccountEmailInput.value = accountEmail;
                ld.AccountPasswordInput.value = accountPassword;
                //TODO: Generate API
                ld.ParentGui.Connect(ld.ServerNameInput.value, LoginGui.a.k);
            }
        }

        public static void LoginAccount(string accountEmail, string accountPassword, string server)
        {
            LoginDialog ld = UnityObject.FindObjectsOfType<LoginDialog>().FirstOrDefault();
            if (ld != null)
            {
                ld.AccountEmailInput.value = accountEmail;
                ld.AccountPasswordInput.value = accountPassword;
                if (string.IsNullOrEmpty(server))
                {
                    //TODO: Generate API
                    ld.ParentGui.Connect(ld.ServerNameInput.value, LoginGui.a.k);
                    return;
                }
                //TODO: Generate API
                ld.ParentGui.Connect(server, LoginGui.a.k);
            }
        }

        public static void LoginCharacter(string charname)
        {
            CharacterSelectionDialog chsd = UnityObject.FindObjectsOfType<CharacterSelectionDialog>().FirstOrDefault();
            if (chsd != null)
            {
                //TODO: Generate API
                PhotonClient.GetInstance().GetLoginPeer().h(charname);
            }
        }
        //TODO: Generate API
        public class LoginInfo
        {
            private LoginGui.b _source;

            internal LoginInfo(LoginGui.b source)
            {
                _source = source;
            }

            public string CharacterName => _source.y();

            public string GuildName => _source.ae();

            public override string ToString() => string.Format("{0}", CharacterName);
        }
    }
}