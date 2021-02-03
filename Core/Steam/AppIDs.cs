using System.Collections.Generic;

namespace Core
{
    public class AppIDs
    {
        public const string DOTA2_ID = "570";

        // Contains vpk_linux32
        public const string SOURCE_SDK_2013_MP_ID = "244310";
        public const string SOURCE_SDK_2013_DS_ID = "243750";

        public static IEnumerable<string> AllAppIDs()
        {
            foreach (var sdk in new List<string>{DOTA2_ID, SOURCE_SDK_2013_DS_ID, SOURCE_SDK_2013_MP_ID})
                yield return sdk;
        }

        public static IEnumerable<string> SourceSDK2013()
        {
            foreach (var sdk in new List<string> { SOURCE_SDK_2013_DS_ID, SOURCE_SDK_2013_MP_ID })
                yield return sdk;
        }
    }
}
