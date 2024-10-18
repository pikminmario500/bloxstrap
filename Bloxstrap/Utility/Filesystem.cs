namespace Bloxstrap.Utility
{
    internal static class Filesystem
    {
        internal static long GetFreeDiskSpace(string path)
        {
            try
            {
                var isUri = Uri.TryCreate(p, UriKind.RelativeOrAbsolute, out var u);

    			if (!Path.IsPathRooted(p) || !Path.IsPathFullyQualified(p) || (isUri && (u?.IsUnc??false)))
                {
                    return -1;
                }

                var drive = new DriveInfo(p);
                return drive.AvailableFreeSpace;
            }
	        catch (ArgumentException e)
	        {
		        App.Logger.WriteLine("Filesystem::BadPath", $"The path: {p} does not contain a valid drive info.");

		        return -1
	        }
        }

        internal static void AssertReadOnly(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists || !fileInfo.IsReadOnly)
                return;

            fileInfo.IsReadOnly = false;
            App.Logger.WriteLine("Filesystem::AssertReadOnly", $"The following file was set as read-only: {filePath}");
        }
    }
}
