using System;

namespace SaveSystem
{
    public static class IdUtils
    {
        public static string GenerateId()
        {
            Guid id = Guid.NewGuid();
            return id.ToString();
        }
    }
}