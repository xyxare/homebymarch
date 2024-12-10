// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("0NR9FOaTcZ757N8pZTt2oQC8/1bOgTmXW59A1al2SqH7UoGzslcOqiKwQhbl2jS8Mr1JY9vM9VZsD89t3WIAD8S2r3yiaV0BTX9RwUweNH7Wbn2u/pnL7PW/ZWBpFn+t2V7yRIdMYnD1jtcqyK7FjxEGZABqoUawAYKMg7MBgomBAYKCgzbNve0K4UWzAYKhs46FiqkFywV0joKCgoaDgDcT7jKZtQQpTIQhdRECCypJIkXEsrimYtVcE+8R2pFY6zjjyELNgRxPLg9r7ikDXSlv8iwi89CcaT3LJXodt4PjvGpH3/OHDRGdyTXRScyhwRrHQpK0hddyr3wH2WRnNxSfOzD4yoITNOXO8GjGtPSYPLN6ngiln4RcOznV8vNtKIGAgoOC");
        private static int[] order = new int[] { 6,11,12,5,8,9,11,11,9,12,13,12,13,13,14 };
        private static int key = 131;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
