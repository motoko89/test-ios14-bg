using System;
using System.Threading.Tasks;

namespace TestBg.Services
{
    public interface IBackgroundService
    {
        Task PerformFetch();
    }
}
