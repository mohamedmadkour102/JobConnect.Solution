namespace JobConnect.Apis.Helpers
{
	public static class FileHelper
	{
		/// <summary>
		/// Converts a file to Base64 string asynchronously.
		/// </summary>
		/// <param name="filePath">The full path to the file on the server.</param>
		/// <returns>A Base64 string representation of the file, or null if the file doesn't exist.</returns>
		/// <exception cref="ArgumentNullException">Thrown if filePath is null or empty.</exception>
		public static async Task<string> ConvertFileToBase64Async(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");

			if (!File.Exists(filePath))
				return null;

			var fileBytes = await File.ReadAllBytesAsync(filePath);
			return Convert.ToBase64String(fileBytes);
		}

		/// <summary>
		/// Converts a file to Base64 string synchronously.
		/// </summary>
		/// <param name="filePath">The full path to the file on the server.</param>
		/// <returns>A Base64 string representation of the file, or null if the file doesn't exist.</returns>
		/// <exception cref="ArgumentNullException">Thrown if filePath is null or empty.</exception>
		public static string ConvertFileToBase64(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");

			if (!File.Exists(filePath))
				return null;

			var fileBytes = File.ReadAllBytes(filePath);
			return Convert.ToBase64String(fileBytes);
		}

		/// <summary>
		/// Combines the web root path with a relative file path and converts the file to Base64 asynchronously.
		/// </summary>
		/// <param name="webRootPath">The web root path (e.g., wwwroot).</param>
		/// <param name="relativePath">The relative path to the file (e.g., /Uploads/resumes/filename.pdf).</param>
		/// <returns>A Base64 string representation of the file, or null if the file doesn't exist.</returns>
		public static async Task<string> ConvertRelativeFileToBase64Async(string webRootPath, string relativePath)
		{
			if (string.IsNullOrEmpty(webRootPath) || string.IsNullOrEmpty(relativePath))
				return null;

			var fullPath = Path.Combine(webRootPath, relativePath.TrimStart('/'));
			return await ConvertFileToBase64Async(fullPath);
		}
	}
}
