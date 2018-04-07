using Albion_Direct;
using System.Linq;

namespace Merlin.AOKore.API
{
    public class Bank
    {
        public static long? Id
        {
            get
            {
                BankBuildingView bbv = GameManager.GetInstance().GetEntities<BankBuildingView>((i) => true).FirstOrDefault();
                if (bbv != null)
                    return bbv.Id;
                return null;
            }
        }

        public static bool IsBusy
        {
            get
            {
                BankBuildingView bbv = GameManager.GetInstance().GetEntities<BankBuildingView>((i) => true).FirstOrDefault();
                return bbv != null && bbv.Busy;
            }
        }

        public static bool IsInUseRange
        {
            get
            {
                BankBuildingView bbv = GameManager.GetInstance().GetEntities<BankBuildingView>((i) => true).FirstOrDefault();
                if (bbv != null)
                {
                    if (bbv.IsInUseRange(GameManager.GetInstance().GetLocalPlayerCharacterView().LocalPlayerCharacter))
                        return true;
                }
                return false;
            }
        }

        public static Vector3 Location
        {
            get
            {
                BankBuildingView bbv = GameManager.GetInstance().GetEntities<BankBuildingView>((i) => true).FirstOrDefault();

                if (bbv != null)
                    return new Vector3(bbv.transform.position.x, bbv.transform.position.y, bbv.transform.position.z);

                return null;
            }
        }

        public static string Name
        {
            get
            {
                BankBuildingView bbv = GameManager.GetInstance().GetEntities<BankBuildingView>((i) => true).FirstOrDefault();

                if (bbv != null)
                    return bbv.PrefabName;

                return null;
            }
        }
        public static object Object => GameManager.GetInstance().GetEntities<BankBuildingView>((i) => true).FirstOrDefault();

        public static void Close() => GameGui.Instance.MultiVaultGui.Close();

        public static bool Open()
        {
            if (Npc.BankBuildingVaultIsOpen)
                return false;

            BankBuildingView bbv = GameManager.GetInstance().GetEntities<BankBuildingView>((i) => true).FirstOrDefault();
            if (bbv != null)
            {
                if (bbv.IsInUseRange(GameManager.GetInstance().GetLocalPlayerCharacterView().LocalPlayerCharacter))
                {
                    bbv.Open();
                    return true;
                }
            }

            return false;
        }
    }
}