using System;
using System.Configuration;

namespace Hack_the_Browser.Config
{
    internal static class EncryptionConfig
    {
        internal static readonly byte[] AesKey = Convert.FromBase64String("7soy64n2dREZQ1sdGSYrIjMyFsxf3GUNOMRFCBVlF4U=");
        internal static readonly byte[] HmacKey = Convert.FromBase64String("s4Wlc6whbqRE22ctAHbjeNIWtZd+NcdQwKROc6C3zobGCw4zU4QQt3Udkv3tx+gtDq8Frp7aDz5uE+NFxKu1SVXZVx+RIlBetijqHsmKUWg/LYZXTqD7HPATOs9m6cPfGViNyA5lAxSOxbRS7wi0LEtrq1KQANw/w9N+AyKOJR4=");
    }
}
