namespace Colors.Core
{
	public static class ColorConstants
	{
		//internal use
		public const string ENUM_NAME = "EnumColors";
		public const string INTERFACE_SERVICE_NAME = "IColorService";
		public const string IMPLEMENTATION_SERVICE_NAME = "ColorService";
		public const string SERVICE_METHOD_NAME = "Get";
		public const string COLORS_NAME = "Colors";

		public const string CONTEXT_FIELD_NAME = "_ctx";
		public const string CONTEXT_PARAMETER_NAME = "ctx";

		//file
		public const string FILE_SUFFIX = ".ComponentColor.cs";

		public const string ENUM_FILE_PATH = ENUM_NAME + FILE_SUFFIX;
		public const string INTERFACE_SERVICE_FILE_PATH = INTERFACE_SERVICE_NAME + FILE_SUFFIX;
		public const string IMPLEMENTATION_SERVICE_FILE_PATH = IMPLEMENTATION_SERVICE_NAME + FILE_SUFFIX;
		public const string COLORS_FILE_PATH = COLORS_NAME + FILE_SUFFIX;
	}
}