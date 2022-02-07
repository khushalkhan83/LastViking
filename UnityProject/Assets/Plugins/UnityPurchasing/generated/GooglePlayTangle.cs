#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("N1yVq5uzxSLXqGhlyY9gVFHke+mcgIXBa7WUzd7qRyP59v/BVyld2HbEoORsP3gxR+lob1Fkfx4ygXKigMg/5BSvm0v0JbKBQ7BB4407u2mG+l+RbKPRy0EppFXAIaRF8uw/X7w/MT4OvD80PLw/Pz6ca259G0gSnmSQTR58ygzR9JqhueIRK/KWAa49uljo9csA+4zDHf37eRLya8Swg05tXXCm3xoEkDHTbyEdpX6I5YqWhPCkqM3gz8NN4nUesbJ0YQguxMWbBR1lFnrjFrC+fTXmmwDkkg+Sww68PxwOMzg3FLh2uMkzPz8/Oz49lFvgu4Xh0qckzUN3YUnIl7LQkiIyjeBnGKQHALEi1AZFkTSgqEsYJ2orOBAmZbCqYzw9Pz4/");
        private static int[] order = new int[] { 4,11,3,3,13,11,11,8,8,12,10,13,12,13,14 };
        private static int key = 62;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
