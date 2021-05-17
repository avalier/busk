using System.Collections.Generic;

namespace Avalier.Busk.Example.Host.Services
{
    public interface IPrimeService
    {
        IEnumerable<long> GetPrimesUpTo(long index);
    }
}