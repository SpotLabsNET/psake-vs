namespace PSake.TaskRunner
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string guidAddCommandPackageString = "aa15781b-46bd-4f94-8289-22e739af9e60";
        public const string guidCommandCmdSetString = "cbc7a17e-ebf5-489d-a28a-e28055a632dc";
        public static Guid guidAddCommandPackage = new Guid(guidAddCommandPackageString);
        public static Guid guidCommandCmdSet = new Guid(guidCommandCmdSetString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int AddCommandId = 0x0064;
    }
}
