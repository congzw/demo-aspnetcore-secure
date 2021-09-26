using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CookieSample.Demo
{
    public interface ILockUserManager
    {
        bool IsLocked(string user);
        void SetLock(string user, bool locked);
        List<string> GetLockUsers();
    }

    class LockUserManager : ILockUserManager
    {
        public IDictionary<string, bool> LockUsers { get; set; } = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        public bool IsLocked(string user)
        {
            if (string.IsNullOrWhiteSpace(user))
            {
                return false;
            }
            LockUsers.TryGetValue(user, out var locked);
            return locked;
        }

        public void SetLock(string user, bool locked)
        {
            if (locked)
            {
                LockUsers[user] = true;
            }
            else
            {
                LockUsers.Remove(user);
            }
        }

        public List<string> GetLockUsers()
        {
            return LockUsers.Keys.ToList();
        }
    }
}