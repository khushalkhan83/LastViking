#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class UnityChannelTangle
    {
        private static byte[] data = System.Convert.FromBase64String("7e57q5L3LFCGbAht7vXQcIa9joGWE7kqTyo6OMYcqYEBWnmNubIIVSAFX8n/X002vIFqvl06g1q0qfCaK2W6JBTgMrPfq0hOwtxWbRrn8ucMXQFFK+iK165a0IRYrSeSV//LVaj/fQrctrWlBf4joh5bZJQtZH6LdkcH7aav07E3aYCFsT6Yze4AiDOOuWk9GjGdek4YL5r71yfXTpooaW6SnAYMHmt3M38xVjOSeL9rFfb1iji7mIq3vLOQPPI8Tbe7u7u/urkrpYujVtR01HYCUuTB7qxSMOMhMXR0n6mIquXhyuS6vJhKqdlzLPTAOLu1uoo4u7C4OLu7umX7m7Dssp8adSCckn9FlvHcSFl6PmXOhHFEMYOaPo92s3X6lbi5u7q7");
        private static int[] order = new int[] { 8,6,3,13,13,5,12,9,11,11,11,13,12,13,14 };
        private static int key = 186;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
