namespace Merlin.AOKore.API
{
    public static class Npc
    {
        public static bool BankBuildingVaultIsOpen
        {
            get
            {
                return GameGui.Instance.BuildingUsageAndManagementGui.isActiveAndEnabled;
            }
        }
    }
}