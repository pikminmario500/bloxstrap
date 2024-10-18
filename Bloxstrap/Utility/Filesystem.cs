namespace Bloxstrap.Utility
{
    internal static class Filesystem
    {
        const string LOG_IDENT = "Filesystem::";

        internal static long GetFreeDiskSpace(string path)
        {
            try
            {
                var isUri = Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var u);

    		    if (!Path.IsPathRooted(path) || !Path.IsPathFullyQualified(path) || (isUri && (u?.IsUnc??false)))
                {
                    return -1;
                }

                var drive = new DriveInfo(path);
                return drive.AvailableFreeSpace;
            }
	        catch (Exception ex)
	        {
		        App.Logger.WriteLine($"{LOG_IDENT}BadPath", $"The path: {path} does not contain a valid drive info.");
			    App.Logger.WriteException($"{LOG_IDENT}BadPath", ex);

		        return -1;
	        }
        }

        internal static void AssertReadOnly(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists || !fileInfo.IsReadOnly)
                return;

            fileInfo.IsReadOnly = false;
            App.Logger.WriteLine($"{LOG_IDENT}AssertReadOnly", $"The following file was set as read-only: {filePath}");
        }
    }
}
